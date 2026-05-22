using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
 
public class ScreenGlitchEffect : MonoBehaviour
{
    [Header("References")]
    public RawImage glitchOverlay; // The fullscreen RawImage with the render texture
    public VideoPlayer videoPlayer; // The Video Player component
    public RectTransform overlayRect; // The RectTransform of the GlitchOverlay
    public RectTransform canvasRect;
 
    [Header("Timings")]
    public float glitchDuration = 3f; // How long the glitch video plays before CRT effect
    public float crtShrinkDuration = 0.5f; // How long the CRT shrink-to-line takes
    public float lingerDuration = 0.3f; // How long the line stays before going black
    public float blackDuration = 0.5f; // How long black screen holds before next scene
 
    private Vector2 originalSize;
 
    void Start()
    {
        // Hide overlay at start
        glitchOverlay.gameObject.SetActive(false);
        originalSize = canvasRect.sizeDelta;
    }
 
    // Called by IntroCutsceneManager after dialogue 2 ends
 
    public void StartGlitchSequence()
    {
        Debug.Log("StartGlitchSequence called!");
        StartCoroutine(GlitchSequence());
    }

    IEnumerator GlitchSequence()
    {
        glitchOverlay.gameObject.SetActive(true);
        videoPlayer.Play();

        // Fade glitch overlay from transparent to opaque
        float elapsed = 0f;
        Color c = glitchOverlay.color;
        while (elapsed < glitchDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / glitchDuration);
            glitchOverlay.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
        glitchOverlay.color = new Color(c.r, c.g, c.b, 1f);

        // Hide everything underneath — player can't see it since overlay is fully opaque
        Transform canvas = FindObjectOfType<IntroCutsceneManager>().transform;
        foreach (Transform child in transform)
        {
            if(child.gameObject.name != "GlitchOverlay")
            {
                child.gameObject.SetActive(false);
            }
        }

        // CRT squish — Y collapses to a thin line
        elapsed = 0f;
        Vector3 originalScale = overlayRect.localScale;
        float half = crtShrinkDuration * 0.5f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Pow(elapsed / half, 2f);
            float newScaleY = Mathf.Lerp(1f, 0.005f, t);
            overlayRect.localScale = new Vector3(originalScale.x, newScaleY, originalScale.z);
            yield return null;
        }

        // Hold the line briefly
        yield return new WaitForSeconds(lingerDuration);

        // Then X collapses to nothing
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Pow(elapsed / half, 2f);
            float newScaleX = Mathf.Lerp(1f, 0f, t);
            overlayRect.localScale = new Vector3(newScaleX, 0.005f, originalScale.z);
            yield return null;
        }

        overlayRect.localScale = Vector3.zero;
        yield return new WaitForSeconds(blackDuration);

        SceneManager.LoadScene("Tutorial1");
    }
}