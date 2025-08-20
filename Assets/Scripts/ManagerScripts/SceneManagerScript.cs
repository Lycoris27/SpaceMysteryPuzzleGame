using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public void LoadingScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
