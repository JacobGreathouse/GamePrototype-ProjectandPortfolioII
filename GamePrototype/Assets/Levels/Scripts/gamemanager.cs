using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Ethan: Added this line

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] int numOfOrbs;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin, menuLose;
    [SerializeField] GameObject allOrbsCol;
    public GameObject allOrbsNotCol;
    [SerializeField] TMP_Text goalCountText; // Ethan: Added this line
    public TMP_Text playerXPText; // Ethan: Added this line
    public TMP_Text playerLevelText; // Ethan: Added this line
    public Image playerHPBar; // Ethan: Added this line
    public Image playerManaBar; // Ethan: Added this line
    public Image playerXPBar; // Ethan: Added this line
    public GameObject playerDamageScreen; // Ethan: Added this line

    public GameObject player;
    public PlayerController playerScript;

    public bool isPaused = false;

    float timeScaleOrig;
    //writing as enemy boss count will be added later
    int orbCount;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
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
        }
    }
    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        menuActive.SetActive(false);
        menuActive = null;
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

  
}
