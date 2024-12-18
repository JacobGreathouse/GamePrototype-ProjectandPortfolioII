using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDetect : MonoBehaviour
{
    [SerializeField] private GameObject miniMapQuad;

    private void Start()
    {
        miniMapQuad.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (miniMapQuad != null)
            {
                miniMapQuad.SetActive(true);
            }
        }
    }
}
