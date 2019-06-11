using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { set; get; }
    public SaveState state;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        
        Load();
    }

    public void Save()
    {
        if (!WaveManager.isWaveActive)
            PlayerPrefs.SetString("save", Helper.Serialize<SaveState>(state));
        else
        {
            StartCoroutine("PopUp");
        }
    }

    IEnumerator PopUp()
    {
        FindObjectOfType<ScreenController>().notification.text = "Você não pode salvar o jogo durante uma onda.";
        FindObjectOfType<ScreenController>().notification.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        FindObjectOfType<ScreenController>().notification.gameObject.SetActive(false);
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("save"))
        {
            state = Helper.Deserialize<SaveState>(PlayerPrefs.GetString("save"));
        }
        else
        {
            state = new SaveState();
            state.gunIdUnlocked[0] = true;
            state.waveUnlocked[0] = true;
            Save();
            Debug.Log("No save file... New one created.");
        }

    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("save");
    }
}
