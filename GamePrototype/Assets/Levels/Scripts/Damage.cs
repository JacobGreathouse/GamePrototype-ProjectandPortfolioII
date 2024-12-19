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
    [SerializeField][Range(1, 30)] float triggerRadius;
    [SerializeField][Range(1,10)] float AOETriggerRadius;
    [SerializeField] ParticleSystem hitEffect;
    [Header("----- Damage Sounds -----")]

    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip[] impactSound;
    [SerializeField][Range(0, 1)] float impactSoundVol;
    public int playerLvl;

    // Start is called before the first frame update
    void Start()
    {
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        damageAmount = damageAmount + (playerLvl * 2);
        IDamage dmg = other.GetComponent<IDamage>();
         


        if (dmg != null)
        {
            // audPlayer.PlayOneShot(impactSound[Random.Range(0, impactSound.Length)], impactSoundVol);

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
                    Chain(other);

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

                        dmg.takeDamage(damageAmount);

                
            }
        }
    }

}
