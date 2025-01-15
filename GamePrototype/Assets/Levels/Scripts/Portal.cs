using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    enum PORTALTYPE {SCENE_LOCAL, SCENE_TRANSITION}


    [Header("~~~ Global ~~~")]
    [SerializeField] PORTALTYPE _type;
    [SerializeField] Camera _exitCam;
    //[SerializeField] Collider _renderTrigger;
    [SerializeField] Collider _teleportTrigger;

    [Header("~~~ Scene Local ~~~")]
    [SerializeField] GameObject _exitNode;

    [Header("~~~ Scene Transition ~~~")]
    [SerializeField] [Range(0, 1000)] int _sceneIndex;


    // Start is called before the first frame update
    void Start()
    {
        _exitCam.transform.SetPositionAndRotation(_exitNode.transform.position, _exitNode.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        //_exitCam.transform.position = Camera.main.transform.position;
        //_exitCam.transform.rotation = gamemanager.instance.transform.rotation;
    }
}
