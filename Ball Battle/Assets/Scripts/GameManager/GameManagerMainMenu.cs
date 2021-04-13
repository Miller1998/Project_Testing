using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerMainMenu : MonoBehaviour
{

    public void NextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitApps()
    {
        Application.Quit();
    }

}
