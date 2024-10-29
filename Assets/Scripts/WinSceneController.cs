using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class WinSceneController : MonoBehaviour
{
    private GameObject signText;

    // Start is called before the first frame update
    void Start()
    {
        signText = GameObject.FindGameObjectWithTag("signText");
        signText.SetActive(false);
        InitializeFog();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GoToMainMenu()
    {
        StartCoroutine(SwitchScenes("MainMenu"));
    }

    public void RestartGame()
    {
        StartCoroutine(SwitchScenes("Slope"));
    }

    
    private void InitializeFog()
    {
        // Set fog to max then lerp it down to create nice transition
        float ogDensity = RenderSettings.fogDensity;
        RenderSettings.fogDensity = 1f;
        StartCoroutine(LerpFogDensity(ogDensity, 1.5f));
        StartCoroutine(TurnSignOn(1.2f));
    }

    private IEnumerator TurnSignOn(float sec)
    {
        yield return new WaitForSeconds(sec);
        signText.SetActive(true);
    }

    public IEnumerator SwitchScenes(string sceneName)
    {
        // Disable sign text
        GameObject.FindGameObjectWithTag("signText").SetActive(false);
        // Start the fog lerp coroutine and wait for it to finish
        yield return StartCoroutine(LerpFogDensity(1f, 1.5f));
        SceneManager.LoadScene(sceneName);
    }

    // Coroutine to lerp fog density
    private IEnumerator LerpFogDensity(float endDensity, float duration = 1f)
    {
        float timeElapsed = 0f;

        ColorUtility.TryParseHtmlString("#D7EEFF", out Color fogColor);
        Color camStartColor = Camera.main.backgroundColor;
        float startDensity = RenderSettings.fogDensity;
        Color startColor = RenderSettings.fogColor;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(timeElapsed / duration);

            Camera.main.backgroundColor = Color.Lerp(camStartColor, fogColor, normalizedTime);
            RenderSettings.fogDensity = Mathf.Lerp(startDensity, endDensity, normalizedTime);
            RenderSettings.fogColor = Color.Lerp(startColor, fogColor, normalizedTime);
            yield return null;
        }

        // Ensure the final fog density is set
        Camera.main.backgroundColor = fogColor;
        RenderSettings.fogDensity = endDensity;
    }
}
