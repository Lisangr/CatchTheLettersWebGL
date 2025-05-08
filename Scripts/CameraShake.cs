using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Tooltip("Длительность тряски в секундах")]
    public float duration = 0.2f;
    [Tooltip("Амплитуда смещения")]
    public float magnitude = 0.2f;

    private Coroutine shakeCoroutine;

    /// <summary>
    /// Запускает тряску камеры.
    /// </summary>
    public void Shake()
    {
        Debug.Log("[CameraShake] Shake() called");
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(DoShake());
    }

    private IEnumerator DoShake()
    {
        Debug.Log("[CameraShake] DoShake() start");
        float elapsed = 0f;
        // Берём положение камеры как исходное
        Vector3 originalPos = transform.position;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.position = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Возвращаем в исходное положение
        transform.position = originalPos;
        Debug.Log("[CameraShake] DoShake() end");
    }
}
