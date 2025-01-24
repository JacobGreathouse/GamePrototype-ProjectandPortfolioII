using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisapearingPlatformTrigger : MonoBehaviour
{
    [SerializeField] DisapearingPlatform dp;

    public DisapearingPlatform parent => dp;

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<FootTrigger>() != null)
        {
            dp.isActive = false;
        }
    }
}
