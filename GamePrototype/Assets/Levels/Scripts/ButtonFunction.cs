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
        audButton.PlayOneShot(audButtonClick[Random.Range(0, audButtonClick.Length)], audButtonVol);
        gamemanager.instance.stateUnpause();
    }

    public void Reset()
    {
        audButton.PlayOneShot(audButtonClick[Random.Range(0, audButtonClick.Length)], audButtonVol);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamemanager.instance.stateUnpause();
    }
    public void Respawn()
    {
        audButton.PlayOneShot(audButtonClick[Random.Range(0, audButtonClick.Length)], audButtonVol);

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

    public void Quit()
    {
        audButton.PlayOneShot(audButtonClick[Random.Range(0, audButtonClick.Length)], audButtonVol);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Options()
    {
        audButton.PlayOneShot(audButtonClick[Random.Range(0, audButtonClick.Length)], audButtonVol);
        if (gamemanager.instance != null)
            gamemanager.instance.optionsOpen();
    }    

    public void OptionsBack()
    {
        audButton.PlayOneShot(audButtonClick[Random.Range(0, audButtonClick.Length)], audButtonVol);
        if (gamemanager.instance != null)
            gamemanager.instance.optionsClose();
    }

    public void HPplus()
    {
        if (CheckSkillPoints())
        {
            audButton.PlayOneShot(audButtonClick[Random.Range(0, audButtonClick.Length)], audButtonVol);

            gamemanager.instance.player.GetComponent<PlayerController>().SetPlayerHP(5);
            gamemanager.instance.player.GetComponent<PlayerController>().SetSkillPoints(-1);
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void MPplus()
    {
        if (CheckSkillPoints())
        {
            audButton.PlayOneShot(audButtonClick[Random.Range(0, audButtonClick.Length)], audButtonVol);

            gamemanager.instance.player.GetComponent<PlayerController>().SetPlayerMP(15);
            gamemanager.instance.player.GetComponent<PlayerController>().SetSkillPoints(-1);
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void ATKplus()
    {
        if (CheckSkillPoints())
        {
            audButton.PlayOneShot(audButtonClick[Random.Range(0, audButtonClick.Length)], audButtonVol);

            gamemanager.instance.player.GetComponent<PlayerController>().SetDamange(1);
            gamemanager.instance.player.GetComponent<PlayerController>().SetSkillPoints(-1);
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void Chainplus()
    {
        if (CheckSkillPoints() && gamemanager.instance.player.GetComponent<PlayerController>().GetChainMax() >= 6)
        {
            audButton.PlayOneShot(audButtonClick[Random.Range(0, audButtonClick.Length)], audButtonVol);

            gamemanager.instance.player.GetComponent<PlayerController>().SetChainMax(1);
            gamemanager.instance.player.GetComponent<PlayerController>().SetSkillPoints(-1);
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void AOEplus()
    {
        if (CheckSkillPoints()&& gamemanager.instance.player.GetComponent<PlayerController>().GetAOERadius() >= 10)
        {
            audButton.PlayOneShot(audButtonClick[Random.Range(0, audButtonClick.Length)], audButtonVol);

            gamemanager.instance.player.GetComponent<PlayerController>().SetAOERadius(1);
            gamemanager.instance.player.GetComponent<PlayerController>().SetSkillPoints(-1);
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void Missileplus()
    {
        if (CheckSkillPoints() && gamemanager.instance.player.GetComponent<PlayerController>().GetBurstAmount() >= 6)
        {
            audButton.PlayOneShot(audButtonClick[Random.Range(0, audButtonClick.Length)], audButtonVol);

            gamemanager.instance.player.GetComponent<PlayerController>().SetBurstAmount(1);
            gamemanager.instance.player.GetComponent<PlayerController>().SetSkillPoints(-1);
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public bool CheckSkillPoints()
    {
        if (gamemanager.instance.player.GetComponent<PlayerController>().GetSkillPoints() > 0)
        {
            return true;
        }
        return false;
    }


    public void OpenLevelSelect()
    {
        gamemanager.instance.levelSelectOpen();
    }

}
