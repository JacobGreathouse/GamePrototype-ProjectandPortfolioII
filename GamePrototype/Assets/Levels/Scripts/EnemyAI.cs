using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
// using static UnityEditor.PlayerSettings;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] float DeathTimer;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Image HPBar;

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
    

    [Header("----- Hurt Sounds -----")]
    [SerializeField] AudioSource audEnemy;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField][Range(0, 1)] float audHurtVol;

    [Header("----- Death Sounds -----")]
    [SerializeField] AudioClip[] audDying;
    [SerializeField][Range(0, 1)] float audDeadVol;
    [Header("------------------------")]
    [SerializeField] GameObject coinDrop;
    [SerializeField] bool canDropCoin;

    [SerializeField] ParticleSystem hitEffect;

    bool playerInRange;
    bool isDead = false;
    bool isShooting;
    bool isRoaming;
    int HpOrig;

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
        HpOrig = HP;
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
                if (!isRoaming && agent.remainingDistance < 0.01f && !isDead)
                    co = StartCoroutine(roam());
            }
            else if (!playerInRange && !isDead)
            {
                if (!isRoaming && agent.remainingDistance < 0.01f)
                    co = StartCoroutine(roam());
            }
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
        if (isDead)
        {
            agent.SetDestination(agent.transform.position);
        }
        if (Physics.Raycast(headPos.position, playerDir, out hit) && !isDead)
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer < FOV && !isDead)
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);
                agent.stoppingDistance = stoppingDistOrig;

                if (agent.remainingDistance < agent.stoppingDistance && !isDead)
                {
                    faceTarget();
                }
                if (isMelee && !isDead)
                {
                    if (!isShooting && agent.remainingDistance <= agent.stoppingDistance && co == null) //&& distance to player is stopping distance
                    {
                        float remaining = agent.remainingDistance;
                        float stopping = agent.stoppingDistance;
                        if (remaining < stopping)
                            StartCoroutine(meleeHit());
                    }
                    co = null;
                }
                else
                {
                    if (!isShooting && !isDead)
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
        if (agent != null && agent.isActiveAndEnabled)
        {
            if (agent.destination != gamemanager.instance.player.transform.position)
                agent.SetDestination(gamemanager.instance.player.transform.position);
        }

        if(co != null)
            StopCoroutine(co);

        isRoaming = false;

        audEnemy.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        //StartCoroutine(flashRed());
        updateUI();

        if(HP <= 0)
        {
            isDead = true;
            this.GetComponent<NavMeshAgent>().speed = 0;
            agent.GetComponent<Animator>().StopPlayback();

            StartCoroutine(death());

        }
    }

    IEnumerator death()
    {

        if (hitEffect != null)
        {
            if (audDying != null)
                audEnemy.PlayOneShot(audDying[Random.Range(0, audDying.Length)],audDeadVol);
            ParticleSystem particleInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
            particleInstance.Play();
            Destroy(particleInstance.gameObject, particleInstance.main.duration);
            model.enabled = false;
        }

        agent.GetComponent<CapsuleCollider>().enabled = false;
        anim.SetTrigger("Death");
        
        yield return new WaitForSeconds(DeathTimer);

        Destroy(gameObject);

        gamemanager.instance.playerScript.SetPlayerXP(xpOnKill);

        Quaternion rot = Quaternion.LookRotation(new Vector3(transform.rotation.x, -90, transform.rotation.z));
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);

        if (canDropCoin)
            Instantiate(coinDrop, pos, rot).transform.SetParent(gamemanager.instance.SpawnContainer.transform);

       

    }

    IEnumerator shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");

        Instantiate(bullet, shootPos.position, transform.rotation).transform.SetParent(gamemanager.instance.SpawnContainer.transform);
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
    void updateUI()
    {
        HPBar.fillAmount = (float)HP / HpOrig;
    }
}
