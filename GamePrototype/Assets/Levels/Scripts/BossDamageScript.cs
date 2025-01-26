using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamageScript : MonoBehaviour
{

    enum damageType { moving, stationary }
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
    [SerializeField][Range(0, 90)] float turnRate;
    [SerializeField][Range(0, 90)] float maxTurnAngle = 15f;
    [SerializeField][Range(1, 30)] float triggerRadius;
    [SerializeField][Range(1, 10)] float AOETriggerRadius;
    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip[] impactSound;
    [SerializeField][Range(0, 1)] float impactSoundVol;
    public int playerLvl;

    // Start is called before the first frame update
    void Start()
    {
        if (type == damageType.moving)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }
    private void Update()
    {
        if (isMissile == true)
        {
            Debug.Log("Firing missile...");
            StartCoroutine(HomingLogic()); //added homing logic for missile;
        }
    }
    //just type OnTriggerEnter to access what you need for
    //an effect to happen when you enter the trigger area

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (isAOE)
        {
            //audPlayer.PlayOneShot(impactSound[Random.Range(0, impactSound.Length)], impactSoundVol);

            AOEDamage();
            Destroy(gameObject);
            return;
        }

        if (dmg != null)
        {
            // audPlayer.PlayOneShot(impactSound[Random.Range(0, impactSound.Length)], impactSoundVol);

            dmg.takeDamage(damageAmount);
            currentHits++;
            if (currentHits >= maxHits)
            {
                Destroy(gameObject);
            }
            else
            {
                Chain(other);

            }
        }
        if (other.CompareTag("LevelObject") && type == damageType.moving)
            Destroy(gameObject);
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
                Vector3 direction = (hitCollider.transform.position - transform.position);
                direction.y = 0;
                transform.rotation = Quaternion.LookRotation(direction);
                rb.velocity = direction.normalized * speed;
                foundEnemy = true;
                break;
            }
        }
        if (!foundEnemy)
        {
            Destroy(gameObject);
        }

    }
    private void AOEDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, AOETriggerRadius);

        foreach (var hitCollider in hitColliders)
        {
            IDamage dmg = hitCollider.GetComponent<IDamage>();
            if (dmg != null && hitCollider.gameObject.tag != "Enemy")
            {
                dmg.takeDamage(damageAmount - 3);
            }
        }
    }

    private IEnumerator HomingLogic()
    {
        while (true)
        {
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
            if (hitCollider.GetComponent<IDamage>() != null && hitCollider.gameObject.tag != "Enemy")
            {
                // Cast a ray to check if the enemy is visible (i.e., not behind an obstacle)
                Vector3 directionToEnemy = hitCollider.transform.position - transform.position;
                //RaycastHit hit;

                // Perform the raycast and check if it hits an obstacle
                /*if (Physics.Raycast(transform.position, directionToEnemy, out hit, triggerRadius))
                {
                    // If the ray hits an obstacle and it's not the enemy, skip this enemy
                    if (hit.collider != hitCollider)
                    {
                        continue;
                    }
                }*/

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
