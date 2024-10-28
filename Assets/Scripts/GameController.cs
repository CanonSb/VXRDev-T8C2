using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Attachables")]
    public GameObject blackScreenPanel;
    public AttachPlayerToSled attachToSled;

    void Start()
    {
        // Set fog to max then lerp it down to create nice transition
        float ogDensity = RenderSettings.fogDensity;
        RenderSettings.fogDensity = 1f;
        StartCoroutine(LerpFogDensity(ogDensity));

        // Recenter player
        StartCoroutine(WaitAndRecenter(0.5f));
    }

    // Coroutine to lerp fog density
    private IEnumerator LerpFogDensity(float endDensity, float duration = 1f)
    {
        float timeElapsed = 0f;
        float startDensity = RenderSettings.fogDensity;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(timeElapsed / duration);

            RenderSettings.fogDensity = Mathf.Lerp(startDensity, endDensity, normalizedTime);
            yield return null;
        }

        // Ensure the final fog density is set
        RenderSettings.fogDensity = endDensity;
    }


    private IEnumerator WaitAndRecenter(float sec)
    {
        yield return new WaitForSeconds(sec);
        attachToSled.RecenterEasy();
    }

    public void OnGameLoss()
    {
        blackScreenPanel.SetActive(true);
        StartCoroutine(LoadScene("MainMenu", 3f));
    }

    private IEnumerator LoadScene(string sceneName, float sec)
    {
        yield return new WaitForSeconds(sec);
        SceneManager.LoadScene(sceneName);
    }
    
}
