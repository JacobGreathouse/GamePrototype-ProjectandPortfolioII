using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disapearingwalls : MonoBehaviour
{
    [SerializeField] GameObject[] WallsToDisapear;
    [SerializeField] GameObject[] WallsToAppear;
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

        if (other.CompareTag("Player"))
        {

            for(int i = 0; i < WallsToDisapear.Length && WallsToDisapear != null; i++)
            {
                WallsToDisapear[i].SetActive(false);
            }

            for(int i = 0; i < WallsToAppear.Length && WallsToAppear != null; i++)
            {
                WallsToAppear[i].SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) 
            return;

        if (other.CompareTag("Player"))
        {

            for (int i = 0; i < WallsToDisapear.Length && WallsToDisapear != null; i++)
            {
                WallsToDisapear[i].SetActive(true);
            }

            for (int i = 0; i < WallsToAppear.Length && WallsToAppear != null; i++)
            {
                WallsToAppear[i].SetActive(false);
            }
        }
    }
}
