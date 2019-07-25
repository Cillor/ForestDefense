using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneScene : MonoBehaviour
{
    Slider slider;

    private void Awake()
    {
        slider = FindObjectOfType<Slider>();
    }

    void Start()
    {
        StartCoroutine("LoadSceneProgress");
    }

    IEnumerator LoadSceneProgress()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(2);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            slider.value = progress;

            yield return null;
        }
    }
}
