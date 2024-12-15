using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{

    [SerializeField] StaffStats staff;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gamemanager.instance.playerScript.getStaffStats(staff);
            Destroy(gameObject);
        }
    }
}
