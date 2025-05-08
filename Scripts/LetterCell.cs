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
        if (isPlayerFrozen)  // ��������� ����
            return;

        rt.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;
    }
    private bool isPlayerFrozen = false;  // �������� ���� ����

    public void FreezeMovement()
    {
        isPlayerFrozen = true;  // ������������ �������� ������
    }

    public void UnfreezeMovement()
    {
        isPlayerFrozen = false;  // ������������� �������� ������
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

        // ���������� ��������� �����
        Sprite sprite = letterImage.sprite;
        string name = sprite != null ? sprite.name : "";
        char raw = name.Length > 0 ? name[0] : letter;
        char caught = char.ToLowerInvariant(raw);

        Debug.Log($"[LetterCell] Caught '{caught}'");

        // ��������� �����
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
