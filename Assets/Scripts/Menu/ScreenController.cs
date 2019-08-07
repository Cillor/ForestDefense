using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ScreenController : MonoBehaviour
{
    public static bool isPaused;

    public GameObject Shop, LevelSelect, ShopResume, LevelSelectResume, GunShop, GunShopReturn, PauseScreen, OptionsScreen;
    public Image FadeScreen;
    public float openRange = 5;
    public TMP_Text popUpText, notification;
    public static bool isShopOn;

    [Space]

    public string shopString, batteryConverterString, recoverStatusString, sureRecoverString, levelSelectorString, notPossibleToUseString;

    bool usePressed;

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            isShopOn = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            isPaused = false;
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            CursorShow();
            Pause();
            ActivateUsables();
            OpenUIs();
        }
    }

    void ActivateUsables()
    {
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, Camera.main.transform.forward, out hit, openRange))
        {
            if (hit.collider.CompareTag("BatteryConverter"))
            {
                popUpText.gameObject.SetActive(true);
                popUpText.text = batteryConverterString;
                if (Input.GetButtonDown("Use"))
                {
                    FindObjectOfType<StoreManager>().RechargeWeapon();
                }
            }
            if (hit.collider.CompareTag("RecoverCenter"))
            {
                if (!usePressed)
                {
                    popUpText.gameObject.SetActive(true);
                    popUpText.text = recoverStatusString;
                    if (Input.GetButtonDown("Use"))
                    {
                        if (!WaveManager.isWaveActive)
                            RecoverStatus();
                        else
                            usePressed = true;
                    }
                }
                else
                {
                    popUpText.gameObject.SetActive(true);
                    popUpText.text = sureRecoverString;
                    if (Input.GetButtonDown("Use"))
                    {
                        RecoverStatus();
                        usePressed = false;
                    }
                }
            }
        }
        else
        {
            popUpText.gameObject.SetActive(false);
            usePressed = false;
            if (isShopOn)
            {
                if (Input.GetKeyDown("Use"))
                {
                    OnShopOpenClose();
                }
            }
        }

    }
    public void OpenUIs()
    {
        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        if (!WaveManager.isWaveActive)
        {
            if (Physics.Raycast(rayOrigin, Camera.main.transform.forward, out hit, openRange))
            {
                if (hit.collider.CompareTag("Shop"))
                {
                    popUpText.gameObject.SetActive(true);
                    popUpText.text = shopString;
                    if (Input.GetButtonDown("Use"))
                    {
                        OnShopOpenClose();
                    }
                }
                else if (hit.collider.CompareTag("LevelSelector"))
                {
                    popUpText.gameObject.SetActive(true);
                    popUpText.text = levelSelectorString;
                    if (Input.GetButtonDown("Use"))
                    {
                        OCLevelSelector();
                    }
                }
                else if(!hit.collider.CompareTag("RecoverCenter") && !hit.collider.CompareTag("BatteryConverter"))
                {
                    popUpText.gameObject.SetActive(false);
                }
            }
            else
            {
                popUpText.gameObject.SetActive(false);
                if (isShopOn)
                {
                    if (Input.GetKeyDown("Use"))
                    {
                        OnShopOpenClose();
                    }
                }
            }
        }
        else
        {
            if (Physics.Raycast(rayOrigin, Camera.main.transform.forward, out hit, openRange))
            {
                if (hit.collider.CompareTag("Shop"))
                {
                    popUpText.gameObject.SetActive(true);
                    popUpText.text = notPossibleToUseString;
                }
                if (hit.collider.CompareTag("LevelSelector"))
                {
                    popUpText.gameObject.SetActive(true);
                    popUpText.text = notPossibleToUseString;
                }
            }
            else
            {
                popUpText.gameObject.SetActive(false);
            }
        }
    }

    bool OCLSOn;
    public void OCLevelSelector()
    {
        if (!OCLSOn)
        {
            LevelSelect.SetActive(true);
            LevelSelectResume.SetActive(true);
            OCLSOn = true;
            OnPauseUnpause();
        }
        else
        {
            LevelSelect.SetActive(false);
            Shop.SetActive(false);
            LevelSelectResume.SetActive(false);
            GunShop.SetActive(false);
            GunShopReturn.SetActive(false);
            OCLSOn = false;
            OnPauseUnpause();
        }
    }

    public void RecoverStatus()
    {
        if (WaveManager.isWaveActive)
        {
            StartCoroutine("FadeImage");
            SaveManager.Instance.state.batterysInInventory = 0;
            SaveManager.Instance.state.metalAmount = 0;
            SaveManager.Instance.state.recharges = 0;
            SaveManager.Instance.state.wattsStored = 0;
            if (SaveManager.Instance.state.actualWave > 0)
                SaveManager.Instance.state.waveUnlocked[SaveManager.Instance.state.actualWave] = false;

            SaveManager.Instance.state.stamina = PlayerStatus.maxStaminaStatic;
            SaveManager.Instance.state.poison = 0;
        }
        else
        {
            StartCoroutine("FadeImage");
            SaveManager.Instance.state.stamina = PlayerStatus.maxStaminaStatic;
            SaveManager.Instance.state.poison = 0;
        }
    }

    IEnumerator FadeImage()
    {
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            FadeScreen.color = new Color(0, 0, 0, i * 2);
            yield return null;
        }

        OnPauseUnpause();
        yield return new WaitForSecondsRealtime(1f);
        OnPauseUnpause();

        for (float i = 2; i >= 0; i -= Time.deltaTime)
        {
            FadeScreen.color = new Color(0, 0, 0, i * 2);
            yield return null;
        }
    }

    public void OnShopOpenClose()
    {
        if (!isShopOn)
        {
            Shop.SetActive(true);
            ShopResume.SetActive(true);
            isShopOn = true;
            OnPauseUnpause();
        }
        else
        {
            Shop.SetActive(false);
            ShopResume.SetActive(false);
            GunShop.SetActive(false);
            GunShopReturn.SetActive(false);
            isShopOn = false;
            OnPauseUnpause();
        }
    }

    public void OnPauseUnpause()
    {
        if (isPaused)
        {
            isPaused = false;
        }
        else
        {
            isPaused = true;
        }
    }

    void CursorShow()
    {
        if (isPaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
        }
    }

    void Pause()
    {
        if (isPaused && Input.GetButtonDown("Pause"))
        {
            isPaused = false;
            PauseScreen.SetActive(false);
            OptionsScreen.SetActive(false);
            LevelSelect.SetActive(false);
            Shop.SetActive(false);
            ShopResume.SetActive(false);
            LevelSelectResume.SetActive(false);
            GunShop.SetActive(false);
            GunShopReturn.SetActive(false);
            OCLSOn = false;
            isShopOn = false;
        }
        else if (!isPaused && Input.GetButtonDown("Pause"))
        {
            isPaused = true;
            PauseScreen.SetActive(true);
        }
    }

    public void SaveGame()
    {
        SaveManager.Instance.Save();
    }

    public void SceneLoader(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }
}
