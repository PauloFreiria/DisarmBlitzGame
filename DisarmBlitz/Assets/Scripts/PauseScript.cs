using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    [SerializeField] private GameObject pauseScreen;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pauseScreen.gameObject.SetActive(true);
        }
    }

    public void Resume()
    {
        pauseScreen.gameObject.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
