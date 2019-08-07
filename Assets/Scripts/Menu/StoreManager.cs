using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StoreManager : MonoBehaviour
{
    #region Public Variables
    public TMP_Text coffeeQuantityText, buyCoffeeText, buyWeaponText, weaponInfoText;
    [Space]
    public Image weaponImage;
    public Guns[] gunVetor;
    public TMP_Text[] gunsText;
    #endregion

    #region Private Variables
    private int coffeeQuant = 1;
    #endregion
    void Start()
    {
        buyCoffeeText.text = "Buy " + coffeeQuant + " coffee(s) for " + Helper.WattsNomenclature(coffeeQuant * 500);

        for (int i = 0; i < gunVetor.Length; i++)
        {
            ChangeWeaponText(i);
        }
    }

    #region Public Methods
    public void BuyAntenna(int metalQ)
    {
        if (SaveManager.Instance.state.metalAmount >= metalQ)
        {
            SaveManager.Instance.state.endGameOcurred = true;
            StartCoroutine("EndGameCredits");
        }
    }

    IEnumerator EndGameCredits()
    {
        yield return new WaitForSecondsRealtime(5f);
        SceneManager.LoadScene("CongratsForConclusion");
    }

    public void UpdateCoffeeQuantity(int quant)
    {
        if (coffeeQuant + quant > 0)
            coffeeQuant += quant;
        coffeeQuantityText.text = coffeeQuant.ToString();
        buyCoffeeText.text = "Comprar " + coffeeQuant + " café(s) por " + Helper.WattsNomenclature(coffeeQuant * 500);
    }

    public void RechargeWeapon()
    {
        if (SaveManager.Instance.state.wattsStored > 0)
        {
            float wattsToRecharge = ShootController.Instance.gun.energyStored - ShootController.Instance.energyStored;

            if (wattsToRecharge > 0)
            {
                ShootController.Instance.energyStored += wattsToRecharge;
                SaveManager.Instance.state.wattsStored -= wattsToRecharge;

                if (SaveManager.Instance.state.wattsStored < 0)
                {
                    SaveManager.Instance.state.wattsStored = Mathf.Abs(SaveManager.Instance.state.wattsStored);

                    ShootController.Instance.energyStored -= SaveManager.Instance.state.wattsStored;
                    SaveManager.Instance.state.wattsStored -= SaveManager.Instance.state.wattsStored;
                }
            }
        }
    }

    public void BuyPortableRecharge()
    {
        if (SaveManager.Instance.state.wattsStored >= ShootController.Instance.gun.energyStored && SaveManager.Instance.state.recharges < 3)
        {
            SaveManager.Instance.state.recharges++;
            SaveManager.Instance.state.wattsStored -= ShootController.Instance.gun.energyStored;
        }
    }

    public void BuyCoffe(float cost)
    {
        if (SaveManager.Instance.state.wattsStored >= cost * coffeeQuant)
        {
            SaveManager.Instance.state.coffeeQuantity += coffeeQuant;
            SaveManager.Instance.state.wattsStored -= cost * coffeeQuant;
        }
    }

    public void ChangeWeaponText(int ID)
    {
        Guns gun = gunVetor[ID];
        gunsText[ID].text = gun.name;
    }

    int currentWeaponID;
    public void SelectWeapon(int ID)
    {
        currentWeaponID = ID;
        Guns gun = gunVetor[currentWeaponID];
        float weaponPrice = gun.gunPrice;
        weaponImage.sprite = gun.gunIcon;

        string isAuto = "Sim";
        if(!gun.isAutomatic)
            isAuto = "Não";

        weaponInfoText.text = "Automatico: "+ isAuto +
        "\nEnergia: "+ Helper.WattsNomenclature(weaponPrice) + 
        "\nDano: " + Mathf.Abs(gun.gunDamage) +
        "\nTaxa de tiro: " + gun.fireRate + " por segundo";

        if (SaveManager.Instance.state.gunIdUnlocked[gun.gunId])
        {
            buyWeaponText.text = "Equipar";
        }
        else
        {
            buyWeaponText.text = Helper.WattsNomenclature(weaponPrice);
        }
    }

    public void BuyWeapon()
    {
        int ID = currentWeaponID;
        Guns gun = gunVetor[ID];
        string weaponName = gun.gunName;
        float weaponPrice = gun.gunPrice;

        if (SaveManager.Instance.state.gunIdUnlocked[gun.gunId])
        {
            Debug.Log("Equipando");
            ShootController.Instance.EquipWeapon(gun);
            SaveManager.Instance.state.equipedGunId = gun.gunId;
        }
        else
        {
            Debug.Log("Comprando");
            if (SaveManager.Instance.state.wattsStored >= weaponPrice)
            {
                SaveManager.Instance.state.wattsStored -= weaponPrice;
                SaveManager.Instance.state.gunIdUnlocked[gun.gunId] = true;
                ShootController.Instance.EquipWeapon(gun);
                SaveManager.Instance.state.equipedGunId = gun.gunId;
            }
        }
    }

    #endregion
}
