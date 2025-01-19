using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    int _RootSceneIndex;
    int _LoadedMap = -1;

    // Start is called before the first frame update
    void Start()
    {
        _RootSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMap(int index)
    {

        if(_LoadedMap > 0)
        {
            SceneManager.UnloadSceneAsync(_LoadedMap);
        }

        SceneManager.LoadScene(index, LoadSceneMode.Additive);
        _LoadedMap = index;
     
    }
}
