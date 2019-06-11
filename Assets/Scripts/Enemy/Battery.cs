using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    public int minimumEnergy = 300;
    public int maximumEnergy = 1500;
    public AudioClip getBattery, batteryInConverter;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SaveManager.Instance.state.batterysInInventory++;
            other.gameObject.GetComponent<AudioSource>().clip = getBattery;
            other.gameObject.GetComponent<AudioSource>().Play();

            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("BatteryConverter"))
        {
            SaveManager.Instance.state.wattsStored += Random.Range(minimumEnergy, maximumEnergy);
            other.gameObject.GetComponentInParent<AudioSource>().clip = batteryInConverter;
            other.gameObject.GetComponentInParent<AudioSource>().Play();

            Destroy(gameObject);
        }
    }
}
