using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Manages start menu.
/// </summary>
public class StartMenuManager : MonoBehaviour
{
    [SerializeField] Image fadeOutImage;
    [SerializeField] TextMeshProUGUI fadingTitleText;

    AsyncOperation asyncLoad;
    Color titleTextcolor;

    private void Start() 
    {
        // start async scene loading
        asyncLoad = SceneManager.LoadSceneAsync("Game");
        asyncLoad.allowSceneActivation = false;   // but set it to stop at 90%
    }

    public void OnStartButtonPressed()
    {
        titleTextcolor = fadingTitleText.color;
        GameManager.Instance.SceneLoadedFromStart = true;
        StartCoroutine(FadeOutScene());
    }

    /// <summary>
    /// Fades out the screen and the title and then loads next scene.
    /// </summary>
    private IEnumerator FadeOutScene()
    {
        float alpha = 0;

        AudioManager.FadeOutMusic(0, 0, 3, 0);

        // fade screen to black, except title text
        while (alpha < 1)
        {
            alpha += 0.02f;
            Color color = new Color(0f, 0f, 0f, alpha);

            fadeOutImage.color = color;

            yield return new WaitForSecondsRealtime(0.01f);
        }

        alpha = 1;
        // fade the text after scene is dark, for visual candy
        while (alpha > 0)
        {
            alpha -= 0.06f;
            Color color = new Color(titleTextcolor.r, titleTextcolor.g, titleTextcolor.b, alpha);

            fadingTitleText.color = color;

            yield return null;
        }

        // finally when we're done, load next scene
        AudioManager.StopMusic(0);
        asyncLoad.allowSceneActivation = true;
    }
}

