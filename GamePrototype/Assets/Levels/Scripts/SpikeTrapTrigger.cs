using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapTrigger : MonoBehaviour
{
    SpikeTrap _spikeTrapScript;

    [SerializeField] GameObject _spikeTrap;

    private void Start()
    {
        _spikeTrapScript = _spikeTrap.GetComponent<SpikeTrap>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other != null && other.CompareTag("Player"))
        {
            
            if ((!gamemanager.instance.playerScript.Controller.isGrounded) && gamemanager.instance.playerScript.VertMovement < -0.5f)
            {
                Debug.Log(gamemanager.instance.playerScript.VertMovement);
                _spikeTrapScript.Hurt();
            }
        }
    }
}
