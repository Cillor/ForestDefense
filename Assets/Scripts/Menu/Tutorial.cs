using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    void Start()
    {
        //Play Tutorial
    }

    void Update()
    {
        JumpTutorial();
    }

    void JumpTutorial()
    {
        if (Input.GetButtonDown("Jump"))
        {
            SceneManager.LoadScene(1);
            SaveManager.Instance.state.firstStartOcurred = true;
        }
    }
}
