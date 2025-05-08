using System.Collections;
using UnityEngine;

public class Destroer : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DestroyCor());
    }

    private IEnumerator DestroyCor()
    {
        yield return new WaitForSeconds(9f);
        Destroy(this.gameObject);
    }
}
