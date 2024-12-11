using System.Collections;
//using UnityEditor.Build.Content;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage, IOpen
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
    [SerializeField] [Range(5, 15)]int lvlUpCost;

    [Header("----- Mana Stats -----")] // Ethan: added this line
    [SerializeField] [Range(50, 200)]int maxMana; // Ethan: added this line
    [SerializeField] [Range (.1f, 10)]float manaRegenRate; // Ethan: added this line
    [SerializeField] [Range (10, 50)] int specialAbilityManaCost = 20; // Ethan: added this line

    [Header("----- Gun Stats -----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject lightning;
    [SerializeField] int spellcost;

    [Header("----- XP Stats -----")] // Ethan: added this line
    [SerializeField] int playerXP; // Ethan: added this line
    [SerializeField] int playerLvl; // Ethan: added this line

    Vector3 moveDir;
    Vector3 playerVel;

    // int playerXP;
    // int playerLvl;
    int jumpCount;
    int HPOrig;
    int HPMax;
    float currentMana; // Ethan: added this line
    int currentHP; // Ethan: added this line
    

    bool isShooting;
    bool isSprinting;
    bool canRegenMana = true; // Ethan: added this line

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        HPMax = HP;
        currentMana = maxMana;
        updatePlayerUI(); // Ethan: added this line
        StartCoroutine(manaRegeneration()); // Ethan: added this line

        // Initialize XP UI to start at 0
        if(gamemanager.instance.playerXPBar != null) // Ethan: added this line
        {
            gamemanager.instance.playerXPBar.fillAmount = 0f; // Ethan: added this line
        }

        if(gamemanager.instance.playerXPText != null) // Ethan: added this line
        {
            gamemanager.instance.playerXPText.text = $"XP: 0/{lvlUpCost}"; // Ethan: added this line
        }

        if(gamemanager.instance.playerLevelText != null) // Ethan: added this line
        {
            gamemanager.instance.playerLevelText.text = $"Level: {playerLvl}"; // Ethan: added this line
        }
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

        Instantiate(lightning, shootPos.position, transform.rotation);
        UseMana(spellcost);

        /*RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreMask))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if(dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }*/

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
            gamemanager.instance.youLose();
        }
    }


    IEnumerator manaRegeneration() // Ethan: added this function
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            if (canRegenMana && currentMana < maxMana)
            {
                currentMana = Mathf.Min(currentMana + manaRegenRate, maxMana);
                updatePlayerUI();
            }
           
        }
    }

    void UseMana(int amount) // Ethan: added this function
    {
        currentMana = Mathf.Max(0, currentMana - amount);
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
        gamemanager.instance.playerXPBar.fillAmount = (float)playerXP / lvlUpCost;

        // Update Level text
        if (gamemanager.instance.playerLevelText != null) // Ethan: added this line
        {
            gamemanager.instance.playerLevelText.text = $"Level: {playerLvl}"; // Ethan: added this line
        }
        // Reset XP text
        if (gamemanager.instance.playerXPText != null) // Ethan: added this line
        {
            gamemanager.instance.playerXPText.text = $"XP: 0/{lvlUpCost}"; // Ethan: added this line
        }
    }

    private void updatePlayerLevel()
    {
        
        

        playerLvl += 1;
        playerXP = 0;
        lvlUpCost += 5;

        HPMax += 2;
        shootDamage += 1;
        maxMana += 15;
        manaRegenRate += .2f;


        HP = HPMax;
        currentMana = maxMana;

        updatePlayerUI();
    }

    public int GetPlayerXP()
    {
        return playerXP;
    }
    public void SetPlayerXP(int amount)
    {
        playerXP += amount;

        if (playerXP >= lvlUpCost)
            updatePlayerLevel();

        updatePlayerUI();
    }
}
