using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class StaffStats : ScriptableObject
{
    [Header("----- Staff Stats -----")]
    public GameObject model;
    //public int shootDamage;
   // public int shootDistance;
    public float shootRate;
    public GameObject lightning;
    public GameObject fireball;
    public GameObject Missile;
    public GameObject bunnyBomb;
    public int spellcost;
    public int maxHits;
    public float AOERange;
    public bool isBolt;
    public bool isFire;
    public bool isMissile;
    public bool isBunny;

    [Header("----- SFX Effects -----")]

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootSoundVol;
}
