using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{

    public void StartHeadingGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
    public void StartCoolingGame()
    {
        SceneManager.LoadSceneAsync(2);
    }
    public void StartPlayerGame()
    {
        SceneManager.LoadSceneAsync(3);
    }
    public void ResetGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    public void ToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
  
    

