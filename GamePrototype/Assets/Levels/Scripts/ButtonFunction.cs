using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Ethan: Added this line(technically Unity itself did)

public class ButtonFunction : MonoBehaviour
{
    public void resume()
    {
        gamemanager.instance.stateUnpause();
    }

    public void Reset() // Ethan: Added this function
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

    public void Quit() // Ethan: Added this function
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
