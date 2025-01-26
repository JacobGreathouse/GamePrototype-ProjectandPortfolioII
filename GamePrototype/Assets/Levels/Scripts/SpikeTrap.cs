using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    // private properties ----------------------------------------------------
    enum SPIKEMODE {IDLE, RETRACTING, EXTENDING }

    SPIKEMODE _mode = SPIKEMODE.IDLE;

    // Inspector serializables -------------------------------------------------
    [SerializeField] Animator _animator;
    [SerializeField] float _outTime;
    [SerializeField] float _inTime;
    [SerializeField] int _dmg;

    [Header("----- Spike Audio -----")]
    [SerializeField] AudioSource audSpike;
    [SerializeField] AudioClip[] audSpikeMove;
    [SerializeField][Range(0, 1)] float audSpikeVol;

    [Header("----- Hurt Audio -----")]
    [SerializeField] AudioSource audHurt;
    [SerializeField] AudioClip[] audHitPlayer;
    [SerializeField][Range(0, 1)] float audHurtVol;
    // Start is called before the first frame update
    void Start()
    {
        if(_outTime > 0)
        {
            StartCoroutine(RetractTimer(_inTime));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RetractTimer(float inTime)
    {
        yield return new WaitForSeconds(inTime);
        Retract();
    }

    IEnumerator ExtendTimer(float outTime)
    {
        yield return new WaitForSeconds(outTime);
        Extend();
    }


    public void Extend()
    {
        _mode = SPIKEMODE.EXTENDING;
        _animator.SetTrigger("Extend");
        audSpike.PlayOneShot(audSpikeMove[Random.Range(0, audSpikeMove.Length)], audSpikeVol);
        if (_outTime > 0)
        {
            StartCoroutine(RetractTimer(_outTime));
        }
    }

    public void Retract()
    {
        _mode = SPIKEMODE.IDLE;
        _animator.SetTrigger("Retract");
        audSpike.PlayOneShot(audSpikeMove[Random.Range(0, audSpikeMove.Length)], audSpikeVol);
        if (_inTime > 0)
        {
            StartCoroutine(ExtendTimer(_inTime));
        }
    }

    public void Hurt()
    {


        switch (_mode)
        {
            case SPIKEMODE.IDLE:
            case SPIKEMODE.RETRACTING:
                if ((!gamemanager.instance.playerScript.Controller.isGrounded) && gamemanager.instance.playerScript.VertMovement < -0.5f)
                {
                    audHurt.PlayOneShot(audHitPlayer[Random.Range(0, audHitPlayer.Length)], audHurtVol);
                    gamemanager.instance.playerScript.takeDamage(_dmg);
                }
                break;
            case SPIKEMODE.EXTENDING:
                if (true) // the laziest code I've ever written - gabe
                {
                    audHurt.PlayOneShot(audHitPlayer[Random.Range(0, audHitPlayer.Length)], audHurtVol);
                    gamemanager.instance.playerScript.takeDamage(_dmg);
                }
                break;
        }
        
    }
}
