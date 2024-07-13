using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Options()
    {
        SceneManager.LoadScene(2);
    }

    public void Exit()
    {
        Application.Quit();
    }
}

