using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    [SerializeField] GameObject door;

    [SerializeField] AudioSource audDoor;
    [SerializeField] AudioClip[] audDoorOpen;
    [SerializeField][Range(0, 1)] float audOpenVol;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IOpen opn = other.GetComponent<IOpen>();
        if (opn != null)
        {
            door.SetActive(false);
            audDoor.PlayOneShot(audDoorOpen[Random.Range(0, audDoorOpen.Length)], audOpenVol);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;

        IOpen opn = other.GetComponent<IOpen>();
        if (opn != null)
        {
            door.SetActive(true);
            audDoor.PlayOneShot(audDoorOpen[Random.Range(0, audDoorOpen.Length)], audOpenVol);
        }
    }
}
