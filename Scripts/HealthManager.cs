using UnityEngine;
using YG;

public class HealthManager : MonoBehaviour
{
    [Header("Lives UI")]
    public GameObject[] heartIcons;
    public GameObject losePanel;

    private int lives;
    private int AdID = 1; //����� �������
    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += Rewarded;
    }

    public void Rewarded(int id)
    {
        if (id != AdID) return; // ���������� ������� � ������ ID

        AddLives();
    }
    private void AddLives()
    {
        // ������������ ���������� ������ �� ���������
        lives = heartIcons.Length;

        // �������� ����������� ������ � UI
        foreach (var h in heartIcons)
        {
            h.SetActive(true); // ������� ��� ������ ��������
        }

        // ������� ������ "Game Over", ���� ��� ���� �������
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

    /// <summary>������� ���� �����, ��������� UI � ��������� Game Over</summary>
    public void ApplyDamage()
    {
        if (lives <= 0) return;

        lives--;
        if (lives >= 0 && lives < heartIcons.Length)
            heartIcons[lives].SetActive(false);

        if (lives <= 0)
        {
            losePanel.SetActive(true);
            // ���������� ����� � ���������� ������
            FindObjectOfType<LetterFallManager>().StopSpawning();
            FindObjectOfType<PlayerMovement>().FreezePlayer();
            //FindObjectOfType<LetterCell>().FreezeMovement();
        }
    }
}
