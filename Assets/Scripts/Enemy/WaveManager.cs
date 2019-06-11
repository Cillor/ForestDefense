using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public GameObject alien;
    public float timeBtwWaves;
    public AnimationCurve enemysPerWave = new AnimationCurve();
    public AnimationCurve nextEnemyIn = new AnimationCurve();
    [HideInInspector]
    public float nextWaveIn, enemysAlive;
    public static bool isWaveActive;
    [Space]
    public GameObject[] spawnPoint;

    public event Action clearWave;

    public TMP_Text nextWaveInText, actualWaveText, waveReadyText, waveStartedText;

    bool waveReseted = true;

    private void Start()
    {
        nextWaveIn = timeBtwWaves;
    }

    void SandBox()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ResetWave();
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            SaveManager.Instance.state.actualWave--;
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            SaveManager.Instance.state.actualWave++;
            if (!SaveManager.Instance.state.waveUnlocked[SaveManager.Instance.state.actualWave])
            {
                SaveManager.Instance.state.waveUnlocked[SaveManager.Instance.state.actualWave] = true;
            }
        }
    }

    private void Update()
    {
        //SandBox();

        if (enemysAlive <= 0)
        {
            isWaveActive = false;
        }

        WaveLimiter();

        actualWaveText.text = "Onda: " + SaveManager.Instance.state.actualWave;

        if (enemysAlive <= 0 && !isWaveActive)
        {
            if (!waveReseted && waveUp)
                UnlockNextWave();

            if (nextWaveIn <= 0)
            {
                nextWaveInText.gameObject.SetActive(false);
                waveReadyText.gameObject.SetActive(true);
                StartNewWave();
            }
            else
            {
                nextWaveInText.gameObject.SetActive(true);
                nextWaveInText.text = "Próxima onda em: " + Helper.Round(nextWaveIn, 2);
                nextWaveIn -= Time.deltaTime;
            }
        }
    }

    bool waveUp;
    private void UnlockNextWave()
    {
        if (!SaveManager.Instance.state.waveUnlocked[SaveManager.Instance.state.actualWave + 1])
        {
            SaveManager.Instance.state.waveUnlocked[SaveManager.Instance.state.actualWave + 1] = true;
        }
        if (SaveManager.Instance.state.waveUnlocked[SaveManager.Instance.state.actualWave + 1])
        {
            SaveManager.Instance.state.actualWave++;
        }
        waveUp = false;
    }

    void StartNewWave()
    {
        if (Input.GetButtonDown("StartWave"))
        {
            waveReadyText.gameObject.SetActive(false);
            waveStartedText.gameObject.SetActive(true);

            SpawnEnemys(SaveManager.Instance.state.actualWave);

            nextWaveIn = timeBtwWaves;
        }
    }

    void SpawnEnemys(int wave)
    {
        isWaveActive = true;
        int enemysToSpawn = Mathf.RoundToInt(enemysPerWave.Evaluate(wave)) + Random.Range(-4, 4);
        float timeBtwEnemys = nextEnemyIn.Evaluate(wave);
        StartCoroutine(Spawner(enemysToSpawn, timeBtwEnemys));
        waveReseted = false;
    }

    IEnumerator Spawner(int _enemysToSpawn, float _timeBtwEnemys)
    {
        List<GameObject> allowedSpawns = new List<GameObject>();
        allowedSpawns.Clear();
        allowedSpawns.AddRange(spawnPoint);

        List<GameObject> notAllowedSpawns = new List<GameObject>();
        notAllowedSpawns.Clear();

        while (_enemysToSpawn > 0)
        {
            if (notAllowedSpawns.Count > 6)
            {
                allowedSpawns.Add(notAllowedSpawns[0]);
                notAllowedSpawns.RemoveAt(0);
            }

            int _spawn = Random.Range(0, allowedSpawns.Count - 1);
            Instantiate(alien, allowedSpawns[_spawn].transform.position, alien.transform.rotation);

            notAllowedSpawns.Add(allowedSpawns[_spawn]);
            allowedSpawns.RemoveAt(_spawn);

            _enemysToSpawn--;
            yield return new WaitForSeconds(_timeBtwEnemys);
            waveStartedText.gameObject.SetActive(false);
            waveUp = true;
        }
    }

    void WaveLimiter()
    {
        if (SaveManager.Instance.state.actualWave <= 0)
        {
            SaveManager.Instance.state.actualWave = 0;
        }

        if (SaveManager.Instance.state.actualWave >= 10)
        {
            SaveManager.Instance.state.actualWave = 9;
        }

        if (SaveManager.Instance.state.actualWave >= 0)
        {
            while (!SaveManager.Instance.state.waveUnlocked[SaveManager.Instance.state.actualWave])
            {
                SaveManager.Instance.state.actualWave--;
            }
        }
    }

    public void ResetWave()
    {
        if (clearWave != null)
        {
            clearWave();
        }

        nextWaveIn = timeBtwWaves;
        waveReseted = true;
        StopAllCoroutines();
        isWaveActive = false;

        if (SaveManager.Instance.state.actualWave > 0)
        {
            //SaveManager.Instance.state.waveUnlocked[SaveManager.Instance.state.actualWave] = false;
            SaveManager.Instance.state.actualWave--;
        }
    }
}
