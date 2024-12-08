using System.Collections;
//using UnityEditor.Build.Content;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    [Header("----- Stats -----")]
    [SerializeField] [Range(1, 5)]float speed;
    [SerializeField] [Range(1, 4)]float sprintMod;
    [SerializeField] [Range(1, 3)]int jumpMax;
    [SerializeField] [Range(5, 20)]int jumpSpeed;
    [SerializeField] [Range(15, 40)]int gravity;
    [SerializeField] [Range(10, 30)]int HP;

    [Header("----- Mana Stats -----")] // Ethan: added this line
    [SerializeField] [Range(50, 200)]int maxMana; // Ethan: added this line
    [SerializeField] [Range (1, 10)]float manaRegenRate; // Ethan: added this line
    [SerializeField] [Range (10, 50)] int specialAbilityManaCost = 20; // Ethan: added this line

    [Header("----- Gun Stats -----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;
    int currentMana; // Ethan: added this line
    int currentHP; // Ethan: added this line

    bool isShooting;
    bool isSprinting;
    bool canRegenMana = true; // Ethan: added this line

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        maxMana = currentMana; // Ethan: added this line
        updatePlayerUI(); // Ethan: added this line
        StartCoroutine(manaRegeneration()); // Ethan: added this line
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);

        movement();
        sprint();
    }

    void movement()
    {
        if(controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        // moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // transform.position += moveDir * speed * Time.deltaTime

        moveDir = (transform.right * Input.GetAxis("Horizontal")) +
                  (transform.forward * Input.GetAxis("Vertical"));
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            playerVel = Vector3.zero;
        }

        if(Input.GetButton("Fire1") && !isShooting)
        {
            StartCoroutine(shoot());
        }
    }

    void jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
            
        }
    }

    void sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if(Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;

        // shoot code goes

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreMask))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if(dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }

        yield return new WaitForSeconds(shootRate);

        isShooting = false;


    }

    public void AddMana(int amount) // Ethan: added this function
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
        updatePlayerUI();
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashScreenDamage());
        if(HP <= 0)
        {
            //XP Death
            gamemanager.instance.youLose();
        }
    }


    IEnumerator manaRegeneration() // Ethan: added this function
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (canRegenMana && currentMana < maxMana)
            {
                currentMana = Mathf.Min(currentMana + Mathf.RoundToInt(manaRegenRate), maxMana);
                updatePlayerUI();
            }
           
        }
    }

    void UseMana(int amount) // Ethan: added this function
    {
        currentMana = Mathf.Min(currentMana + Mathf.RoundToInt(manaRegenRate), maxMana);
        updatePlayerUI();
    }

    IEnumerator flashScreenDamage()
    {
        gamemanager.instance.playerDamageScreen.SetActive(true); // Ethan: added this line
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDamageScreen.SetActive(false); // Ethan: added this line

    }

    public void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        gamemanager.instance.playerManaBar.fillAmount = (float)currentMana / maxMana; // Ethan: added this line
    }


}
