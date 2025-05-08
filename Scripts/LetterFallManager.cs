using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(LetterImageMapping))]
public class LetterFallManager : MonoBehaviour
{
    [Header("Canvas & Spawn Area")]
    [Tooltip("Корневой RectTransform вашего Canvas")]
    public RectTransform canvasRect;
    [Tooltip("Дополнительный контейнер (можно тот же canvasRect) — в пределах него будут спавниться буквы")]
    public RectTransform spawnAreaRect;

    [Header("Level Settings")]
    [Tooltip("1–5 → 3 слова, 6–10 → 4, 11–15 → 5, 16–20 → 6, 21–30 → 7, 31–40 → 8 и т.д.")]
    public int difficultyLevel = 3;

    [Header("Spawn Settings")]
    public RectTransform[] spawnPoints;
    public GameObject letterPrefab;
    public float spawnInterval = 1.0f;

    [Header("UI Elements")]
    public Text currentWordText;
    public Text caughtLettersText;
    public Text wordsCountText;

    [Header("Lives UI")]
    [Tooltip("Иконки ваших сердец (3 шт) в порядке от первой к последней")]
    public GameObject[] heartIcons;
    public GameObject losePanel;
    public GameObject winPanel;

    private int lives;
    private LetterImageMapping letterImageMapping;
    private List<string> wordList;
    private int currentWordIndex = 0;
    private string currentWord;
    private List<char> lettersToSpawn;
    private List<char> caughtThisWord = new List<char>();

    // Списки букв для каждого языка
    private List<char> lettersRU = new List<char> {
    'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я'
};
    private List<char> lettersEN = new List<char> {
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
};
    void Awake()
    {
        // Кэшируем компонент, чтобы не вызывать GetComponent каждый раз
        letterImageMapping = GetComponent<LetterImageMapping>();
    }

    void Start()
    {
        // Проверяем корректность настроек спавна
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn Points не заданы в инспекторе!");
            enabled = false;
            return;
        }

        if (letterPrefab == null)
        {
            Debug.LogError("Letter Prefab не привязан в инспекторе!");
            enabled = false;
            return;
        }

        if (letterImageMapping == null)
        {
            Debug.LogError("На объекте нет компонента LetterImageMapping!");
            enabled = false;
            return;
        }

        // Отбираем слова для текущего уровня
        SetupWordListByLevel();

        lives = heartIcons.Length;
        foreach (var h in heartIcons) h.SetActive(true);
        losePanel.SetActive(false);
        winPanel.SetActive(false);

        caughtThisWord = new List<char>();

        UpdateWordsCountUI();
        NextWord();
        StartCoroutine(SpawnLettersLoop());
    }

    private void SetupWordListByLevel()
    {
        if (YandexGame.EnvironmentData.language == "ru")
        {
            var all = new List<string>(WordLists.LettersRUWords);
            int max = GetMaxWordsForLevel(difficultyLevel);
            Shuffle(all);
            wordList = all.Take(Mathf.Min(max, all.Count)).ToList();
        }
        else if (YandexGame.EnvironmentData.language == "en")
        {
            var all = new List<string>(WordLists.LettersENWords);
            int max = GetMaxWordsForLevel(difficultyLevel);
            Shuffle(all);
            wordList = all.Take(Mathf.Min(max, all.Count)).ToList();
        }
        else
        {
            Debug.LogError("ЧЕТ НЕ ТО ПРОИСХОДИТ С ЯЗЫКОМ");
        }
    }
    /// <summary>
    /// Возвращает максимальное число слов для данного уровня.
    /// 1–5 → 3, 6–10 → 4, 11–15 → 5, 16–20 → 6, 21–30 → 7, 31–40 → 8, …
    /// </summary>
    private int GetMaxWordsForLevel(int level)
    {
        if (level <= 5) return 3;
        if (level <= 10) return 4;
        if (level <= 15) return 5;
        if (level <= 20) return 6;
        // для уровней >20: на каждые следующие 10 уровней +1 слово, начиная с 7
        // (21–30 → 7, 31–40 → 8, 41–50 → 9 и т.д.)
        return 7 + ((level - 1 - 20) / 10);
    }
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
    private void NextWord()
    {
        caughtThisWord.Clear();
        caughtLettersText.text = "";

        if (currentWordIndex >= wordList.Count)
        {
            winPanel.SetActive(true);
            return;
        }

        currentWord = wordList[currentWordIndex++].Trim().ToLowerInvariant();
        Debug.Log($"[Manager] New currentWord = '{currentWord}'");
        currentWordText.text = currentWord.ToUpperInvariant();

        UpdateWordsCountUI();
    }

    /// <summary>
    /// Проверяет пойманную букву: если она есть в слове и новая — возвращает true,
    /// иначе false.
    /// </summary>
    /// </summary>
    public bool HandleCaughtLetter(char letter)
    {
        if (winPanel.activeSelf)
            return true;

        char norm = char.ToLowerInvariant(letter);

        // 1) Буква вообще есть в слове?
        bool isInWord = currentWord.IndexOf(norm) >= 0;
        // 2) Сколько всего таких букв в слове?
        int totalCount = currentWord.Count(c => c == norm);
        // 3) Сколько уже поймали?
        int caughtCount = caughtThisWord.Count(c => c == norm);

        Debug.Log($"[Manager] Letter='{norm}', inWord={isInWord}, caught={caughtCount}/{totalCount}");

        // Разрешаем «поймать» букву, пока не набрали полный комплект
        if (isInWord && caughtCount < totalCount)
        {
            caughtThisWord.Add(norm);
            caughtLettersText.text = string.Concat(caughtThisWord)
                                         .ToUpperInvariant();

            // если слово собрано — следующее
            if (caughtThisWord.Count == currentWord.Length)
                NextWord();

            return true;
        }

        // иначе — неверная или «лишняя» третья (четвёртая) буква
        return false;
    }
    private bool isGameOver = false;  // Добавьте этот флаг

    private IEnumerator SpawnLettersLoop()
    {
        while (!isGameOver)  // Добавьте проверку флага
        {
            SpawnOneLetter();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void StopSpawning()
    {
        isGameOver = true;  // Устанавливаем флаг, чтобы остановить спавн
    }

    public void PlaySpawning()
    {
        isGameOver = false;  // Устанавливаем флаг, чтобы остановить спавн
        StartCoroutine(SpawnLettersLoop());
    }
    private void SpawnOneLetter()
    {
        if (canvasRect == null || spawnPoints == null || spawnPoints.Length == 0 || letterPrefab == null)
        {
            Debug.LogError("Missing refs!");
            return;
        }

        RectTransform spawnRT = spawnPoints[Random.Range(0, spawnPoints.Length)] as RectTransform;
        if (spawnRT == null) { Debug.LogError("spawnPoint не RectTransform!"); return; }

        // 1) Создаём букву под canvas
        GameObject go = Instantiate(letterPrefab, canvasRect);
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) { Debug.LogError("prefab без RectTransform"); Destroy(go); return; }

        // 2) Преобразуем позицию spawnRT в локальные координаты canvasRect
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, spawnRT.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenP, null, out Vector2 localPoint
        );
        rt.anchoredPosition = localPoint;

        // 3) Выбираем букву и устанавливаем спрайт
        char letter = PickLetter();  // Выбираем букву на основе языка
        var cell = go.GetComponent<LetterCell>();
        cell.SetLetterImage(letterImageMapping.GetSpriteForLetter(letter));  // Устанавливаем спрайт для буквы
        cell.letter = letter;
    }

    private char PickLetter()
    {
        List<char> letters = new List<char>();

        // В зависимости от языка выбираем нужный список букв
        if (YandexGame.EnvironmentData.language == "ru")
        {
            letters = lettersRU;
        }
        else if (YandexGame.EnvironmentData.language == "en")
        {
            letters = lettersEN;
        }
        else
        {
            Debug.LogError("Невозможно определить язык!");
        }

        if (letters.Count > 0)
        {
            int idx = Random.Range(0, letters.Count);
            char c = letters[idx];
            return c;
        }

        return 'А'; // по умолчанию возвращаем 'А', если нет букв (на всякий случай)
    }

    private void UpdateWordsCountUI()
    {
        int total = wordList.Count;
        int done = Mathf.Clamp(currentWordIndex, 1, total);
        wordsCountText.text = $"{done}/{total}";
    }
}