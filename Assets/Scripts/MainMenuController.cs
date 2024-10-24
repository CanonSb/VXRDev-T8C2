using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartGame()
    {
        StartCoroutine(SwitchScenes());
    }


    public IEnumerator SwitchScenes()
    {
        // Disable sign text
        GameObject.FindGameObjectWithTag("signText").SetActive(false);
        // Start the fog lerp coroutine and wait for it to finish
        yield return StartCoroutine(LerpFogDensity(1f));
        SceneManager.LoadScene("Slope");
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
}