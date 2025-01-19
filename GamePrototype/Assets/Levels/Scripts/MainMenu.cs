using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    string level1 = "Level1-Tutorial";

    [SerializeField] Image loadingCircle;

    public void NewSave()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level1);

        while (!asyncLoad.isDone)
        {
            float progressValue = Mathf.Clamp01(asyncLoad.progress);
            loadingCircle.fillAmount = progressValue;
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
