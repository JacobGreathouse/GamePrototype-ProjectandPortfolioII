using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGLMouseCapture : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
