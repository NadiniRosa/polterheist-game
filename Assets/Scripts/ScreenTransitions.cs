using System.Collections;
using UnityEngine;

public class ScreenTransitions : MonoBehaviour
{
    public RectTransform circle;
    public float duration = 0.5f;

    void Start()
    {
        if (circle != null && circle.gameObject.activeSelf)
        {
            circle.localScale = Vector3.one * 20f; 
            StartShrink();
        }
    }

    public void StartExpand(System.Action onComplete)
    {
        StartCoroutine(Expand(onComplete));
    }

    public void StartShrink(System.Action onComplete = null)
    {
        StartCoroutine(Shrink(onComplete));
    }

    IEnumerator Expand(System.Action onComplete)
    {
        circle.gameObject.SetActive(true);

        float time = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one * 20f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            circle.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        onComplete?.Invoke();
    }

    IEnumerator Shrink(System.Action onComplete)
    {
        float time = 0f;
        Vector3 startScale = circle.localScale;
        Vector3 targetScale = Vector3.zero;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            circle.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        circle.gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}
