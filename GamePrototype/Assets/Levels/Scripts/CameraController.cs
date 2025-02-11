using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //[SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // get input
        int sensVal = gamemanager.instance.sensVal;

        if(Application.platform == RuntimePlatform.WebGLPlayer)
        {
            sensVal = (int)((float)sensVal * 0.15);
        }


        float mouseX = Input.GetAxis("Mouse X") * sensVal * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensVal * Time.deltaTime;

        // tie the mouseY input to the rotX
        if (invertY)
            rotX -= mouseY;
        else
            rotX += mouseY;

        // clamp the cam x rot
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // rotate the cam on the x-axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // rotate the player on the y-axis
        transform.parent.Rotate(Vector3.up * mouseX);

    }
}
