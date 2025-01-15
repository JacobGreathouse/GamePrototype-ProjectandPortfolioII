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
            _spikeTrapScript.Hurt();
        }
    }
}
