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
    [SerializeField][Range(1,10)] int AOEDamageAmount;
    [SerializeField] AudioSource audPlayer;
    [SerializeField] AudioClip[] impactSound;
    [SerializeField][Range(0, 1)] float impactSoundVol;


    // Start is called before the first frame update
    void Start()
    {
       if(type == damageType.moving)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
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

        if(dmg != null)
        {
           // audPlayer.PlayOneShot(impactSound[Random.Range(0, impactSound.Length)], impactSoundVol);

            dmg.takeDamage(damageAmount);
            currentHits++;
            if(currentHits >= maxHits)
            {
                Destroy(gameObject);
            }
            else
            {
                Chain(other);

            }   
        }
        if (other.CompareTag("LevelObject") && type ==damageType.moving)
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
        Collider[] hitColliders = Physics.OverlapSphere (transform.position, AOETriggerRadius);

        foreach (var hitCollider in hitColliders)
        {
            IDamage dmg = hitCollider.GetComponent<IDamage>();
            if (dmg != null && hitCollider.gameObject.tag != "Player")
            {
                dmg.takeDamage(AOEDamageAmount);
            }
        }
    }
}
