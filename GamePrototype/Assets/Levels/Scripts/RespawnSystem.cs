using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    [SerializeField] Transform[] respawnPoints;
    [SerializeField] ParticleSystem spawnParticles;
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
        ParticleSystem particleInstance = Instantiate(spawnParticles, activeRespawnPoint.transform.position, Quaternion.identity);
        StartCoroutine(DestroyParticleSystemAfterDelay(particleInstance, 2f));
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
            ParticleSystem particleInstance =  Instantiate(spawnParticles, player.transform.position, Quaternion.identity);
            StartCoroutine(DestroyParticleSystemAfterDelay(particleInstance, 2f));
        }
        else
        {
            Debug.LogError("No active respawn point set!");  // Check if it's null
        }


    }
    private IEnumerator DestroyParticleSystemAfterDelay(ParticleSystem particleSystem, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (particleSystem != null)
        {
            particleSystem.Stop(); // Stop emitting new particles
            particleSystem.Clear(); // Clear existing particles
            Destroy(particleSystem.gameObject); // Destroy the particle system object
        }
    }
}
