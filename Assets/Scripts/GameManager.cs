
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameWinUI;
    [SerializeField] private GameObject pauseGameUI;
    private int score = 0;
    private bool isGameWin = false;
    private bool isPaused = false;
    public Transform player;
    void Start()
    {
        if (PlayerPrefs.GetInt("HasSaved", 0) == 1)
        {
            score = PlayerPrefs.GetInt("Score", 0);
        }

        UpdateScoreText(); // cập nhật hiển thị
        gameWinUI.SetActive(false);
        pauseGameUI.SetActive(false);
        Time.timeScale = 1;
    }

    void Update()
    {
        // Nhấn phím Esc để bật/tắt pause menu
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameWin)
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                Time.timeScale = 0;
                pauseGameUI.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                pauseGameUI.SetActive(false);
            }
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }
    public void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }


    public void GameWin()
    {
        isGameWin = true;
        Time.timeScale = 0;
        gameWinUI.SetActive(true);
    }

    public void RestartGame()
    {
        isGameWin = false;
        score = 0;
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;
        SaveCheckpoint();
        SceneManager.LoadScene("Menu");

    }

    public void Continue()
    {
        isPaused = false;
        pauseGameUI.SetActive(false);
        Time.timeScale = 1;
    }

    public bool IsGameWin()
    {
        return isGameWin;
    }

    private void SaveCheckpoint()
    {
        PlayerPrefs.SetFloat("PlayerX", player.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.position.y);
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("HasSaved", 1);
        PlayerPrefs.Save();
    }
}
