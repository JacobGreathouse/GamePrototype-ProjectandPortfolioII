using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    [SerializeField] Transform[] respawnPoints;

    private Transform activeRespawnPoint;


    void Start()
    {  //set the first checpoint as the main
        if(respawnPoints.Length > 0)
        {
            activeRespawnPoint = respawnPoints[0];
        }
        else
        {
            Debug.LogError("No respawn points set in the RespawnSystem.");
        }
    }
    public void SetActiveRespawnPoint(Transform newRespawnPoint)
    {
        activeRespawnPoint = newRespawnPoint;
        Debug.Log("Active respawn point set to: " + activeRespawnPoint.position);
    }
    public void RespawnPlayer(GameObject player)
    {
        if (activeRespawnPoint != null)
        {         
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController.enabled == true)
            {
                playerController.enabled = false;
            }
            player.transform.position = activeRespawnPoint.position;
            player.transform.rotation = activeRespawnPoint.rotation;
            Debug.Log("Player respawned at: " + activeRespawnPoint);


            if (playerController != null)
            {
                playerController.SetHPMPFull();
            }
        }
        else
        {
            Debug.LogError("No active respawn point set!");  // Check if it's null
        }


    }
}
