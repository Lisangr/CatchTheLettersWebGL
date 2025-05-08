using UnityEngine;
using YG;

public class HealthManager : MonoBehaviour
{
    [Header("Lives UI")]
    public GameObject[] heartIcons;
    public GameObject losePanel;

    private int lives;
    private int AdID = 1; //дубль награды
    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += Rewarded;
    }

    public void Rewarded(int id)
    {
        if (id != AdID) return; // Игнорируем события с другим ID

        AddLives();
    }
    private void AddLives()
    {
        // Восстановить количество жизней до максимума
        lives = heartIcons.Length;

        // Обновить отображение жизней в UI
        foreach (var h in heartIcons)
        {
            h.SetActive(true); // Сделать все иконки видимыми
        }

        // Закрыть панель "Game Over", если она была открыта
        losePanel.SetActive(false);

        FindObjectOfType<LetterFallManager>().PlaySpawning();
        FindObjectOfType<PlayerMovement>().UnfreezePlayer();
        //FindObjectOfType<LetterCell>().UnfreezeMovement();
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= Rewarded;
        Time.timeScale = 1f;
    }
    void Start()
    {
        lives = heartIcons.Length;
        foreach (var h in heartIcons) h.SetActive(true);
        losePanel.SetActive(false);
    }

    /// <summary>Снимает одну жизнь, обновляет UI и проверяет Game Over</summary>
    public void ApplyDamage()
    {
        if (lives <= 0) return;

        lives--;
        if (lives >= 0 && lives < heartIcons.Length)
            heartIcons[lives].SetActive(false);

        if (lives <= 0)
        {
            losePanel.SetActive(true);
            // Остановите спавн и заморозьте игрока
            FindObjectOfType<LetterFallManager>().StopSpawning();
            FindObjectOfType<PlayerMovement>().FreezePlayer();
            //FindObjectOfType<LetterCell>().FreezeMovement();
        }
    }
}
