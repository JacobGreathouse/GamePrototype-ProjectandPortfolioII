using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointTrigger : MonoBehaviour
{
    [SerializeField] Transform[] respawnPoints;
    [SerializeField] ParticleSystem spawnParticles;

    [Header("----- Audio -----")]
    [SerializeField] AudioSource audCheckPoint;
    [SerializeField] AudioClip[] audCheckPointHit;
    [SerializeField][Range(0, 1)] float audCheckVol;

    bool _isActive = false;

    private RespawnSystem respawnSystem;
    // Start is called before the first frame update
    void Start()
    {
        respawnSystem = FindObjectOfType<RespawnSystem>();
        if (respawnSystem == null)
        {
            //Debug.LogError("RespawnSystem not found in the scene!");
        }
    }
    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isActive)
        {
            _isActive = true;
            audCheckPoint.PlayOneShot(audCheckPointHit[Random.Range(0, audCheckPointHit.Length)], audCheckVol);

            respawnSystem.SetActiveRespawnPoint(transform);
            Debug.Log("Checkpoint reached: " + transform.position);
            gamemanager.instance.respawnButton.SetActive(true);
        }
    }

}


