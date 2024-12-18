using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IDetect : MonoBehaviour
{
    [SerializeField] private GameObject miniMapQuad;
    [SerializeField] private Image bossHp;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (miniMapQuad != null)
            {
                miniMapQuad.SetActive(true);
            }
            if (bossHp != null)
            {
                bossHp.gameObject.SetActive(true);
            }
        }
    }
}
