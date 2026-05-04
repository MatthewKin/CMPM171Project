using UnityEngine;
using TMPro;
using System.Collections;

public class TextAnim : MonoBehaviour
{
    [Header("Typing Settings")]
    public float typeSpeed = 0.05f;
    public float delayBeforeStart = 0f;

    [Header("Glitch Settings")]
    public bool useGlitch = true;
    public float glitchDuration = 0.4f;
    public float flickerSpeed = 0.05f;
    public float shakeMagnitude = 5f;

    [Header("Fade Settings")]
    public float delayBeforeFade = 5f;
    public float fadeDuration = 1.5f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip glitchSound;

    private TextMeshProUGUI tmpText;
    private string fullText;
    private Vector3 originalPosition;
    private Vector3 originalScale;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        fullText = tmpText.text;
        tmpText.text = "";
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(PlayEffect());
    }

    IEnumerator PlayEffect()
    {
        if (delayBeforeStart > 0f)
            yield return new WaitForSeconds(delayBeforeStart);

        foreach (char c in fullText)
        {
            tmpText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        
        yield return new WaitForSeconds(delayBeforeFade);

        if (useGlitch)
        {
            yield return StartCoroutine(GlitchRoutine());
        }

        float elapsedTime = 0;
        Color originalColor = tmpText.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        gameObject.SetActive(false);
    }

    IEnumerator GlitchRoutine()
    {
        if (audioSource != null && glitchSound != null)
            audioSource.PlayOneShot(glitchSound);

        float elapsed = 0f;
        bool lastVisible = true;

        while (elapsed < glitchDuration)
        {
            lastVisible = !lastVisible;
            tmpText.enabled = lastVisible;

            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude);
            float offsetY = Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

            float scaleJitter = Random.Range(0.9f, 1.1f);
            transform.localScale = originalScale * scaleJitter;

            yield return new WaitForSeconds(flickerSpeed);
            elapsed += flickerSpeed;
        }

        tmpText.enabled = true;
        transform.localPosition = originalPosition;
        transform.localScale = originalScale;
    }
}