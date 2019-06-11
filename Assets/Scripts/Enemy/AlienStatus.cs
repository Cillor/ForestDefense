using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienStatus : MonoBehaviour
{
    public int maxHealth = 100;
    [Space]
    public GameObject battery, metalPiece;
    
    [Range(0,100)]
    public float[] metalDropChance = new float [10];
    [Range(0,100)]
    public float[] batteryDropChance = new float [10];

    [Space]
    public GameObject explosionFX;

    private WaveManager waveM;
    float health;

    private void Start()
    {        
        health = maxHealth;
        waveM = FindObjectOfType<WaveManager>();
        waveM.clearWave += Eliminate;
        waveM.enemysAlive++;
    }

    public void Health(int lifeChange)
    {
        health += lifeChange;

        if (health >= maxHealth)
        {
            health = maxHealth;
        }
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Instantiate(explosionFX, transform.position, Quaternion.identity);
        DropBattery();
        DropMetalPiece();
        waveM.enemysAlive--;
        Destroy(gameObject);
    }

    public void Eliminate()
    {
        waveM.clearWave -= Eliminate;
        waveM.enemysAlive--;
        Destroy(gameObject);
    }

    void DropMetalPiece()
    {
        if (Random.value <= (metalDropChance[SaveManager.Instance.state.actualWave] / 100f))
            Instantiate(metalPiece, transform.position, metalPiece.transform.rotation);
    }

    void DropBattery()
    {
        if (Random.value <= (batteryDropChance[SaveManager.Instance.state.actualWave] / 100f))
            Instantiate(battery, transform.position, battery.transform.rotation);
    }
}
