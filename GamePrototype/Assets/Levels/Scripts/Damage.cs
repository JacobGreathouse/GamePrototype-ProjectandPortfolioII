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
    [SerializeField][Range(1, 30)] float triggerRadius;
    [SerializeField] bool isAOE = false;
    [SerializeField][Range(1,10)] float AOETriggerRadius;
    [SerializeField][Range(1,10)] int AOEDamageAmount;

    

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
         
        if(dmg != null)
        {
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
            //if (isAOE)
            //{
            //    AOEImpact();
            //}
            if(other.tag =="LevelObject")
                Destroy(gameObject);
           
        }
     
    }
    public void Chain(Collider previousEnemy)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, triggerRadius);
        bool foundEnemy = false;
        foreach (var hitCollider in hitColliders)
        {
            //check if collider is enemy and not enemy just hit
            if (hitCollider.gameObject != previousEnemy.gameObject && hitCollider.GetComponent<IDamage>() != null && hitCollider.gameObject.tag != "Player")
            {
                Vector3 direction = (hitCollider.transform.position - transform.position);
                direction.y = 0;
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
//    public void AOEImpact()
 //   {
//        Collider[] hitColliders = Physics.OverlapSphere(transform.position, AOETriggerRadius);
//        foreach (var hitCollider in hitColliders)
//        {
//            IDamage dmg = hitCollider.GetComponent<IDamage>();
//            if (dmg != null && hitCollider.gameObject.tag != "Player")
//            {
//                dmg.takeDamage(AOEDamageAmount);
//
//           }
//        }
//    }
}
