using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
public class DialogueData
{
    public string[] lines;
}

public class JsonTextLoader : MonoBehaviour
{
    [Header("References")]
    public TextAsset jsonFile;
    public TextMeshProUGUI textComponent;
    public AudioSource audioSource;
    public AudioClip glitchSound;

    [Header("Timings")]
    public float delayBeforeStart = 0f;
    public float typeSpeed = 0.05f;

    [Header("Glitch Effect")]
    public bool useGlitchOnType = true;
    public float shakeMagnitude = 2f;

    private DialogueData data;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private Vector3 originalPos;

    // FIX: Changed 'void' to 'IEnumerator' to allow yield return
    IEnumerator Start()
    {
        originalPos = textComponent.transform.localPosition;

        if (jsonFile == null)
        {
            Debug.LogError("No JSON file attached!");
            yield break; // Exit the coroutine
        }

        data = JsonUtility.FromJson<DialogueData>(jsonFile.text);

        if (data != null && data.lines.Length > 0)
        {
            if (delayBeforeStart > 0f)
                yield return new WaitForSeconds(delayBeforeStart);

            PlayCurrentLine();
        }
    }

    void Update()
    {
        // Simple input check
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            
            // Finish line immediately
            textComponent.text = data.lines[currentLineIndex];
            textComponent.transform.localPosition = originalPos; // Reset shake
            isTyping = false;
        }
        else
        {
            currentLineIndex++;
            if (currentLineIndex < data.lines.Length)
            {
                PlayCurrentLine();
            }
            else
            {
                // Disable the whole object or just the text when done
                gameObject.SetActive(false);
            }
        }
    }

    private void PlayCurrentLine()
    {
        textComponent.gameObject.SetActive(true);
        typingCoroutine = StartCoroutine(TypeoutEffect(data.lines[currentLineIndex]));
    }

    IEnumerator TypeoutEffect(string lineToType)
    {
        isTyping = true;
        textComponent.text = "";

        if (audioSource != null && glitchSound != null)
            audioSource.PlayOneShot(glitchSound);

        foreach (char c in lineToType)
        {
            textComponent.text += c;

            // Optional: Subtle jitter while typing
            if (useGlitchOnType)
            {
                float ox = Random.Range(-shakeMagnitude, shakeMagnitude);
                float oy = Random.Range(-shakeMagnitude, shakeMagnitude);
                textComponent.transform.localPosition = originalPos + new Vector3(ox, oy, 0);
            }

            yield return new WaitForSeconds(typeSpeed);
        }

        // Reset position after typing is done
        textComponent.transform.localPosition = originalPos;
        isTyping = false;
    }
}