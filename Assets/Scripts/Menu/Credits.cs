using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("CreditRoll");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            GoBackToMenu();
        }
    }

    void GoBackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator CreditRoll()
    {
        yield return new WaitForSecondsRealtime(15f);
        GoBackToMenu();
    }
}
