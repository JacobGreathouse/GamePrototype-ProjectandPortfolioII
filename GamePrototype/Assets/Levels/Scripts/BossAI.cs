using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] LayerMask ignoreMask;

    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [Header("----- Boss Stats -----")]
    [SerializeField] int HP;
    [SerializeField] float faceTargetSpeed;
    [SerializeField] float stoppingDist;
    [SerializeField] int xpOnKill;
    [SerializeField] int animSpeedTrans;


    [Header("----- Attack Stats -----")]
    [SerializeField] GameObject[] spellObject;
    [SerializeField] float shootRate;

    [Header("----- Audio -----")]
    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip[] audTakeDamage;
    [SerializeField][Range(0, 1)] float audTakeDamVol;

    
    
    

    bool playerInRange;
    bool isShooting;
    int HPOrig;

    Vector3 playerDir;

    float angleToPlayer;
    int randSpell;

    Color colorOrig;
    Coroutine co;

    // Start is called before the first frame update
    void Start()
    {
        // colorOrig = model.material.color;
        HPOrig = HP;
        updateUI();
    }

    // Update is called once per frame
    void Update()
    {

        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeed = anim.GetFloat("Speed");
        anim.SetFloat("Speed", Mathf.MoveTowards(animSpeed, agentSpeed, Time.deltaTime * animSpeedTrans));

        if (playerInRange && !canSeePlayer())
        {
            randSpell = Random.Range(0, 1);
        }
    }

    bool canSeePlayer()
    {
        playerDir = gamemanager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
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
                }
                else
                {
                    
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
            gamemanager.instance.youWin();
            Destroy(gameObject);
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");

        Quaternion rotat = Quaternion.LookRotation(new Vector3(playerDir.x, playerDir.y - 1, playerDir.z + 1.75f));

        shootPos.rotation = rotat;

        Instantiate(spellObject[Random.Range(0, spellObject.Length)], shootPos.position, shootPos.rotation);
                
        

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    void updateUI()
    {
        gamemanager.instance.bossHPBar.fillAmount = (float)HP / HPOrig;

    }
    /*IEnumerator flashRed()
     {
         model.material.color = Color.red;
         yield return new WaitForSeconds(0.3f);
         model.material.color = colorOrig;
     }*/
    public int GetHp()
    {
        return HP;
    }
    public int GetHpOrig()
    {
        return HPOrig;
    }
}
