using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Tooltip("¬рем€ удержани€ дл€ начала движени€ на мобильных устройствах (в секундах)")]
    public float holdTime = 0.2f;

    private RectTransform rectTransform;
    [SerializeField] private RectTransform parentRect;     // <- родительский RectTransform
    private Canvas canvas;
    private RectTransform canvasRectTransform;

    private Vector2 pointerStartLocalPos;
    private Vector2 playerStartAnchoredPos;

    private bool isPointerDown;
    private bool isTouchHold;
    private float pointerDownTime;

    private bool isPlayerFrozen = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // ѕолучаем RectTransform родител€
        if (transform.parent != null)
            parentRect = transform.parent.GetComponent<RectTransform>();
        else
            Debug.LogError("Player должен быть внутри какого-то RectTransform-контейнера!");

        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        else
            Debug.LogWarning("[PlayerController] Ќе найден Canvas в родител€х. ƒвижение может работать некорректно.");
    }

    public void FreezePlayer() 
    {
        isPlayerFrozen = true;
        Time.timeScale = 0f;
    }
public void UnfreezePlayer()
    {
        isPlayerFrozen = false;
        Time.timeScale = 1f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pointerDownTime = Time.unscaledTime;
        isTouchHold = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pointerStartLocalPos
        );
        playerStartAnchoredPos = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPlayerFrozen)
            return;

        // провер€ем hold дл€ тачей
        bool isTouch = eventData.pointerId >= 0;
        if (isTouch && !isTouchHold)
        {
            if (Time.unscaledTime - pointerDownTime >= holdTime)
                isTouchHold = true;
            else
                return;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPos
        );

        Vector2 offset = localPointerPos - pointerStartLocalPos;
        Vector2 targetPos = playerStartAnchoredPos + new Vector2(offset.x, offset.y);

        // ќграничиваем движение по границам родител€
        if (parentRect != null)
        {
            float halfParentW = parentRect.rect.width * 0.5f;
            float halfParentH = parentRect.rect.height * 0.5f;
            float halfSelfW = rectTransform.rect.width * 0.5f;
            float halfSelfH = rectTransform.rect.height * 0.5f;

            // clamp X
            targetPos.x = Mathf.Clamp(
                targetPos.x,
                -halfParentW + halfSelfW,
                 halfParentW - halfSelfW
            );

            // clamp Y (если нужно двигать и по вертикали)
            targetPos.y = Mathf.Clamp(
                targetPos.y,
                -halfParentH + halfSelfH,
                 halfParentH - halfSelfH
            );
        }

        rectTransform.anchoredPosition = targetPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        isTouchHold = false;
    }
}
