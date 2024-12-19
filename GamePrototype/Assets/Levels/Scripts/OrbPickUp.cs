using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbPickUp : MonoBehaviour
{
    void Start()
    {
        gamemanager.instance.updateOrbGoal(1);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Lower orb count by 1 so goal changes to 1 less orb to collect
            gamemanager.instance.updateOrbGoal(-1);

            Destroy(gameObject);
        }
    }
}
