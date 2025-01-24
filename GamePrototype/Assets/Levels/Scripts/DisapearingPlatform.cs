using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisapearingPlatform : MonoBehaviour
{
    [SerializeField] float _strength;
    [SerializeField] float _respawnCooldown;
    [SerializeField] GameObject _platform;
    [SerializeField] GameObject _trigger;

    bool _isCooling = false;
    bool _isActive = false;
    float _time = 0.0f;

    public bool isActive 
    { 
        get 
        { 
            return _isActive; 
        }
        set
        {
            _isActive = value;
        }
    }

    

    private void Update()
    {
        if (!_isCooling)
        {
            if (_isActive)
            {
                _time += Time.deltaTime;
                Debug.Log(_time);
            }
            else
            {
                _time = 0;
            }

            if (_time > _strength)
            {
                _platform.SetActive(false);
                _time = 0;
                _isActive = false;
                CoolDown(_respawnCooldown);
            }
        }
    }

    IEnumerator CoolDown(float time)
    {
        if (!_isCooling)
        {
            _isCooling = true;
            yield return new WaitForSeconds(time);

            _platform.SetActive(true);
            _time = 0.0f;
            _isCooling = false;
        }
    }



}
