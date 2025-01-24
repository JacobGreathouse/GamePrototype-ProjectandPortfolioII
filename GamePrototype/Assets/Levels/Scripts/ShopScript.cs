using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{

    bool playerInTrigger;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInTrigger)
        {
            if(Input.GetButtonDown("Interact") && gamemanager.instance.player.GetComponent<PlayerController>().CoinCount >= 10)
            {
                BuyHealthPotion();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        playerInTrigger = true;
        
    }

    void BuyHealthPotion()
    {
        gamemanager.instance.player.GetComponent<PlayerController>().setHealthPotion(1);
        gamemanager.instance.player.GetComponent<PlayerController>().CoinCount = -10;
    }
}
