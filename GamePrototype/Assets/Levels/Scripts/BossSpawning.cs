using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawning : MonoBehaviour
{
    [Header("----- MobSpawn -----")]
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int spawnSpeed;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] int spawngroupSize;
    [SerializeField] ParticleSystem spawnParticles;

    Vector3 playerDir;

    bool startSpawning;
    bool isSpawning = false;
    int spawnCount;



    int BossHPOrig;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            if (startSpawning == true && !isSpawning && spawnCount < numToSpawn)
            { 
               
                   StartCoroutine(spawn());
                
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    IEnumerator spawn()
    {
        isSpawning = true;

        playerDir = gamemanager.instance.transform.position;
        for(int i = 0; i < spawngroupSize; i++)
        {
            int spawnInt = Random.Range(0, spawnPos.Length);
            Quaternion rotat = Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y, playerDir.z + 1.75f));
            ParticleSystem particleInstance = Instantiate(spawnParticles, spawnPos[spawnInt].transform.position, Quaternion.identity);
            StartCoroutine(DestroyParticleSystemAfterDelay(particleInstance, 2f));
            spawnCount += spawngroupSize;
            Instantiate(objectToSpawn, spawnPos[spawnInt].position, rotat);
        }
        

        yield return new WaitForSeconds(spawnSpeed);

        isSpawning = false;
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
