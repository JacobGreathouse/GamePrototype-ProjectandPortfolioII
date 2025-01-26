using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    bool TutorialRan = false;
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

        if(other.CompareTag("Player") && TutorialRan == false)
        {
            TutorialRan=true;
            gamemanager.instance.HowToPlayMenu.SetActive(true);
            gamemanager.instance.statePause();
        }
    }
}
