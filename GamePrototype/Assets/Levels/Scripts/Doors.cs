using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    [SerializeField] GameObject door;

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
        }
    }
}
