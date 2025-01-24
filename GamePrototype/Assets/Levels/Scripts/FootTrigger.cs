using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootTrigger : MonoBehaviour
{
    bool _isColliding;

    public bool isColliding => _isColliding;

    GameObject _floor;

    public GameObject floor => _floor;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            _isColliding = true;
            _floor = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            _isColliding = false;
            _floor = null;
        }
    }

}
