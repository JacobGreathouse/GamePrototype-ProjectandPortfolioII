using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BossAI : MonoBehaviour, IDamage, IBoss
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] Image HPBar;

    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] GameObject SpawnPortal;
    [SerializeField] GameObject Spawner;
    [SerializeField] GameObject Bridge;

    [Header("----- Boss Stats -----")]
    [SerializeField] int HP;
    [SerializeField] float faceTargetSpeed;
    [SerializeField] float stoppingDist;
    [SerializeField] int xpOnKill;
    [SerializeField] int animSpeedTrans;
    [SerializeField] int bulletnumbermin;
    [SerializeField] int bulletnumbermax;

    [Header("----- TP Stats -----")]
    [SerializeField] bool canTeleport;
    [SerializeField] Transform[] tpPoints;
    [SerializeField] float tpdelay;
    bool isTeleporting;

    [Header("----- Attack Stats -----")]
    [SerializeField] GameObject[] spellObject;
    [SerializeField] float shootRatemin;
    [SerializeField] float shootRatemax;
    [SerializeField] bool canshootmagicmissle;

    [Header("----- Audio -----")]
    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip[] audTakeDamage;
    [SerializeField][Range(0, 1)] float audTakeDamVol;

    [Header("----- Lvl3 -----")]
    [SerializeField] bool isFinalBoss;
    [SerializeField] int magicMissleTimer;
    [SerializeField] GameObject magicMissle;


    bool isDead = false;
    bool magicMissleShot;
    bool playerInRange;
    bool isShooting;
    int HPOrig;

    Vector3 playerDir;

    float angleToPlayer;
    int randSpell;
    float tpdelayorig;

    Color colorOrig;
    Coroutine co;

    // Start is called before the first frame update
    void Start()
    {
        // colorOrig = model.material.color;
        HPOrig = HP;
        tpdelayorig = tpdelay;
        isTeleporting = false;
        updateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled && !isDead)
        {
            float agentSpeed = agent.velocity.normalized.magnitude;
            float animSpeed = anim.GetFloat("Speed");
            anim.SetFloat("Speed", Mathf.MoveTowards(animSpeed, agentSpeed, Time.deltaTime * animSpeedTrans));

            if (playerInRange && !canSeePlayer())
            {
                randSpell = Random.Range(0, 1);
            }
        }

    }

    bool canSeePlayer()
    {
        playerDir = gamemanager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (isDead)
        {
            agent.SetDestination(agent.transform.position);
        }
        if (Physics.Raycast(headPos.position, playerDir, out hit) && !isDead)
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                    if (!isShooting && gamemanager.instance.isPaused == false)
                    {
                        co = StartCoroutine(shoot());
                    }
                    if(!magicMissleShot && canshootmagicmissle)
                    {
                        StartCoroutine(magicMissleShoot());
                    }
                }
                else
                {
                    
                }
                if (canTeleport == true && isTeleporting == false && HP <= (HPOrig / .9f))
                {
                    StartCoroutine(teleport());
                }
                return true;
            }
        }

        

        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updateUI();
        // StartCoroutine(flashRed());
        audPlayer.PlayOneShot(audTakeDamage[Random.Range(0, audTakeDamage.Length)], audTakeDamVol);
        if (HP <= 0)
        {
            isDead = true;
            this.GetComponent<NavMeshAgent>().speed = 0;
            agent.GetComponent<Animator>().StopPlayback();


            if (Bridge != null)
            {
                Bridge.SetActive(true);
            }

            Destroy(Spawner);

            StartCoroutine(death());
        }
    }

    IEnumerator death()
    {
        anim.SetTrigger("Death");

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);

        if(isFinalBoss)
        {
            gamemanager.instance.youWin();
        }

        if (SpawnPortal != null)
        {
            /*Quaternion rot = Quaternion.LookRotation(new Vector3(transform.rotation.x, 0, transform.rotation.z));
            Vector3 pos = new Vector3(transform.position.x, 5.0f, transform.position.z);
            Instantiate(SpawnPortal, pos, rot).transform.SetParent(gamemanager.instance.SpawnContainer.transform);*/
            SpawnPortal.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z); ;
            SpawnPortal.GetComponent<Portal>().Spawn();
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;

        anim.SetTrigger("Shoot");

        int bulletnumber = Random.Range(bulletnumbermin, bulletnumbermax);

        int spellToUse = Random.Range(0, spellObject.Length);

        for (int i = 0; i < bulletnumber / 2; i++)
        {
            Quaternion rotat = Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y - 1, playerDir.z + i));
            shootPos.rotation = rotat;
            Instantiate(spellObject[spellToUse], shootPos.position, shootPos.rotation).transform.SetParent(gamemanager.instance.SpawnContainer.transform);

            rotat = Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y - 1, playerDir.z + (i + 2)));
            shootPos.rotation = rotat;
            Instantiate(spellObject[spellToUse], shootPos.position, shootPos.rotation).transform.SetParent(gamemanager.instance.SpawnContainer.transform);


        }
        
        float shootRate = Random.Range(shootRatemin, shootRatemax);

        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    IEnumerator magicMissleShoot()
    {
        magicMissleShot = true;

        yield return new WaitForSeconds(magicMissleTimer);

        Quaternion rotat = Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y - 1, playerDir.z));
        shootPos.rotation = rotat;
        Instantiate(magicMissle, shootPos.position, shootPos.rotation).transform.SetParent(gamemanager.instance.SpawnContainer.transform);

        magicMissleShot = false;
    }
    void updateUI()
    {
        HPBar.fillAmount = (float)HP / HPOrig;
    }
    /*IEnumerator flashRed()
     {
         model.material.color = Color.red;
         yield return new WaitForSeconds(0.3f);
         model.material.color = colorOrig;
     }*/

    IEnumerator teleport()
    {
        isTeleporting = true;

        Vector3 newPos = tpPoints[Random.Range(0, tpPoints.Length)].transform.position;
        
        if(newPos == transform.position)
        {
            transform.position = newPos;
        }
        else
        {
            tpdelay = 0;
        }

        yield return new WaitForSeconds(tpdelay);

        tpdelay = 5;
        isTeleporting = false;
    }
    public int GetHp()
    {
        return HP;
    }
    public int GetHpOrig()
    {
        return HPOrig;
    }
    
}
