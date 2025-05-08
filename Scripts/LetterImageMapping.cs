using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class LetterSpritePair
{
    public char letter;
    public Sprite sprite;
}
public class LetterImageMapping : MonoBehaviour
{
    public List<LetterSpritePair> letterSpritePairs; // —писок пар дл€ задани€ в инспекторе
    private Dictionary<char, Sprite> letterToImage = new Dictionary<char, Sprite>();

    private void Awake()
    {
        // «агрузка изображений в словарь из списка
        foreach (var pair in letterSpritePairs)
        {
            char lowerLetter = char.ToLower(pair.letter);
            char upperLetter = char.ToUpper(pair.letter);

            if (!letterToImage.ContainsKey(lowerLetter))
            {
                letterToImage[lowerLetter] = pair.sprite;
            }
            if (!letterToImage.ContainsKey(upperLetter))
            {
                letterToImage[upperLetter] = pair.sprite;
            }
        }
    }

    private List<char> missingLetters = new List<char>(); // ƒл€ хранени€ отсутствующих букв

    public Sprite GetSpriteForLetter(char letter)
    {
        char normalizedLetter = char.ToLower(letter);

        if (letterToImage.TryGetValue(normalizedLetter, out Sprite sprite))
        {
            return sprite;
        }
        else
        {
            Debug.LogError($"»зображение дл€ буквы '{letter}' не найдено.");
            return null;
        }
    }


    private void OnApplicationQuit()
    {
        if (missingLetters.Count > 0)
        {
            Debug.LogError("ќтсутствуют изображени€ дл€ следующих букв: " + string.Join(", ", missingLetters));
        }
    }

}