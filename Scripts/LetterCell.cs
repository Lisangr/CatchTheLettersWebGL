using UnityEngine;
using UnityEngine.UI;

public class LetterCell : MonoBehaviour
{
    public Image letterImage;
    [HideInInspector] public char letter;
    public float fallSpeed = 20f;

    private UIShake uiShake;
    private RectTransform rt;
    private LetterFallManager wordManager;
    private HealthManager healthManager;
    private CameraShake cameraShake;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        wordManager = FindObjectOfType<LetterFallManager>();
        healthManager = FindObjectOfType<HealthManager>();
        cameraShake = FindObjectOfType<CameraShake>().GetComponent<CameraShake>();
        uiShake = FindObjectOfType<UIShake>();
        if (uiShake == null)
            Debug.LogError("[LetterCell] UIShake not found on any GameObject!");

        if (wordManager == null) Debug.LogError("No LetterFallManager!");
        if (healthManager == null) Debug.LogError("No HealthManager!");
        if (cameraShake == null) Debug.LogError("No CameraShake on MainCamera!");
    }

    void Update()
    {
        if (isPlayerFrozen)  // Проверяем флаг
            return;

        rt.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;
    }
    private bool isPlayerFrozen = false;  // Добавьте этот флаг

    public void FreezeMovement()
    {
        isPlayerFrozen = true;  // Замораживаем движение игрока
    }

    public void UnfreezeMovement()
    {
        isPlayerFrozen = false;  // Размораживаем движение игрока
    }
    public void SetLetterImage(Sprite sprite)
    {
        if (sprite == null) return;
        letterImage.sprite = sprite;
        letter = sprite.name.Length > 0
                 ? sprite.name[0]
                 : letter;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Catcher")) return;

        // определяем пойманную букву
        Sprite sprite = letterImage.sprite;
        string name = sprite != null ? sprite.name : "";
        char raw = name.Length > 0 ? name[0] : letter;
        char caught = char.ToLowerInvariant(raw);

        Debug.Log($"[LetterCell] Caught '{caught}'");

        // проверяем слово
        bool correct = wordManager.HandleCaughtLetter(caught);

        if (!correct)
        {
            healthManager.ApplyDamage();
            cameraShake.Shake();
            uiShake.Shake();
        }
        Destroy(gameObject);
    }
}
