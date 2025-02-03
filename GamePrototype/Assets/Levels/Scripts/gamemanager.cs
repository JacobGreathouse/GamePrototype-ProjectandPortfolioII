using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] int numOfOrbs;
    public GameObject allOrbsNotCol;
    [Header("----- Menu Options -----")]
    public GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin, menuLose;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject menuStats;
    [SerializeField] GameObject allOrbsCol;
    [SerializeField] GameObject _loadingScreen;
    [SerializeField] GameObject _menuDebugLevelSelect;
    [SerializeField] GameObject _spawnContainerPrefab;
    [SerializeField] GameObject _webGLLockStateScreen;

    public GameObject HowToPlayMenu;
    public GameObject GameGoalMenu;

    [Header("----- Player Stuff -----")]
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] TMP_Text PlayerLevel;
    public TMP_Text CurrentHPText;
    public TMP_Text CurrentMPText;
    public TMP_Text CurrentATKText;
    public TMP_Text CurrentAOERadiusText;
    public TMP_Text CurrentChainMaxText;
    public TMP_Text CurrentBurstText;
    public TMP_Text playerSPText;
    public TMP_Text CurrentCoinText;
    public TMP_Text CurrentPotionAmountStats;
    public TMP_Text CurrentPotionAmountHud;
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

    [Header("----- Coin Pickup Sounds -----")]
    [SerializeField] AudioSource audCoin;
    [SerializeField] AudioClip[] audPickupCoin;
    [SerializeField][Range(0, 1)] float audPickupCoinVol;



    public bool isPaused = false;
    int maxHits;
    float AOETriggerRadius;
    float timeScaleOrig;
    int orbCount;

    string MenuKey = "Cancel";

    LoadingScreen _loadingScreenScript;
    int _currentMapIndex = -1;
    bool _isLoading = false;

    public GameObject SpawnContainer;

    //[Header("----- Sensitivity Preset -----")]
    [SerializeField][Range(100, 600)] public int sensVal;

    // Start is called before the first frame update
    void Awake()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.stickyCursorLock = false;
        MenuKey = "Submit";
#endif
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        boss = GameObject.FindWithTag("Boss");
        playerScript = player.GetComponent<PlayerController>();
        _loadingScreenScript = _loadingScreen.GetComponent<LoadingScreen>(); 
        audPlayer.PlayOneShot(audAmbient[Random.Range(0, audAmbient.Length)], audAmbientVol);

        LoadMap(2);
    }

    // Update is called once per frame
    void Update()
    {
        //if (!_isLoading)
        //{
            //if (Input.GetButtonDown("Submit"))
            //{
                //_loadingScreen.SetActive(false);
                //gamemanager.instance.stateUnpause(true);
                //_isLoading = false;
                //GameObject SpawnPoint = GameObject.FindWithTag("SpawnPoint");
                //gamemanager.instance.playerScript.WarpPosition(SpawnPoint.transform.position, SpawnPoint.transform.rotation);
            //}
        //}
        //else 
        {
           
            if (Input.GetButtonDown(MenuKey))
            {
                if (menuActive == null && !_isLoading)
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
            if (Input.GetButtonDown("Stats") && !_isLoading)
            {
                if (menuActive == null)
                {
                    statePause();
                    menuActive = menuStats;
                    menuActive.SetActive(true);
                }
                else if (menuActive == menuStats)
                {
                    stateUnpause();
                }
            }
            if (!audPlayer.isPlaying)
            {
                audPlayer.PlayOneShot(audAmbient[Random.Range(0, audAmbient.Length)], audAmbientVol);
            }

            checkLockState();
        }
    }

    public void statePause()
    {

        playerScript.enabled = false;
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void stateUnpause(bool ignoreMenu = false)
    {

        playerScript.ResetSpeed();
        //playerScript.enabled = true;
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        if (!ignoreMenu && menuActive != null)
        { 
            menuActive.SetActive(false);
            menuActive = null;
        }
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

    public void sensitivityChanged(int value)
    {
        sensVal = value;
    }

    public void levelSelectOpen()
    {
        //menuActive = _menuDebugLevelSelect;
        _menuDebugLevelSelect.SetActive(true);
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

    public void pickupCoin()
    {
        audCoin.PlayOneShot(audPickupCoin[Random.Range(0, audPickupCoin.Length)], audPickupCoinVol);
    }
    public void PlayerHPUpdate()
    {
        CurrentHPText.text = playerScript.PlayerHP.ToString("F0");
    }
    public void PlayerMPUpdate()
    {
        CurrentMPText.text = playerScript.PlayerMP.ToString("F0");
    }
    public void PlayerATKUpdate()
    {
        CurrentATKText.text = playerScript.shootDamage.ToString("F0");
    }
    public void PlayerMissileUpdate()
    {
        CurrentBurstText.text = playerScript.BurstAmount.ToString("F0");
    }
    public void PlayerChainUpdate()
    {
        CurrentChainMaxText.text = playerScript.ChainMax.ToString("F0");
    }
    public void PlayerAOEUpdate()
    {
        CurrentAOERadiusText.text = playerScript.AOERadius.ToString("F0");
    }
    public void PlayerSPUpdate()
    {
        playerSPText.text = playerScript._SkillPoints.ToString("F0");
    }
    public void PlayerCoinUpdate()
    {
        CurrentCoinText.text = playerScript.GetCoinCount().ToString("F0");
    }

    public void PotionUpdate()
    {       
        CurrentPotionAmountHud.text = playerScript.GetHealthPotion().ToString("F0");
    }

    public void LoadMap(int index)
    {
        _loadingScreen.SetActive(true);
        gamemanager.instance.statePause();
        
        //_loadingScreenScript.LoadingText.SetActive(true);
        //_loadingScreenScript.ContinueText.SetActive(false);

        if(index == 3)
        {
            _loadingScreenScript.Level2Pan.SetActive(true);
            _loadingScreenScript.Level3Pan.SetActive(false);

        } else if (index == 4)
        {
            _loadingScreenScript.Level2Pan.SetActive(false);
            _loadingScreenScript.Level3Pan.SetActive(true);

        }
        else
        {
            _loadingScreenScript.Level2Pan.SetActive(false);
            _loadingScreenScript.Level3Pan.SetActive(false);
        }

        _isLoading = true;

        orbCount = 0;

        StartCoroutine(LoadMapAsync(index));

        //_isLoading = false;

        //_loadingScreenScript.LoadingText.SetActive(false);
        //_loadingScreenScript.ContinueText.SetActive(true);

        


    }

    public IEnumerator LoadMapAsync(int index)
    {
        yield return null;

        if (_currentMapIndex > 0)
        {
            AsyncOperation unloadProgress = SceneManager.UnloadSceneAsync(_currentMapIndex);

            while (!unloadProgress.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        if(SpawnContainer != null)
        {
            Destroy(SpawnContainer);
            SpawnContainer = null;
        }

        AsyncOperation loadProgress = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        while (!loadProgress.isDone)
        {

            yield return new WaitForEndOfFrame();
        }

        _loadingScreen.SetActive(false);
        gamemanager.instance.stateUnpause(true);

        SpawnContainer = Instantiate(_spawnContainerPrefab);

        _currentMapIndex = index;

        GameObject SP = GameObject.FindGameObjectWithTag("SpawnPoint");

        playerScript.WarpPosition(SP.transform.position, SP.transform.rotation);

        

        _isLoading = false;
    }

    private void checkLockState()
    {
        //Debug.Log(Cursor.lockState);
        if (Application.platform == RuntimePlatform.WebGLPlayer && !isPaused)
        {
            
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                _webGLLockStateScreen.SetActive(true);
            }
            else
            {
                _webGLLockStateScreen.SetActive(false);
            }
        }
    }
}
