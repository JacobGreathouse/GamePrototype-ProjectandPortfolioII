using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    // Private properties ----------------------------------------------------

    Portal _portalScript;

    // Inspector serializables -------------------------------------------------

    [SerializeField] GameObject _parentPortal;

    private void Start()
    {
        _portalScript = _parentPortal.GetComponent<Portal>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other != null && other.CompareTag("Player"))
        {
            _portalScript.Teleport();
        }
    }
}
