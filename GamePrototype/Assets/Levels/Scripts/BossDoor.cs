using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    [SerializeField] GameObject door;

    /*[SerializeField] AudioSource audDoor;
    [SerializeField] AudioClip[] audDoorOpen;
    [SerializeField][Range(0, 1)] float audOpenVol;*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IOpen opn = other.GetComponent<IOpen>();
        if (opn != null && gamemanager.instance.GetOrbCount() <= 0)
        {
            door.SetActive(false);
            //audDoor.PlayOneShot(audDoorOpen[Random.Range(0, audDoorOpen.Length)], audOpenVol);
        }
        else if(opn != null && gamemanager.instance.GetOrbCount() > 0)
        {
            gamemanager.instance.allOrbsNotCol.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;

        IOpen opn = other.GetComponent<IOpen>();
        if (opn != null && gamemanager.instance.GetOrbCount() <= 0)
        {
            door.SetActive(true);
            //audDoor.PlayOneShot(audDoorOpen[Random.Range(0, audDoorOpen.Length)], audOpenVol);
        }
        else if (opn != null && gamemanager.instance.GetOrbCount() > 0)
        {
            gamemanager.instance.allOrbsNotCol.SetActive(false);
        }
    }
}
