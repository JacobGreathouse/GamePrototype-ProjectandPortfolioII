using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Portal : MonoBehaviour
{
    // private properties ----------------------------------------------------

    Collider _teleportCollider;
    enum PORTALTYPE {SCENE_LOCAL, SCENE_TRANSITION}
    bool _transitionMutex = false;

    // Inspector serializables -------------------------------------------------

    [Header("~~~ Global ~~~")]
    [SerializeField] PORTALTYPE _type;
    [SerializeField] Camera _exitCam;
    //[SerializeField] Collider _renderTrigger;
    [SerializeField] GameObject _teleportTrigger;
    [SerializeField] GameObject _renderMesh;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _teleportSound;

    [Header("~~~ Scene Local ~~~")]
    [SerializeField] GameObject _exitNode;

    [Header("~~~ Scene Transition ~~~")]
    [SerializeField] [Range(0, 1000)] int _sceneIndex;
    [SerializeField] float _SceneTransitionDelay;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Image loadingCircle;

    // pubic properties ---------------------------------------------------------
    public GameObject exitNode => _exitNode;

    // Start is called before the first frame update
    void Start()
    {
        //_teleportCollider = GetComponent<Collider>();
        _exitCam.transform.SetPositionAndRotation(_exitNode.transform.position, _exitNode.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        _renderMesh.transform.LookAt(gamemanager.instance.player.transform);
    }

    public void Teleport()
    {
        _audioSource.PlayOneShot(_teleportSound);
        if (_type == PORTALTYPE.SCENE_LOCAL)
        {
            gamemanager.instance.playerScript.WarpPosition(_exitNode.transform.position, _exitNode.transform.rotation);
        }
        else
        {
            loadingScreen.SetActive(true);
            StartCoroutine(LoadAsyncScene());
            //StartCoroutine(TransitionDelay());
        }
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
    //IEnumerator TransitionDelay()
    //{

    //    yield return new WaitForSeconds(_SceneTransitionDelay);
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    //}

}
