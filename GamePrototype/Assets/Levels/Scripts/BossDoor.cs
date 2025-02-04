using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    [SerializeField] GameObject door;

    bool doorOpen = false;

    [SerializeField] AudioSource audDoor;
    [SerializeField] AudioClip[] audDoorOpen;
    [SerializeField][Range(0, 1)] float audOpenVol;



    [SerializeField] Animator _animator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IOpen opn = other.GetComponent<IOpen>();
        if (opn != null && gamemanager.instance.GetOrbCount() <= 10)
        {
            doorOpen = true;
            _animator.SetBool("isOpen", true);
            //door.SetActive(false);
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
        if (opn != null && doorOpen == true)
        {
            _animator.SetBool("isOpen", false);
            //door.SetActive(false);
            
            //audDoor.PlayOneShot(audDoorOpen[Random.Range(0, audDoorOpen.Length)], audOpenVol);
        }
        else if (opn != null)
        {
            StartCoroutine(message());
        }
    }

    public void playDoorSound()
    {
        
        audDoor.PlayOneShot(audDoorOpen[0], audOpenVol);
    }

    public void playRumbleSound()
    {
        
        audDoor.PlayOneShot(audDoorOpen[1]);
    }

    IEnumerator message()
    {
        gamemanager.instance.allOrbsNotCol.SetActive(true);

        yield return new WaitForSeconds(3);

        gamemanager.instance.allOrbsNotCol.SetActive(false);
    }
    
}
