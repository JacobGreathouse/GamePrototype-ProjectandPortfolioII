using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage, IOpen
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] int HP;
    [SerializeField] float faceTargetSpeed;
    [SerializeField] int FOV;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [SerializeField] bool isMelee;
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeHitDistance;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] int xpOnKill;

    bool playerInRange;
    bool isShooting;

    Vector3 playerDir;

    Color colorOrig;

    float angleToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && canSeePlayer())
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
            if (hit.collider.CompareTag("Player") && angleToPlayer < FOV)
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                }
                if (isMelee)
                {
                    if (!isShooting)
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
        StartCoroutine(flashRed());

        if(HP <= 0)
        {
            // I'm dead
            gamemanager.instance.updateGameGoal(-1);
            gamemanager.instance.playerScript.SetPlayerXP(xpOnKill);
            Destroy(gameObject);
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    IEnumerator meleeHit()
    {
        isShooting = true;

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
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
