using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;


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
    [SerializeField] int HP;
    [SerializeField][Range(5, 15)] int lvlUpCost;

    [Header("----- Mana Stats -----")] // Ethan: added this line
    [SerializeField][Range(50, 200)] int maxMana; // Ethan: added this line
    [SerializeField][Range(.1f, 10)] float manaRegenRate; // Ethan: added this line

    [Header("----- Staff Stats -----")]
    [SerializeField] List<StaffStats> staffList = new List<StaffStats>();
    [SerializeField] GameObject staffModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject lightning;
    [SerializeField] GameObject fireball;
    [SerializeField] GameObject missile;
    [SerializeField] GameObject bunnyBomb;
    [SerializeField] int spellcost;
    [SerializeField] bool isBolt;
    [SerializeField] bool isFire;
    [SerializeField] bool isMissile;
    [SerializeField] bool isBunny;
    [SerializeField][Range(0.1f, 1f)] float burstDelay = 0.2f;
    [SerializeField] int burstCount = 3;

    [Header("----- XP Stats -----")] // Ethan: added this line
    [SerializeField] int playerXP; // Ethan: added this line
    public int playerLvl; // Ethan: added this line

    [Header("----- Player Sounds -----")]
    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip[] audJump;
    [SerializeField][Range(0, 1)] float audJumpVol;
    [SerializeField] AudioClip[] audDamage;
    [SerializeField][Range(0, 1)] float audDamageVol;
    [SerializeField] AudioClip[] audStep;
    [SerializeField][Range(0, 1)] float audStepVol;
    [SerializeField] AudioClip[] shootSound;
    [SerializeField][Range(0, 1)] float shootSoundVol;
    [SerializeField] AudioClip[] levelSound;
    [SerializeField][Range(0, 1)] float levelSoundVol;

    [Header("----- Dodge Stats -----")]
    [SerializeField][Range(0,10)] float dodgeDistance;
    [SerializeField][Range(0, 1)] float dodgeDuration;
    [SerializeField][Range(0, 5)] float dodgeCooldown;

    private bool isDodging = false;
    private bool isDodgeCooldown = false;
    private float dodgeCooldownTimer = 0f;
    private Vector3 dodgeDirection;

    Vector3 moveDir;
    Vector3 playerVel;

    // int playerXP;
    // int playerLvl;
    int HealthPotionCount;
    int SkillPoints;
    int coinCount;
    int jumpCount;
    int HPOrig;
    int HPMax;
    int staffListPos;
    float currentMana; // Ethan: added this line
    int currentHP; // Ethan: added this line


    bool isShooting;
    bool isSprinting;
    bool isPlayingStep;
    bool canRegenMana = true; // Ethan: added this line

    public CharacterController Controller => controller;
    public float VertMovement => playerVel.y;

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        HPMax = HP;
        currentMana = maxMana;
        updatePlayerUI(); // Ethan: added this line
        StartCoroutine(manaRegeneration()); // Ethan: added this line
        statMenuUpdate();

        // Initialize XP UI to start at 0
        if (gamemanager.instance.playerXPBar != null) // Ethan: added this line
        {
            gamemanager.instance.playerXPBar.fillAmount = 0f; // Ethan: added this line
        }

        if (gamemanager.instance.playerXPText != null) // Ethan: added this line
        {
            gamemanager.instance.playerXPText.text = $"XP: 0/{lvlUpCost}"; // Ethan: added this line
        }

        if (gamemanager.instance.playerLevelText != null) // Ethan: added this line
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
        selectStaff();
    }

    void movement()
    {
        if (isDodging) 
            return; // prevent movement logic if dodging

        if (controller.isGrounded)
        {
            if (moveDir.magnitude > 0.1f && !isPlayingStep && !isDodging)
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

        if ((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            playerVel = Vector3.zero;
        }

        if (Input.GetButton("Fire1") && !isShooting && gamemanager.instance.isPaused == false)
        {
            Debug.Log("Firing spell");
            StartCoroutine(shootMagic());


        }
        if (Input.GetButton("Dodge") && !isDodgeCooldown && !isDodging && controller.isGrounded)
        {
            StartCoroutine(Dodge());
        }
        if (isDodgeCooldown)
        {
            dodgeCooldownTimer -= Time.deltaTime;
            if (dodgeCooldownTimer <= 0)
            {
                isDodgeCooldown = false;
            }
        }
    }

    IEnumerator Dodge()
    {
        dodgeDirection = moveDir;
        isDodging = true;
        int origLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("DodgePhase");

        float dodgeTime = 0f;
        while (dodgeTime < dodgeDuration)
        {
            controller.Move(dodgeDirection * dodgeDistance * Time.deltaTime / dodgeDuration); // Move player quickly
            dodgeTime += Time.deltaTime;
            yield return null;
        }

        isDodging = false;
        gameObject.layer =origLayer;
        isDodgeCooldown = true;
        dodgeCooldownTimer = dodgeCooldown;
    }
    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
            audPlayer.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    IEnumerator shootMagic()
    {
        isShooting = true;
        if (isMissile==true)
        {
            Debug.Log("Firing missile");
           
           for (int i = 0; i < burstCount; i++)
           {
                audPlayer.PlayOneShot(shootSound[Random.Range(0, shootSound.Length)], shootSoundVol);
                //added to reduce redundancy 
                GameObject newProjectile = Instantiate(missile, shootPos.position, Camera.main.transform.rotation);
                newProjectile.GetComponent<Damage>().Fire();
                //move this up if we decide to only use tha mana once
                UseMana(spellcost);
                //delay time but still quickly
                yield return new WaitForSeconds(burstDelay);
           }
        }
        if (isBunny == true)
        {
            audPlayer.PlayOneShot(shootSound[Random.Range(0, shootSound.Length)], shootSoundVol);
            GameObject newProjectile = Instantiate(bunnyBomb, shootPos.position, Camera.main.transform.rotation);
            newProjectile.GetComponent<Damage>().Fire();
            UseMana(spellcost);
        }
        // shoot code goes
        if (isBolt == true)
        {
            // shoot code goes
            audPlayer.PlayOneShot(shootSound[Random.Range(0, shootSound.Length)], shootSoundVol);
            GameObject newProjectile = Instantiate(lightning, shootPos.position, Camera.main.transform.rotation);
            newProjectile.GetComponent<Damage>().Fire();
            UseMana(spellcost);
        }
        else if (isFire == true)
        {
            // shoot code goes
            audPlayer.PlayOneShot(shootSound[Random.Range(0, shootSound.Length)], shootSoundVol);
            GameObject newProjectile = Instantiate(fireball, shootPos.position, Camera.main.transform.rotation);
            newProjectile.GetComponent<Damage>().Fire();
            UseMana(spellcost);
        }

        //i wanted a redundency if the previous two were false

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
        HP = HP - (amount + (int)((float)playerLvl * 1.5f));
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
        gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPMax;
        gamemanager.instance.playerManaBar.fillAmount = (float)currentMana / maxMana; // Ethan: added this line
        gamemanager.instance.playerXPBar.fillAmount = (float)playerXP / lvlUpCost;
        gamemanager.instance.PlayerLevelUpdate();
    }
    public void statMenuUpdate()
    {
        gamemanager.instance.CurrentHPText.text = $"{currentHP}";
    }
    private void updatePlayerLevel()
    {
        playerLvl += 1;
        playerXP = 0;
        lvlUpCost += 5;
     


        SkillPoints += 5;

        manaRegenRate += .2f;


        SetHPMPFull();
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
        {
            updatePlayerLevel();
            audPlayer.PlayOneShot(levelSound[Random.Range(0, levelSound.Length)], levelSoundVol);
        }
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

        //shootDamage = staff.shootDamage;
        //shootDistance = staff.shootDistance;
        shootRate = staff.shootRate;
        spellcost = staff.spellcost;
        isBolt = staff.isBolt;
        isFire = staff.isFire;
        isBunny = staff.isBunny;
        isMissile = staff.isMissile;

        staffModel.GetComponent<MeshFilter>().sharedMesh = staff.model.GetComponent<MeshFilter>().sharedMesh;
        staffModel.GetComponent<MeshRenderer>().sharedMaterial = staff.model.GetComponent<MeshRenderer>().sharedMaterial;

        staffListPos = staffList.Count - 1;
    }
    void selectStaff()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && staffListPos < staffList.Count - 1)
        {
            staffListPos++;
            changeStaff();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && staffListPos > 0)
        {
            staffListPos--;
            changeStaff();
        }
    }
    void changeStaff()
    {
        //shootDamage = staffList[staffListPos].shootDamage;
        //shootDistance = staffList[staffListPos].shootDistance;
        shootRate = staffList[staffListPos].shootRate;
        spellcost = staffList[staffListPos].spellcost;
        isBolt = staffList[staffListPos].isBolt;
        isFire = staffList[staffListPos].isFire;
        isMissile = staffList[staffListPos].isMissile;
        isBunny = staffList[staffListPos].isBunny;
        AudioClip[] shootSound = staffList[staffListPos].shootSound;

        float shootSoundVol = staffList[staffListPos].shootSoundVol;

        staffModel.GetComponent<MeshFilter>().sharedMesh = staffList[staffListPos].model.GetComponent<MeshFilter>().sharedMesh;
        staffModel.GetComponent<MeshRenderer>().sharedMaterial = staffList[staffListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public int GetplayerLvl()
    {
        return playerLvl;
    }
    public void SetHPMPFull()
    {
        HP = HPMax;
        currentMana = maxMana;
        updatePlayerUI();
    }

    /// <summary>
    /// Warp the player to a different position on the map.
    /// </summary>
    /// <param name="Pos">Position to warp to.</param>
    public void WarpPosition(Vector3 Pos, Quaternion Rot)
    {
        // Character controllers can cause conflicts when updating the transform directly.
        controller.enabled = false; 

        transform.SetPositionAndRotation(Pos, Rot);

        controller.enabled = true;
    }

    public void setCoinCount(int amount)
    {
        coinCount += amount;
    }

    public int getCoinCount()
    {
        return coinCount;
    }

    public void setHealthPotion(int amount)
    {
        HealthPotionCount += amount;
    }

    public int getDamage()
    {
        return shootDamage;
    }
}
