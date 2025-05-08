using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIShake : MonoBehaviour
{
    public float duration = 0.2f;
    public float magnitude = 8f; // в пикселях

    private RectTransform rt;
    private Vector2 originalPos;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        originalPos = rt.anchoredPosition;
    }

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(DoShake());
    }

    private IEnumerator DoShake()
    {
        Debug.Log("[UIShake] DoShake() start");
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            rt.anchoredPosition = originalPos + new Vector2(x, y);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rt.anchoredPosition = originalPos;
        Debug.Log("[UIShake] DoShake() end");
    }
}
