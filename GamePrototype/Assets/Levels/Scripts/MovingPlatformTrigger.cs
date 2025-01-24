using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformTrigger : MonoBehaviour
{
    [SerializeField] MovingPlatform _parent;
    public MovingPlatform parent => _parent;
}
