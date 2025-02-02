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
    bool _isSpawned = false;

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
    [SerializeField] Animator _animator;

    [Header("~~~ Scene Local ~~~")]
    [SerializeField] GameObject _exitNode;

    [Header("~~~ Scene Transition ~~~")]
    [SerializeField] [Range(0, 1000)] int _sceneIndex;
    [SerializeField] float _SceneTransitionDelay;

    // pubic properties ---------------------------------------------------------
    public GameObject exitNode => _exitNode;

    // Start is called before the first frame update
    void Start()
    {
        //_teleportCollider = GetComponent<Collider>();
        _exitCam.transform.SetPositionAndRotation(_exitNode.transform.position, _exitNode.transform.rotation);

        if(_type == PORTALTYPE.SCENE_TRANSITION)
        {
            if (!_isSpawned)
            {
                _animator.SetTrigger("Not_Spawned");
                _teleportTrigger.SetActive(false);
            }
            else
            {
                _animator.SetTrigger("Idle");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        _renderMesh.transform.LookAt(gamemanager.instance.player.transform);
    }

    public void Spawn()
    {
        _teleportTrigger.SetActive(true);
        _animator.SetTrigger("Spawn");
        _isSpawned = true;
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
            StartCoroutine(DelayedSceneTransition(_sceneIndex));
        }
    }

    IEnumerator DelayedSceneTransition(int index)
    {
        gamemanager.instance.statePause();
        yield return new WaitForSecondsRealtime(_SceneTransitionDelay);
        //yield return new WaitForSeconds(_SceneTransitionDelay);
        gamemanager.instance.LoadMap(index);
        
    }

}
