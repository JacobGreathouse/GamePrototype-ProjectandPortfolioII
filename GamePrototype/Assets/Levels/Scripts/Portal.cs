using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] Camera _exitCam;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _exitCam.transform.position = Camera.main.transform.position;
        _exitCam.transform.rotation = gamemanager.instance.transform.rotation;
    }
}
