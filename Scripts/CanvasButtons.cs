using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class CanvasButtons : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;

    private int currentLevelIndex;
    private int newPointsRezult;

   
    private void Awake()
    {
        if (winPanel != null)
        winPanel.SetActive(false);

        if (losePanel != null)
        losePanel.SetActive(false);

        if (PlayerPrefs.HasKey("Level"))
        {
            currentLevelIndex = PlayerPrefs.GetInt("Level");
        }
        else
        {
            currentLevelIndex = 1; // ������������� ������� �� ���������
            PlayerPrefs.SetInt("Level", currentLevelIndex);
            PlayerPrefs.Save();
        }
    }

    public void OnStartButtonClick()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            currentLevelIndex = PlayerPrefs.GetInt("Level");
        }

        // �������� ������� ������� � LetterFallManager
        LetterFallManager letterFallManager = FindObjectOfType<LetterFallManager>();
        if (letterFallManager != null)
        {
            letterFallManager.difficultyLevel = currentLevelIndex; // ������������� �������
        }
        SceneManager.LoadScene("MainScene");
    }

    public void OnGoToMenuButton()
    {
        YandexGame.FullscreenShow();
        SceneManager.LoadScene("MenuScene"); // �������� "MenuScene" �� ��� ����� ����� ����
    }
    public void OnExitButtonClick()
    {
        YandexGame.FullscreenShow();

        newPointsRezult = currentLevelIndex + 1;
        YandexGame.NewLeaderboardScores("Levels", newPointsRezult);        
        PlayerPrefs.SetInt("Level", newPointsRezult);
        PlayerPrefs.Save();     

        // ������� �� ����� ����
        SceneManager.LoadScene("MenuScene"); // �������� "MenuScene" �� ��� ����� ����� ����
    }
    public void OnNextButtonClick()
    {
        YandexGame.FullscreenShow();

        // ��������� �������
        newPointsRezult = currentLevelIndex + 1;
        YandexGame.NewLeaderboardScores("Levels", newPointsRezult);

        // ��������� ����� ������� � PlayerPrefs
        PlayerPrefs.SetInt("Level", newPointsRezult);
        PlayerPrefs.Save();

        // ��������� ��������� ������� � ��� ���������� ���������� Level
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void RestartCurrentLevel()
    {
        YandexGame.FullscreenShow();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }  
}