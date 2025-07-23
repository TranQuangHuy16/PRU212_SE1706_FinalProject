using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void PlayNewGame()
    {
        PlayerPrefs.DeleteKey("HasSaved");
        SceneManager.LoadScene("Game");
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {

        if (PlayerPrefs.GetInt("HasSaved", 0) == 1)
        {
            SceneManager.LoadScene("Game");
        }
    }
}
