using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointTrigger : MonoBehaviour
{
    private RespawnSystem respawnSystem;
    // Start is called before the first frame update
    void Start()
    {
        respawnSystem = FindObjectOfType<RespawnSystem>();
        if (respawnSystem == null)
        {
            Debug.LogError("RespawnSystem not found in the scene!");
        }
    }
    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            respawnSystem.SetActiveRespawnPoint(transform);
            Debug.Log("Checkpoint reached: " + transform.position);
        }
    }

}


