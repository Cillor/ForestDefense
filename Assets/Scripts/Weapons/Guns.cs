using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class Guns : ScriptableObject
{
    public int gunId;
    public GameObject model;
    [Space]
    public Sprite gunIcon;
    public string gunName;
    public float gunPrice;
    public AudioClip gunSound;

    public bool isAutomatic = false;
    public int energyStored = 15;

    public int gunDamage = 35;
    public float fireRate = .25f;
    public float weaponRange = 100f;
}
