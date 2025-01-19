using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewSave()
    {
        SceneManager.LoadScene("Level1-Tutorial");
    }

    public void ContinueSave()
    {
        //need to implement
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

}
