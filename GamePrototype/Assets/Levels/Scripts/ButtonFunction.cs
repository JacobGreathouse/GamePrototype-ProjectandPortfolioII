using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{
    [Header("----- Audio -----")]
    [SerializeField] AudioSource audButton;
    [SerializeField] AudioClip[] audButtonClick;
    [SerializeField][Range(0, 1)] float audButtonVol;


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
                Debug.Log("Player respawned.");
            }
        }
        gamemanager.instance.stateUnpause();

    }

    public void NewSave()
    {
        StartCoroutine(playButtonClick());
        SceneManager.LoadScene(1);
    }

    IEnumerator playButtonClick()
    {
        audButton.PlayOneShot(audButtonClick[Random.Range(0, audButtonClick.Length)], audButtonVol);
        yield return new WaitForSeconds(audButtonClick.Length);
    }

    //public void ContinueSave()
    //{
    //    StartCoroutine(playButtonClick());

    //    //need to implement
    //}

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
        if (CheckSkillPoints())
        {
            StartCoroutine(playButtonClick());

            gamemanager.instance.player.GetComponent<PlayerController>().PlayerHP = 5;
            gamemanager.instance.player.GetComponent<PlayerController>()._SkillPoints = -1;
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void MPplus()
    {
        if (CheckSkillPoints())
        {
            StartCoroutine(playButtonClick());

            gamemanager.instance.player.GetComponent<PlayerController>().PlayerMP=  5;
            gamemanager.instance.player.GetComponent<PlayerController>()._SkillPoints = -1;
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void ATKplus()
    {
        if (CheckSkillPoints())
        {
            StartCoroutine(playButtonClick());

            gamemanager.instance.player.GetComponent<PlayerController>().shootDamage = 1;
            gamemanager.instance.player.GetComponent<PlayerController>()._SkillPoints = -1;
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void Chainplus()
    {
        if (CheckSkillPoints() && gamemanager.instance.player.GetComponent<PlayerController>().ChainMax <= 5)
        {
            StartCoroutine(playButtonClick());

            gamemanager.instance.player.GetComponent<PlayerController>().ChainMax = 1;
            gamemanager.instance.player.GetComponent<PlayerController>()._SkillPoints = -5;
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void AOEplus()
    {
        if (CheckSkillPoints()&& gamemanager.instance.player.GetComponent<PlayerController>().AOERadius <= 9)
        {
            StartCoroutine(playButtonClick());

            gamemanager.instance.player.GetComponent<PlayerController>().AOERadius = 1;
            gamemanager.instance.player.GetComponent<PlayerController>()._SkillPoints = -5;
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void Missileplus()
    {
        if (CheckSkillPoints() && gamemanager.instance.player.GetComponent<PlayerController>().BurstAmount <= 5)
        {
            StartCoroutine(playButtonClick());

            gamemanager.instance.player.GetComponent<PlayerController>().BurstAmount = 1;
            gamemanager.instance.player.GetComponent<PlayerController>()._SkillPoints = -5;
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public bool CheckSkillPoints()
    {
        if (gamemanager.instance.player.GetComponent<PlayerController>()._SkillPoints > 0)
        {
            return true;
        }
        return false;
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
