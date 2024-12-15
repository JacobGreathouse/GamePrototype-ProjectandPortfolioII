using System.Collections;
using System.Collections.Generic;

//using UnityEditor.Build.Content;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage, IOpen
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    [Header("----- Stats -----")]
    [SerializeField][Range(1, 5)] float speed;
    [SerializeField][Range(1, 4)] float sprintMod;
    [SerializeField][Range(1, 3)] int jumpMax;
    [SerializeField][Range(5, 20)] int jumpSpeed;
    [SerializeField][Range(15, 40)] int gravity;
    [SerializeField][Range(10, 30)] int HP;
    [SerializeField][Range(5, 15)] int lvlUpCost;

    [Header("----- Mana Stats -----")] // Ethan: added this line
    [SerializeField][Range(50, 200)] int maxMana; // Ethan: added this line
    [SerializeField][Range(.1f, 10)] float manaRegenRate; // Ethan: added this line

    [Header("----- Staff Stats -----")]
    [SerializeField] List <StaffStats> staffList = new List<StaffStats>();
    [SerializeField] GameObject staffModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject lightning;
    [SerializeField] GameObject fireball;
    [SerializeField] int spellcost;
    [SerializeField] bool isBolt;
    [SerializeField] bool isFire;

    [Header("----- XP Stats -----")] // Ethan: added this line
    [SerializeField] int playerXP; // Ethan: added this line
    [SerializeField] int playerLvl; // Ethan: added this line

    [Header("----- Player Sounds -----")]
    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip[] audJump;
    [SerializeField][Range(0, 1)] float audJumpVol;
    [SerializeField] AudioClip[] audDamage;
    [SerializeField][Range(0, 1)] float audDamageVol;
    [SerializeField] AudioClip[] audStep;
    [SerializeField][Range(0, 1)] float audStepVol;

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
    bool isPlayingStep;
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
            if (moveDir.magnitude > 0.1f && !isPlayingStep)
            {
                StartCoroutine(playStep());
            }

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
            StartCoroutine(shootMagic());
        }
    }

    void jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
            audPlayer.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
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

    IEnumerator shootMagic()
    {
        isShooting = true;
        // shoot code goes
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance, ~ignoreMask))
        {
            {
                if (isBolt == true)
                {
                    // shoot code goes

                    Instantiate(lightning, shootPos.position, Camera.main.transform.rotation);
                    UseMana(spellcost);
                }
                else if (isFire == true)
                {
                    // shoot code goes

                    Instantiate(fireball, shootPos.position, Camera.main.transform.rotation);
                    UseMana(spellcost);
                }
            }
            //i wanted a redundency if the previous two were false
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
        audPlayer.PlayOneShot(audDamage[Random.Range(0, audDamage.Length)], audDamageVol);
        if (HP <= 0)
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
    IEnumerator playStep()
    {
        isPlayingStep = true;

        audPlayer.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);

        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        if (isSprinting)
            yield return new WaitForSeconds(0.25f);

        isPlayingStep = false;

    }
    public void getStaffStats(StaffStats staff)
    {
        staffList.Add(staff);

        shootDamage = staff.shootDamage;
        shootDistance = staff.shootDistance;
        shootRate = staff.shootRate;
        spellcost = staff.spellcost;
        isBolt = staff.isBolt;
        isFire = staff.isFire;

        staffModel.GetComponent<MeshFilter>().sharedMesh = staff.model.GetComponent<MeshFilter>().sharedMesh;
        staffModel.GetComponent<MeshRenderer>().sharedMaterial = staff.model.GetComponent<MeshRenderer>().sharedMaterial;
    }
}
