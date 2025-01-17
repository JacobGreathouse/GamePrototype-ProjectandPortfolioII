using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] int numOfOrbs;
    GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin, menuLose;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject allOrbsCol;
    public GameObject allOrbsNotCol;
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] TMP_Text PlayerLevel;
    public TMP_Text playerXPText;
    public TMP_Text playerLevelText;
    public Image playerHPBar;
    public Image playerManaBar;
    public Image playerXPBar;
    public GameObject playerDamageScreen;
    public GameObject respawnButton;

    [Header("----- Audio -----")]
    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip[] audAmbient;
    [SerializeField][Range(0, 1)] float audAmbientVol;

    [Header("----- Player Access-----")]
    public GameObject player;
    public PlayerController playerScript;

    [Header("----- Boss Access-----")]
    public GameObject boss;
    public Image bossHPBar;

    [Header("----- Orb Pickup Sounds -----")]
    [SerializeField] AudioSource audOrb;
    [SerializeField] AudioClip[] audPickup;
    [SerializeField][Range(0, 1)] float audPickupVol;


    public bool isPaused = false;

    float timeScaleOrig;
    int orbCount;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        boss = GameObject.FindWithTag("Boss");
        audPlayer.PlayOneShot(audAmbient[Random.Range(0, audAmbient.Length)], audAmbientVol);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);

            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
            else if (menuActive == menuOptions)
            {
                optionsClose();
            }
        }
        if (!audPlayer.isPlaying)
            audPlayer.PlayOneShot(audAmbient[Random.Range(0, audAmbient.Length)], audAmbientVol);
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        menuActive.SetActive(false);
        menuActive = null;
        if (player.GetComponent<PlayerController>().enabled == false)
        {
            StartCoroutine(enablePlayer());
        }
    }
    public void updateOrbGoal(int amount)
    {
        orbCount += amount;
        goalCountText.text = orbCount.ToString("F0");
        if (orbCount <= 0)
        {
            StartCoroutine(showDisplayMessage());
        }

    }
    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }


    public void youWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void optionsOpen()
    {
        menuActive = menuOptions;
        menuPause.SetActive(false);
        menuOptions.SetActive(true);
    }

    public void optionsClose()
    {
        menuActive = menuPause;
        menuOptions.SetActive(false);
        menuPause.SetActive(true);
    }

    IEnumerator showDisplayMessage()
    {
        allOrbsCol.SetActive(true);

        yield return new WaitForSeconds(5.0f);

        allOrbsCol.SetActive(false);
    }

    public int GetOrbCount()
    {
        return orbCount;
    }

    public void PlayerLevelUpdate()
    {
        PlayerLevel.text = playerScript.GetplayerLvl().ToString("F0");
    }

    public IEnumerator enablePlayer()
    {

        yield return new WaitForSeconds(0.1f);
        
        player.GetComponent<PlayerController>().enabled = true;

    }

    public void pickup()
    {
        audOrb.PlayOneShot(audPickup[Random.Range(0, audPickup.Length)], audPickupVol);
    }
}
