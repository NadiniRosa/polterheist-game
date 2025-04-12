using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public RectTransform circle;

    public float expandTime = 0.5f;

    public string gameSceneName = "Game";

    private bool isTransitioning = false;

    public void PlayGame()
    {
        if (!isTransitioning)
        {
            StartCoroutine(ExpandAndLoad());
        }
    }

    IEnumerator ExpandAndLoad()
    {
        isTransitioning = true;

        float time = 0f;
        Vector3 startScale = circle.localScale;
        Vector3 targetScale = Vector3.one * 18f;

        while (time < expandTime)
        {
            time += Time.deltaTime;
            float t = time / expandTime;

            circle.gameObject.SetActive(true);
            circle.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        SceneManager.LoadScene(gameSceneName);
    }
}
