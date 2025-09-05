using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "ScriptableObjects/GunData")]
public class GunData : ScriptableObject
{
    public AudioClip shotClip;
    public AudioClip reloadClip;
    public float damage = 25f;
    public int magCapacity = 25;
    public int startAmmoRemaining = 100; // Ã³À½ Åº¾à ¼ö
    public float timeBetFire = 0.1f;
    public float reloadTime = 1.8f;
}
