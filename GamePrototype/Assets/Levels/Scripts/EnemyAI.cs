using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] int HP;
    [SerializeField] float faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    private float roamTimer;
    [SerializeField] int animSpeedTrans;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [SerializeField] bool isMelee;
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeHitDistance;

    [SerializeField] LayerMask ignoreMask;
    [SerializeField] int xpOnKill;

    bool playerInRange;
    bool isShooting;
    bool isRoaming;

    Vector3 playerDir;
    Vector3 startingPos;

    Color colorOrig;

    float angleToPlayer;
    float stoppingDistOrig;

    Coroutine co;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeed = anim.GetFloat("Speed");
        anim.SetFloat("Speed", Mathf.MoveTowards(animSpeed, agentSpeed, Time.deltaTime * animSpeedTrans));

        if(playerInRange && !canSeePlayer())
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)
                co = StartCoroutine(roam());
        }
        else if (!playerInRange)
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)
                co = StartCoroutine(roam());
        }

    }

    IEnumerator roam()
    {
        isRoaming = true;
        roamTimer = Random.Range(1, 5);

        yield return new WaitForSeconds(roamTimer);

        agent.stoppingDistance = 0;

        Vector3 randomPos = Random.insideUnitSphere * roamDist;
        randomPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);

        isRoaming = false;
    }

    bool canSeePlayer()
    {
        playerDir = gamemanager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer < FOV)
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);
                agent.stoppingDistance = stoppingDistOrig;

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                }
                if (isMelee)
                {
                    if (!isShooting && agent.remainingDistance <= agent.stoppingDistance) //&& distance to player is stopping distance
                    {
                        StartCoroutine(meleeHit());
                    }
                }
                else
                {
                    if (!isShooting)
                    {
                        StartCoroutine(shoot());
                    }
                }
                return true;
            }
        }
        agent.stoppingDistance = 0;
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
            agent.stoppingDistance = 0;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if(agent.destination != gamemanager.instance.player.transform.position)
            agent.SetDestination(gamemanager.instance.player.transform.position);

        if(co != null)
            StopCoroutine(co);

        isRoaming = false;
        

        StartCoroutine(flashRed());

        if(HP <= 0)
        {
            // I'm dead
            gamemanager.instance.playerScript.SetPlayerXP(xpOnKill);
            Destroy(gameObject);
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");

        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    IEnumerator meleeHit()
    {
        isShooting = true;
        anim.SetTrigger("Melee");

        RaycastHit hit;
        if (Physics.Raycast(shootPos.position, playerDir, out hit, meleeHitDistance, ~ignoreMask))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(meleeDamage);
            }
        }

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
