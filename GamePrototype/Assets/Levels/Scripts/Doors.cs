using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    [SerializeField] GameObject door;

    [SerializeField] AudioSource audDoor;
    [SerializeField] AudioClip[] audDoorOpen;
    [SerializeField][Range(0, 1)] float audOpenVol;
    [SerializeField] Animator _animator;
    [SerializeField] GameObject _ParticleSpawnPointA;
    [SerializeField] GameObject _ParticleSpawnPointB;
    [SerializeField] GameObject _DustParticles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IOpen opn = other.GetComponent<IOpen>();
        if (opn != null)
        {
            //door.SetActive(false);
            //_animator.SetTrigger("Open");
            _animator.SetBool("isOpen", true);
            audDoor.PlayOneShot(audDoorOpen[0], audOpenVol);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;

        IOpen opn = other.GetComponent<IOpen>();
        if (opn != null)
        {
            //door.SetActive(true);
            //_animator.SetTrigger("Close");
            _animator.SetBool("isOpen", false);
            audDoor.PlayOneShot(audDoorOpen[0], audOpenVol);
        }
    }

    public void playSlam()
    {
        spawnParticles();
        audDoor.PlayOneShot(audDoorOpen[1], audOpenVol);
    }

    public void spawnParticles()
    {
        Instantiate(_DustParticles, _ParticleSpawnPointA.transform.position, _ParticleSpawnPointA.transform.rotation);
        Instantiate(_DustParticles, _ParticleSpawnPointB.transform.position, _ParticleSpawnPointB.transform.rotation);
        //Instantiate(_DustParticles, transform);
    }
}
