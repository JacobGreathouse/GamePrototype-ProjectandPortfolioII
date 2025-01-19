using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Image loadingCircle;

    public void NewSave()
    {
        loadingScreen.SetActive(true);
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

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
