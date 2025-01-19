using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] int speed;
    [SerializeField] bool isX;
    [SerializeField] bool isY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isY)
        {
            transform.Rotate(Vector3.up, Time.deltaTime * speed);
        }
        else if (isX)
        {
            transform.Rotate(Vector3.forward, Time.deltaTime * speed);
        }
    }
}
