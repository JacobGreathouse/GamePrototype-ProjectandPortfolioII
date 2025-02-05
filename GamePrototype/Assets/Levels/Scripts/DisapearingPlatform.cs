using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisapearingPlatform : MonoBehaviour
{
    [SerializeField] float _strength;
    [SerializeField] float _respawnCooldown;
    [SerializeField] float _timingOffset;
    [SerializeField] GameObject _platform;
    [SerializeField] GameObject _trigger;
    [SerializeField] Animator _animator;
    [SerializeField] AudioClip[] _sfx;
    [SerializeField] AudioSource _audioSource;
    

    bool _isCooling = false;
    bool _isActive = false;
    float _time;

    bool _isVisible;
    bool _soundPlayed = false;

    private void Start()
    {
        _time -= _timingOffset;
    }

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
        float rumbleAmt = calcRumble(_time, _strength);
        _audioSource.volume = rumbleAmt;
        if (rumbleAmt > 0.5f && !_soundPlayed)
        {
            _soundPlayed = true;
            _audioSource.PlayOneShot(_sfx[0]);
        }

        _animator.SetFloat("Rumble", rumbleAmt);


        if (!_isCooling)
        {
            
            _time += Time.deltaTime;

            //_animator.SetFloat("Rumble", 0);

            

            if (_time > _strength )
            {

                //_isVisible = !_isVisible;
                //_platform.SetActive(false);
                _animator.SetBool("isVisable", false);
                
                //_audioSource.pitch = 0.95f;
                //_audioSource.PlayOneShot(_sfx[1]);
                _time = 0;
                StartCoroutine(CoolDown(_respawnCooldown));
                
            }
        }
        else
        {
            
            //_animator.SetFloat("Rumble", 0);
        }
    }

    IEnumerator CoolDown(float time)
    {

        _isCooling = true;
        yield return new WaitForSeconds(time);
        //_platform.SetActive(true);
        _animator.SetBool("isVisable", true);
        _soundPlayed = false;
        _audioSource.pitch = 0.90f;
        _audioSource.PlayOneShot(_sfx[1]);
        _time = 0.0f;
        _isCooling = false;
    }

    private float calcRumble(float time, float max)
    {
        float normRumble = time / max;
        return normRumble;
    }



}
