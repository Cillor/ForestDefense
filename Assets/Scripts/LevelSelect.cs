using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public TMP_Text selectUnlockText;
    string select = "Selecionar";
    string locked = "bloqueada";

    int waveSelected;

    private void Start()
    {
        ConfirmText();
    }

    public void ClickWave(int waveNumber)
    {
        waveSelected = waveNumber;
        ConfirmText();
    }

    public void ConfirmText()
    {
        if (SaveManager.Instance.state.waveUnlocked[waveSelected])
        {
            selectUnlockText.text = select + " a onda " + waveSelected;
        }
        else
        {
            selectUnlockText.text = "Onda " + waveSelected +" está " + locked;
        }
    }

    public void OnClickSelect()
    {
        if (SaveManager.Instance.state.waveUnlocked[waveSelected])
        {
            Debug.Log("Wave " + waveSelected + " iniciada");
            SaveManager.Instance.state.actualWave = waveSelected;
        }
    }
}
