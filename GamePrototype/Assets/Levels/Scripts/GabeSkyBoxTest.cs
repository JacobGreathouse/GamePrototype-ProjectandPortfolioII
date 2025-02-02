using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


public class GabeSkyBoxTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Update()
    {

        //transform.localRotation = Quaternion.Euler(rotX, 0, 0);
        //transform.parent.Rotate(Vector3.up * mouseX);

        transform.localRotation = Quaternion.Euler(Camera.main.transform.localRotation.eulerAngles.x, gamemanager.instance.player.transform.rotation.eulerAngles.y, 0);

    }
}
