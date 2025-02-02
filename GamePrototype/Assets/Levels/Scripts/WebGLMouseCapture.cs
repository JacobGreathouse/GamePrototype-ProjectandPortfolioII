using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGLMouseCapture : MonoBehaviour
{
    private void OnMouseDown()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
