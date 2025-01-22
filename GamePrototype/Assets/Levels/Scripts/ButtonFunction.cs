using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{
    public void resume()
    {
        gamemanager.instance.stateUnpause();
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamemanager.instance.stateUnpause();
    }
    public void Respawn()
    {
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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Options()
    {
        gamemanager.instance.optionsOpen();
    }    

    public void OptionsBack()
    {
        gamemanager.instance.optionsClose();
    }
    public void HPplus()
    {
        if (CheckSkillPoints())
        {
            gamemanager.instance.player.GetComponent<PlayerController>().SetPlayerHP(5);
            gamemanager.instance.player.GetComponent<PlayerController>().SetSkillPoints(-1);
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void MPplus()
    {
        if (CheckSkillPoints())
        {
            gamemanager.instance.player.GetComponent<PlayerController>().SetPlayerMP(15);
            gamemanager.instance.player.GetComponent<PlayerController>().SetSkillPoints(-1);
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void ATKplus()
    {
        if (CheckSkillPoints())
        {
            gamemanager.instance.player.GetComponent<PlayerController>().SetDamange(1);
            gamemanager.instance.player.GetComponent<PlayerController>().SetSkillPoints(-1);
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void Chainplus()
    {
        if (CheckSkillPoints() && gamemanager.instance.player.GetComponent<PlayerController>().GetChainMax() >= 6)
        {
            gamemanager.instance.player.GetComponent<PlayerController>().SetChainMax(1);
            gamemanager.instance.player.GetComponent<PlayerController>().SetSkillPoints(-1);
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void AOEplus()
    {
        if (CheckSkillPoints()&& gamemanager.instance.player.GetComponent<PlayerController>().GetAOERadius() >= 10)
        {
            gamemanager.instance.player.GetComponent<PlayerController>().SetAOERadius(1);
            gamemanager.instance.player.GetComponent<PlayerController>().SetSkillPoints(-1);
            gamemanager.instance.player.GetComponent<PlayerController>().updatePlayerUI();
        }
    }
    public void Missileplus()
    {
        if (CheckSkillPoints() && gamemanager.instance.player.GetComponent<PlayerController>().GetBurstAmount() >= 6)
        {
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
