using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public void NewSave()
    {
        //loadingScreen.SetActive(true);
        //StartCoroutine(LoadAsyncScene());
        SceneManager.LoadScene(1);
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        while (!asyncLoad.isDone)
        {
            float progressValue = Mathf.Clamp01(asyncLoad.progress);
            yield return null;
        }
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
