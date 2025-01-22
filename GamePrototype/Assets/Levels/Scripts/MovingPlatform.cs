using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // -------------------------------------------------- PRIVATE MEMBERS

    enum MOVEMENT_STATE {IN, OUT}
    MOVEMENT_STATE _state = MOVEMENT_STATE.IN;
    Vector3 _startPos;
    Quaternion _startOrientation;
    Vector3 _directionVector;

    // -------------------------------------------------- SERIALIZED PRIVATE MEMBERS

    [SerializeField] GameObject _endNode;
    [SerializeField] GameObject _platform;
    [SerializeField] [Range(0, 100)] float _movementSpeed;
    [SerializeField] [Range(0, 100)] float _pauseA;
    [SerializeField] [Range(0, 100)] float _pauseB;

    // ------------------------------------------------ PUBLIC PROPERTIES

    public Vector3 LinearVelocity = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        _startPos = transform.position;
        _startOrientation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {
            case MOVEMENT_STATE.IN: // towards endpoint
                _directionVector = (_endNode.transform.position - _startPos).normalized;
                LinearVelocity = _directionVector * _movementSpeed;
                _platform.transform.position += LinearVelocity * Time.deltaTime;

                if (Vector3.Distance(_endNode.transform.position, _platform.transform.position) <= 0.1)
                {
                    _state = MOVEMENT_STATE.OUT;
                }
                break;

            case MOVEMENT_STATE.OUT: // towards startpoint
                _directionVector = (_startPos - _endNode.transform.position).normalized;
                LinearVelocity = _directionVector * _movementSpeed;
                _platform.transform.position += LinearVelocity * Time.deltaTime;

                if (Vector3.Distance(_startPos, _platform.transform.position) <= 0.1)
                {
                    _state = MOVEMENT_STATE.IN;
                }

                break;

        }

    }

    IEnumerator wait(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
