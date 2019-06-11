using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartAndEndGame : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void SceneLoader(int scene)
    {
        if (!SaveManager.Instance.state.firstStartOcurred)
        {
            SceneManager.LoadScene("Tutorial");
        }
        else if (SaveManager.Instance.state.endGameOcurred)
        {
            SceneManager.LoadScene("CongratsForConclusion")
;
        }
        else if (SaveManager.Instance.state.firstStartOcurred && !SaveManager.Instance.state.endGameOcurred)
        {
            if (scene == 1)
            {
                SaveManager.Instance.Load();
            }
            SceneManager.LoadScene(scene);
        }
    }

    public void SimpleSceneLoader(int sceneIndex)
    {
        SaveManager.Instance.Load();
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quit.");
        Application.Quit();
    }

    public void ResetSave()
    {
        SaveManager.Instance.ResetSave();
        SaveManager.Instance.Load();
    }
}
