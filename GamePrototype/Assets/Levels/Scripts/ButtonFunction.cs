using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ButtonFunction : MonoBehaviour
{
    [SerializeField] GameObject firstScreen;
    [SerializeField] GameObject MainMenuScreen;
    [SerializeField] IntroAnimationScript _introAni;
    
    [Header("----- Audio -----")]
    [SerializeField] AudioSource audButton;
    [SerializeField] AudioClip[] audButtonClick;
    [SerializeField][Range(0, 1)] float audButtonVol;

    [Header("----- Buttons -----")]
    GameObject activeButton;
    [SerializeField] GameObject[] firstButton;
    

    private void Update()
    {
        if (firstScreen != null && firstScreen.activeSelf)
        {
            FirstScreenClick();
        }

        if(firstButton != null)
        {
            for (int i = 0; i < firstButton.Length && firstButton[i].activeSelf; i++)
            {
                if (activeButton == null)
                {
                    if (Input.GetKeyDown("down"))
                    {
                        if (firstButton != null)
                        {
                            activeButton = firstButton[i];
                            EventSystem.current.SetSelectedGameObject(firstButton[i], new BaseEventData(EventSystem.current));
                        }
                    }
                }

            }
        }
        
        
    }

    public void resume()
    {
        StartCoroutine(playButtonClick());
        gamemanager.instance.stateUnpause();
    }

    public void Reset()
    {
        StartCoroutine(playButtonClick());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamemanager.instance.stateUnpause();
    }
    public void Respawn()
    {
        StartCoroutine(playButtonClick());

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            RespawnSystem respawnSystem = FindObjectOfType<RespawnSystem>();
            if (respawnSystem != null)
            {
                respawnSystem.RespawnPlayer(player);
                //Debug.Log("Player respawned.");
            }
        }
        gamemanager.instance.stateUnpause();

    }

    public void NewSave()
    {
        StartCoroutine(playButtonClick());
        //SceneManager.LoadScene(1);

        _introAni.Play();
    }

    IEnumerator playButtonClick()
    {
        audButton.PlayOneShot(audButtonClick[Random.Range(0, audButtonClick.Length)], audButtonVol);
        yield return new WaitForSeconds(audButtonClick.Length);
        activeButton = null;
    }

    //public void ContinueSave()
    //{
    //    StartCoroutine(playButtonClick());

    //    //need to implement
    //}

    public void FirstScreenClick()
    {
        if(Input.anyKeyDown)
        {
            firstScreen.SetActive(false);
            MainMenuScreen.SetActive(true);
        }
    }

    public void Credits()
    {
        StartCoroutine(playButtonClick());
        //opens and closes relavent menus through Unity
    }

    public void Quit()
    {
        StartCoroutine(playButtonClick());
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Options()
    {
        StartCoroutine(playButtonClick());
        if (gamemanager.instance != null)
            gamemanager.instance.optionsOpen();
    }    

    public void OptionsBack()
    {
        StartCoroutine(playButtonClick());
        if (gamemanager.instance != null)
            gamemanager.instance.optionsClose();

    }

    public void HPplus()
    {
        if (CheckSkillPoints(1))
        {
            StartCoroutine(playButtonClick());

            gamemanager.instance.player.GetComponent<PlayerController>().PlayerHP = 5;
            gamemanager.instance.player.GetComponent<PlayerController>()._SkillPoints = -1;
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void MPplus()
    {
        if (CheckSkillPoints(1))
        {
            StartCoroutine(playButtonClick());

            gamemanager.instance.player.GetComponent<PlayerController>().PlayerMP=  15;
            gamemanager.instance.player.GetComponent<PlayerController>()._SkillPoints = -1;
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void ATKplus()
    {
        if (CheckSkillPoints(1))
        {
            StartCoroutine(playButtonClick());

            gamemanager.instance.player.GetComponent<PlayerController>().shootDamage = 1;
            gamemanager.instance.player.GetComponent<PlayerController>()._SkillPoints = -1;
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void Chainplus()
    {
        if (CheckSkillPoints(5) && gamemanager.instance.player.GetComponent<PlayerController>().ChainMax <= 5)
        {
            StartCoroutine(playButtonClick());

            gamemanager.instance.player.GetComponent<PlayerController>().ChainMax = 1;
            gamemanager.instance.player.GetComponent<PlayerController>()._SkillPoints = -5;
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void AOEplus()
    {
        if (CheckSkillPoints(5)&& gamemanager.instance.player.GetComponent<PlayerController>().AOERadius <= 9)
        {
            StartCoroutine(playButtonClick());

            gamemanager.instance.player.GetComponent<PlayerController>().AOERadius = 1;
            gamemanager.instance.player.GetComponent<PlayerController>()._SkillPoints = -5;
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void Missileplus()
    {
        if (CheckSkillPoints(5) && gamemanager.instance.player.GetComponent<PlayerController>().BurstAmount <= 5)
        {
            StartCoroutine(playButtonClick());

            gamemanager.instance.player.GetComponent<PlayerController>().BurstAmount = 1;
            gamemanager.instance.player.GetComponent<PlayerController>()._SkillPoints = -5;
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public bool CheckSkillPoints(int value)
    {

        if (gamemanager.instance.player.GetComponent<PlayerController>()._SkillPoints >= value)
        {
            return true;
        }
        return false;
    }

    public void ContinueButton()
    {
        gamemanager.instance.HowToPlayMenu.SetActive(false);
        gamemanager.instance.GameGoalMenu.SetActive(true);
    }

    public void ExitButton()
    {
        gamemanager.instance.GameGoalMenu.SetActive(false);
        gamemanager.instance.stateUnpause();
        gamemanager.instance.menuActive = null;
    }

    public void OpenLevelSelect()
    {
        gamemanager.instance.levelSelectOpen();
    }

    public void BuyHealthPotion()
    {
        if (gamemanager.instance.player.GetComponent<PlayerController>().GetCoinCount() >= 10)
        {
            gamemanager.instance.player.GetComponent<PlayerController>().setHealthPotion(1);
            gamemanager.instance.player.GetComponent<PlayerController>().SetCoinCount(-10);
        }
    }
}
