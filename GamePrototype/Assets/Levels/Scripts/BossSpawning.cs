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
            spawnCount += spawngroupSize;
            Instantiate(objectToSpawn, spawnPos[spawnInt].position, rotat);
        }
        

        yield return new WaitForSeconds(spawnSpeed);

        isSpawning = false;
    }
}
