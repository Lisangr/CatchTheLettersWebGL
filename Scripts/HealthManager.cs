using UnityEngine;
using YG;

public class HealthManager : MonoBehaviour
{
    [Header("Lives UI")]
    public GameObject[] heartIcons;
    public GameObject losePanel;

    [Header("Audio")]
    public AudioClip correctClip;
    public AudioClip wrongClip;
    private AudioSource audioSource;

    private int lives;
    private int AdID = 1; // дубль награды

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D-звук
    }

    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += Rewarded;
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

    public void Rewarded(int id)
    {
        if (id != AdID) return;
        AddLives();
    }

    private void AddLives()
    {
        lives = heartIcons.Length;
        foreach (var h in heartIcons)
            h.SetActive(true);

        losePanel.SetActive(false);
        FindObjectOfType<LetterFallManager>().PlaySpawning();
        FindObjectOfType<PlayerMovement>().UnfreezePlayer();
    }

    /// <summary>
    /// Вызывается при сборе буквы.
    /// isCorrect == true  → верная буква (correctClip)
    /// isCorrect == false → неверная буква (wrongClip + потеря жизни)
    /// </summary>
    public void ApplyDamage(bool isCorrect)
    {
        // 1) Проигрываем звук
        AudioClip clip = isCorrect ? correctClip : wrongClip;
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"HealthManager: {(isCorrect ? "correctClip" : "wrongClip")} не назначен!");
        }

        // 2) Если буква неверная — снимаем жизнь и проверяем Game Over
        if (!isCorrect)
        {
            if (lives <= 0) return;

            lives--;
            if (lives >= 0 && lives < heartIcons.Length)
                heartIcons[lives].SetActive(false);

            if (lives <= 0)
            {
                losePanel.SetActive(true);
                FindObjectOfType<LetterFallManager>().StopSpawning();
                FindObjectOfType<PlayerMovement>().FreezePlayer();
            }
        }
    }
}
