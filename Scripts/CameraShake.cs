using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Tooltip("������������ ������ � ��������")]
    public float duration = 0.2f;
    [Tooltip("��������� ��������")]
    public float magnitude = 0.2f;

    private Coroutine shakeCoroutine;

    /// <summary>
    /// ��������� ������ ������.
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
        // ���� ��������� ������ ��� ��������
        Vector3 originalPos = transform.position;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.position = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ���������� � �������� ���������
        transform.position = originalPos;
        Debug.Log("[CameraShake] DoShake() end");
    }
}
