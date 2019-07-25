using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class ShootController : MonoBehaviour
{
    #region Public Variables
    public Guns gun;
    public Transform gunEnd;
    public TMP_Text energyStoredTxt;
    public GameObject bp1, bp2, bp3;
    [Space]
    public GameObject metalShootFX;
    public static ShootController Instance { set; get; }

    public Guns[] gunsArray;
    #endregion

    #region Private Variables
    private int gunDamage;
    private float fireRate;
    private float weaponRange;
    private float hitForce;
    private bool isAutomatic;
    [HideInInspector]
    public float energyStored;
    [HideInInspector]

    private Camera cam;
    private WaitForSeconds shotDuration = new WaitForSeconds(.02f);
    private AudioSource gunAudio;
    private LineRenderer laserLine;
    private float nextFire;   
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        cam = Camera.main;

        gun = gunsArray[SaveManager.Instance.state.equipedGunId];
        InstantiateGun(gun.model);
        GunStatus();
    }

    // Update is called once per frame
    void Update()
    {
        if(!ScreenController.isPaused)
            KindOfGun();

        gunEnd = GameObject.FindGameObjectWithTag("GunEnd").GetComponent<Transform>();
        energyStoredTxt.text = energyStored + "W";/* + SaveManager.Instance.state.recharges;*/

        RechargesUI();
    }

    void RechargesUI()
    {
        if (SaveManager.Instance.state.recharges == 0)
        {
            bp1.SetActive(false);
            bp2.SetActive(false);
            bp3.SetActive(false);
        }
        else if (SaveManager.Instance.state.recharges == 1)
        {
            bp1.SetActive(true);
            bp2.SetActive(false);
            bp3.SetActive(false);
        }
        else if (SaveManager.Instance.state.recharges == 2)
        {
            bp1.SetActive(true);
            bp2.SetActive(true);
            bp3.SetActive(false);
        }
        else if (SaveManager.Instance.state.recharges == 3)
        {
            bp1.SetActive(true);
            bp2.SetActive(true);
            bp3.SetActive(true);
        }
    }

    public void InstantiateGun(GameObject gunModel)
    {
        List<Transform> children = new List<Transform>();
        children.AddRange(GetComponentsInChildren<Transform>());
        children.RemoveAt(0);
        foreach(Transform child in children)
        {
            Destroy(child.gameObject);
        }
        GameObject clone = Instantiate(gunModel);
        clone.transform.parent = transform;
        clone.transform.localPosition = new Vector3(0, 0, 0);
        clone.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void EquipWeapon(Guns _gun)
    {
        gun = _gun;
        GunStatus();
        InstantiateGun(gun.model);
    }

    private void KindOfGun()
    {
        if (isAutomatic)
        {
            if (Input.GetButton("Fire1") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;

                if (energyStored > 0)   
                    Shooting();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;

                if(energyStored > 0)
                    Shooting();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
            Reload();

        //Debug.DrawRay(cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f)), cam.transform.forward * weaponRange, Color.green);
        //Debug.DrawRay(gunEnd.position, gunEnd.forward * weaponRange, Color.red);
    }

    private void Shooting()
    {
        StartCoroutine(ShotEffect());

        Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        laserLine.SetPosition(0, gunEnd.position);

        gunAudio.Play();

        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, cam.transform.forward, out hit, weaponRange))
        {
            laserLine.SetPosition(1, hit.point);

            AlienStatus aStat = hit.collider.gameObject.GetComponentInParent<AlienStatus>();
            if (aStat != null)
            {
                aStat.Health(gunDamage);
                Instantiate(metalShootFX, hit.point, Quaternion.identity);
            }
        }
        else
        {
            RaycastHit hitPoint;
            if (Physics.Raycast(gunEnd.position, gunEnd.forward, out hitPoint, weaponRange))
            {
                laserLine.SetPosition(1, hitPoint.point);

                AlienStatus aStat = hitPoint.collider.gameObject.GetComponentInParent<AlienStatus>();
                if (aStat != null)
                {
                    aStat.Health(gunDamage);
                    Instantiate(metalShootFX, hit.point, Quaternion.identity);
                }
            }
            else
            {
                laserLine.SetPosition(1, rayOrigin + (cam.transform.forward * weaponRange));
            }
        }

        energyStored -= 1000;
    }

    private void Reload()
    {
        if ((SaveManager.Instance.state.recharges > 0) && (energyStored != gun.energyStored))
        {
            energyStored = gun.energyStored;
            SaveManager.Instance.state.recharges--;
        }
    }

    private IEnumerator ShotEffect()
    {
        gunAudio.Play();
        laserLine.enabled = true;

        yield return shotDuration;

        laserLine.enabled = false;
    }

    public void GunStatus()
    {
        gunDamage = gun.gunDamage;
        fireRate = gun.fireRate;
        weaponRange = gun.weaponRange;     
        isAutomatic = gun.isAutomatic;
        energyStored = gun.energyStored;
        gunAudio.clip = gun.gunSound;
    }
}
