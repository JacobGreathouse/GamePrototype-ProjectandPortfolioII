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
    [SerializeField] GameObject spellObject;
    [SerializeField] float shootRate;


    bool playerInRange;
    bool isShooting;

    Vector3 playerDir;

    float angleToPlayer;

    Color colorOrig;


    // Start is called before the first frame update
    void Start()
    {
      //  colorOrig = model.material.color;

    }

    // Update is called once per frame
    void Update()
    {

        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeed = anim.GetFloat("Speed");
        anim.SetFloat("Speed", Mathf.MoveTowards(animSpeed, agentSpeed, Time.deltaTime * animSpeedTrans));

        if (playerInRange && !canSeePlayer())
        {
            
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
                    if (!isShooting)
                    {
                        StartCoroutine(shoot());
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

        if (agent.destination != gamemanager.instance.player.transform.position)
            agent.SetDestination(gamemanager.instance.player.transform.position);

        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            // I'm dead
            gamemanager.instance.playerScript.SetPlayerXP(xpOnKill);
            //add death animation here!, then wait for time of animation before destroying object
            //also want to stop him shooting

            Destroy(gameObject);
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");

        Instantiate(spellObject, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        model.material.color = colorOrig;
    }

}
