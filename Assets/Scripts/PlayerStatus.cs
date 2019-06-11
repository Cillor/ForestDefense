using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kino;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour
{
    public int maxStamina = 100;
    public static float maxStaminaStatic;
    public int maxPoison = 1;
    public float cameraDistance;

    public float maxStaminaLossPerSecond = 10;

    public AnimationCurve coffeeEffectiveness = new AnimationCurve();
    public float coffeeAddicitionRate = 0.01f;
    public float coffeStaminaRecover;

    public AnimationCurve staminaEffect;

    public GameObject battery;

    public AudioClip oof;

    public void Start()
    {
        maxStaminaStatic = maxStamina;
        if (!SaveManager.Instance.state.firstStartOcurred)
        {
            SaveManager.Instance.state.stamina = maxStamina;
            SaveManager.Instance.state.poison = 0;
        }
    }

    private void Update()
    {
        if (!ScreenController.isPaused)
        {
            Stamina();
            UseCoffee();
            BatteryThrow();
            ScreenBlur();
        }
    }

    void ScreenBlur()
    {
        Kino.Motion.Instance._frameBlending = staminaEffect.Evaluate(SaveManager.Instance.state.stamina) / 100;
    }

    public void PoisonChange(float _poisonChange)
    {
        SaveManager.Instance.state.poison += _poisonChange;
        GetComponent<AudioSource>().clip = oof;
        GetComponent<AudioSource>().pitch = 1;
        GetComponent<AudioSource>().Play();

        if (SaveManager.Instance.state.poison >= maxPoison)
        {
            SaveManager.Instance.state.poison = maxPoison;
        }
        if (SaveManager.Instance.state.poison <= 0)
        {
            SaveManager.Instance.state.poison = 0;
        }
    }

    void Stamina()
    {
        SaveManager.Instance.state.stamina -= Time.deltaTime * maxStaminaLossPerSecond * SaveManager.Instance.state.poison;

        if (SaveManager.Instance.state.stamina <= 0)
        {
            Die();
        }
    }

    public void StaminaChange(float _staminaChange)
    {
        SaveManager.Instance.state.stamina += _staminaChange;

        if (SaveManager.Instance.state.stamina >= maxStamina)
        {
            SaveManager.Instance.state.stamina = maxStamina;
        }
    }

    public void UseCoffee()
    {
        if (Input.GetButtonDown("Coffee"))
        {
            if (SaveManager.Instance.state.coffeeQuantity > 0)
            {
                SaveManager.Instance.state.coffeeQuantity--;
                StaminaChange(coffeeEffectiveness.Evaluate(SaveManager.Instance.state.coffeeAddiction) * coffeStaminaRecover);
                if (SaveManager.Instance.state.coffeeAddiction < 1)
                    SaveManager.Instance.state.coffeeAddiction += coffeeAddicitionRate;
            }
        }

        if (SaveManager.Instance.state.coffeeAddiction > 0)
        {
            SaveManager.Instance.state.coffeeAddiction -= Time.deltaTime * (coffeeAddicitionRate / 64);
        }
    }

    void BatteryThrow()
    {
        if (Input.GetButtonDown("ThrowBattery"))
        {
            if (SaveManager.Instance.state.batterysInInventory > 0)
            {
                SaveManager.Instance.state.batterysInInventory--;
                var clone = Instantiate(battery, Camera.main.transform.position + Camera.main.transform.forward * cameraDistance, Camera.main.transform.rotation);
                clone.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 800);
            }
        }
    }

    public void Die()
    {
        SceneManager.LoadScene("DeathScene");
        Destroy(gameObject);
    }
}
