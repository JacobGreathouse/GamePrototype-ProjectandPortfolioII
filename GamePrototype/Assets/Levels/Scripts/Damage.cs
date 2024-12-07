using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{

    enum damageType { moving, stationary}
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] float speed;
    [SerializeField] int destroyTime;

    bool hasDamaged;

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
        IDamage dmg = other.GetComponent<IDamage>();
         
        if(dmg != null )
        {
            dmg.takeDamage(damageAmount);
        }

        if(type == damageType.moving)
        {
            Destroy(gameObject);
        }
     
    }
}
