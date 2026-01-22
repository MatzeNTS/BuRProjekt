using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void OnStartButton()
    {
        SceneManager.LoadScene(1);  //GameplayScene wird geladen
    }

    public void OnBeendenButton()
    {
        Application.Quit();
    }
}
