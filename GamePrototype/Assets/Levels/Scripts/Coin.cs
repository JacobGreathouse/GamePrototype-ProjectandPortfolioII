using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] AudioSource audCoin;
    [SerializeField] AudioClip[] audClink;
    [SerializeField][Range(0, 1)] float audCoinVol;


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
        if(other.CompareTag("Player"))
        {
            audCoin.PlayOneShot(audClink[Random.Range(0, audClink.Length)], audCoinVol);

            gamemanager.instance.player.GetComponent<PlayerController>().SetCoinCount(1);

            StartCoroutine(CoinDeath());
        }
    }

    IEnumerator CoinDeath()
    {
        this.GetComponent<MeshRenderer>().enabled = false;

        yield return new WaitForSeconds(.25f);

        Destroy(gameObject);
    }
}
