using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIElements : MonoBehaviour
{
    public TMP_Text batterys, gunRange, wattsStored, metals; /*, coffeeAddic;*/
    public Image stamina, poison, eyesClosing, coffeeAddic, coffeeQnt;

    public AnimationCurve staminaEffect;
    float watts;

    void Start()
    {
        
    }

    void Update()
    {
        
        wattsStored.text = Helper.WattsNomenclature(SaveManager.Instance.state.wattsStored);
        batterys.text = "" + SaveManager.Instance.state.batterysInInventory;
        metals.text = "" + SaveManager.Instance.state.metalAmount;
        stamina.fillAmount = SaveManager.Instance.state.stamina / 100f;
        poison.fillAmount = SaveManager.Instance.state.poison / 1f;
        coffeeAddic.fillAmount = Helper.Round(SaveManager.Instance.state.coffeeAddiction, 2);
        coffeeQnt.fillAmount = SaveManager.Instance.state.coffeeQuantity / 10f;
        //coffeeAddic.text = "Coffees: " + SaveManager.Instance.state.coffeeQuantity + "/ Addic: " + Helper.Round(SaveManager.Instance.state.coffeeAddiction, 2);
        eyesClosing.color = new Color(255, 255, 255, staminaEffect.Evaluate(SaveManager.Instance.state.stamina) / 100);
    }
}
