using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Damage : MonoBehaviour
{

    enum damageType { moving, stationary}
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] float speed;
    [SerializeField] int destroyTime;
    //maximum number of enemies the projectile can hi
    [SerializeField] int maxHits;
    private int currentHits = 0;
    //radius of sphere col trig
    [SerializeField] bool isAOE;
    [SerializeField] bool isMissile;
    [SerializeField] bool isBunny;
    [SerializeField][Range(0,90)] float turnRate;
    [SerializeField][Range(0, 90)] float maxTurnAngle = 15f;
    [SerializeField][Range(1, 30)] float triggerRadius;
    [SerializeField][Range(1,10)] float AOETriggerRadius;
    [SerializeField] ParticleSystem hitEffect;
    [Header("----- Damage Sounds -----")]

    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip[] impactSound;
    [SerializeField][Range(0, 1)] float impactSoundVol;
    public int playerLvl;
    public StaffStats staffStats;

    [Header("----- Bunny -----")]
    [SerializeField] GameObject bunnyModel;

    int damageamountOG;

    // Start is called before the first frame update
     void Start()
     {
        damageamountOG = damageAmount;
        if(type == damageType.moving)
        {
             rb.velocity = transform.forward * speed;
             Destroy(gameObject, destroyTime);
        }
     }

    private void Awake()
    {
        playerLvl = GameObject.FindWithTag("Player").GetComponent<PlayerController>().playerLvl;
    }
    //just type OnTriggerEnter to access what you need for
    //an effect to happen when you enter the trigger area
    
    //was told this would reduce redundancies.
    public void Fire()
    {
        if (type == damageType.moving)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
        if (isMissile == true)
        {
            Debug.Log("Firing missile...");
            StartCoroutine(HomingLogic()); //added homing logic for missile;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        if (isBunny && other.CompareTag("Enemy"))
        {
            // Destroy the enemy object
            Destroy(other.gameObject);

            // Instantiate the Bunny model at the enemy's position
            if (bunnyModel != null)
            {
                Instantiate(bunnyModel, other.transform.position, Quaternion.identity);
            }

            // Do not deal damage if it's a BunnyBomb
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>(); 


        if (dmg != null)
        {
            // audPlayer.PlayOneShot(impactSound[Random.Range(0, impactSound.Length)], impactSoundVol);
            if (other.tag != "Player")
                damageAmount = gamemanager.instance.player.GetComponent<PlayerController>().getDamage();

            dmg.takeDamage(damageAmount);
            if (hitEffect != null)
            { 
            ParticleSystem particleInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
            particleInstance.Play();
            Destroy(particleInstance.gameObject, particleInstance.main.duration);
            }
            if (type == damageType.moving)
            {
                currentHits++;
                if (currentHits >= maxHits)
                {
                    if (type == damageType.moving)
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    damageAmount -= 1;
                    if (damageAmount > 0)
                    {
                        Chain(other);
                    }

                }
            }
        }
        if (isAOE)
        {
            AOEDamage(other);
        }
        if (other.CompareTag("LevelObject") && type == damageType.moving)
        {
            if (hitEffect != null)
            {
                ParticleSystem particleInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
                particleInstance.Play();
                Destroy(particleInstance.gameObject, particleInstance.main.duration);
            }
            AOEDamage(other);
            Destroy(gameObject);
        }
    }
    public void Chain(Collider previousEnemy)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, triggerRadius);
        bool foundEnemy = false;
        if (currentHits >= maxHits)
        {
            Destroy(gameObject);
        }
        foreach (var hitCollider in hitColliders)
        {
            //check if collider is enemy and not enemy just hit
            if (hitCollider.gameObject != previousEnemy.gameObject && hitCollider.GetComponent<IDamage>() != null && hitCollider.gameObject.tag != "Player")
            {
                Vector3 targetPosition = hitCollider.transform.position + Vector3.up * (hitCollider.GetComponent<Collider>().bounds.extents.y);
                Vector3 direction = targetPosition - transform.position;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, direction, out hit, triggerRadius))
                {
                    if(hit.collider != hit.collider)
                    {
                        continue;
                    }
                }
                direction.Normalize();
                transform.rotation = Quaternion.LookRotation(direction);
                rb.velocity = direction * speed;
                foundEnemy = true;
                break;
            }
        }
        if (!foundEnemy)
        {
            Destroy(gameObject);
        }

    }
    private void AOEDamage(Collider directHit)
    {
        if (hitEffect != null)
        {
        ParticleSystem particleInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
        particleInstance.Play();
        Destroy(particleInstance.gameObject, particleInstance.main.duration);
        }
        Collider[] hitColliders = Physics.OverlapSphere (transform.position, AOETriggerRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider == directHit)
                continue;
            IDamage dmg = hitCollider.GetComponent<IDamage>();
            if (dmg != null && hitCollider.gameObject.tag != "Player")
            {
     
                    // Ensure the raycast hits the intended target

                        dmg.takeDamage(damageAmount -= 2);

                
            }
        }
    }
    private IEnumerator HomingLogic()
    {
        while (true)
        {
            Debug.Log("Homing logic is running...");
            HomeToNearestEnemy();
            yield return new WaitForSeconds(0.05f);
        }
    }
    private void HomeToNearestEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, triggerRadius);
        Transform nearestEnemy = null;
        float shortestDistance = float.MaxValue;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<IDamage>() != null && hitCollider.gameObject.tag != "Player")
            {
                // Cast a ray to check if the enemy is visible (i.e., not behind an obstacle)
                Vector3 directionToEnemy = hitCollider.transform.position - transform.position;
                RaycastHit hit;

                // Perform the raycast and check if it hits an obstacle
                if (Physics.Raycast(transform.position, directionToEnemy, out hit, triggerRadius))
                {
                    // If the ray hits an obstacle and it's not the enemy, skip this enemy
                    if (hit.collider != hitCollider)
                    {
                        continue;
                    }
                }

                // Calculate the distance to the enemy
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = hitCollider.transform;
                }
            }
        }

        if (nearestEnemy != null)
        {
            // Move towards the nearest enemy
            Vector3 targetPosition = nearestEnemy.position + Vector3.up * (nearestEnemy.GetComponent<Collider>().bounds.extents.y);
            Vector3 direction = targetPosition - transform.position;
            direction.Normalize();
            // gradually rotates so we can increase this later
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            if (angle > maxTurnAngle)
            {
                rb.velocity = transform.forward * speed;
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnRate * Time.deltaTime);
                rb.velocity = direction * speed;
            }
        }
        else
        {
            Debug.Log("No enemies in range for homing.");
        }
    }
}
