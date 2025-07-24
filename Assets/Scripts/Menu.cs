using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;


    void Start()
    {

        if (!PlayerPrefs.HasKey("HasSaved") || PlayerPrefs.GetInt("HasSaved") != 1)
        {
            continueButton.SetActive(false);
        }
    }

    public void PlayNewGame()
    {
        PlayerPrefs.DeleteAll();
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
