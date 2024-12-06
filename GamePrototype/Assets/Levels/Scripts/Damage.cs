using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] int damageAmount;
    // Start is called before the first frame update
    void Start()
    {
        
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
     
    }
}
