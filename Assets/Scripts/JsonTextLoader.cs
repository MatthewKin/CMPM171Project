using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using Ink.Runtime;


public class JsonTextLoader : MonoBehaviour
{
    public enum DialogueMode {Elara, EvilMan}

    [Header("References")]
    public TextAsset inkJSON;
    public TextMeshProUGUI textComponent;
    public AudioSource audioSource;
    public AudioClip glitchSound;
    public UnityEvent dialogueFinished;

    [Header("Ink Settings")]  
    public string startKnot; //This will be accessing the Ink knots such as "Tutotial1_Elara" 
    public DialogueMode dialogueMode = DialogueMode.Elara;  // Can be used to trigger different visual/audio effects based on the character speaking

    [Header("Timings")]
    public float delayBeforeStart = 0f;
    public float typeSpeed = 0.05f;
    public bool waitForStart = false;
    
    [Header("EvilMan Auto-Advance")]
    public float delayBeforeFade = 2f; // Time to wait after a line finishes before auto-advancing

    [Header("Glitch Effect (Both)")]
    public bool useGlitchOnType = true; // Whether to apply a glitch effect while typing
    public float shakeMagnitude = 2f; // How much the text should shake while typing

    [Header("EvilMan Glitch/Flicker/Fade")]
    public float glitchDuration = 0.4f; // Duration of the glitch effect after typing
    public float flickerSpeed = 0.05f; // How fast the text should flicker during the glitch
    public float fadeDuration = 1.5f; // Duration of the fade-out effect

    private Story inkStory;
    private string currentLine = "";
    private bool isTyping = false;
    private bool waitingForInput = false;
    private Coroutine typingCoroutine;
    private Vector3 originalPos;
    private Vector3 originalScale;
    private bool started = false;

    // FIX: Changed 'void' to 'IEnumerator' to allow yield return
    IEnumerator Start()
    {
        originalPos = textComponent.transform.localPosition;
        originalScale = textComponent.transform.localScale;

        if (inkJSON == null)
        {
            Debug.LogError("No Ink JSON file attached!");
            yield break; // Exit the coroutine
        }

        inkStory = new Story(inkJSON.text);

        //jump to the current knot for this object
        if (!string.IsNullOrEmpty(startKnot))
        {
            inkStory.ChoosePathString(startKnot);
        }
        
        if (delayBeforeStart > 0f && !waitForStart)
        {
            yield return new WaitForSeconds(delayBeforeStart);
            started = true;
            PlayNextLine();
        }
    }

    void Update()
    {
        // Simple input check
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if(!waitForStart) 
            {
                HandleInput();
            }
        }

        if(!started && !waitForStart) {
            started = true;
            PlayNextLine();
        }
    }

    private void HandleInput()
    {
        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            
            // Finish line immediately
            textComponent.text = currentLine;
            textComponent.transform.localPosition = originalPos; // Reset shake
            isTyping = false;
            waitingForInput = true;
        }
        else if (waitingForInput)
        {
            waitingForInput = false;
            PlayNextLine();
        }
    }

    private void PlayNextLine()
    {
        Debug.Log($"canContinue: {inkStory.canContinue}, waitingForInput: {waitingForInput}, isTyping: {isTyping}");
        if(inkStory.canContinue)
        {
            string line = inkStory.Continue().Trim();
            if (string.IsNullOrEmpty(line))
            {
                PlayNextLine();
                return;
            }
            currentLine = line;
            textComponent.gameObject.SetActive(true);
            typingCoroutine = StartCoroutine(TypeoutEffect(line));
        }
        else
        {
            gameObject.SetActive(false); // Hide text when story ends
            //dialogue ends
            if(dialogueFinished != null)
            {
                dialogueFinished.Invoke();
            }
        }
    }

    public void ReplayDialogue()
    {
        // Reset story back to the start knot
        inkStory = new Story(inkJSON.text);
        if (!string.IsNullOrEmpty(startKnot))
            inkStory.ChoosePathString(startKnot);
 
        // Reset state
        isTyping = false;
        waitingForInput = false;
        started = false;
        currentLine = "";
        textComponent.text = "";
 
        // Re-activate and start
        gameObject.SetActive(true);
        waitForStart = false;
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

        if(dialogueMode == DialogueMode.EvilMan)
        {
            StartCoroutine(EvilManSequence());
        }
        else
        {
            waitingForInput = true; // Wait for user input to advance
        }
    }

     IEnumerator EvilManSequence()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        if (audioSource != null && glitchSound != null)
            audioSource.PlayOneShot(glitchSound);

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < glitchDuration)
        {
            visible = !visible;
            textComponent.enabled = visible;

            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude);
            float offsetY = Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

            float scaleJitter = Random.Range(0.9f, 1.1f);
            transform.localScale = originalScale * scaleJitter;

            yield return new WaitForSeconds(flickerSpeed);
            elapsed += flickerSpeed;
        }

        textComponent.enabled = true;
        transform.localPosition = originalPos;
        transform.localScale = originalScale;

        float fadeElapsed = 0f;
        Color OriginalColor = textComponent.color;
        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeElapsed / fadeDuration);
            textComponent.color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, alpha);
            yield return null;
        }

        textComponent.color = OriginalColor; 
        PlayNextLine(); // Automatically advance to the next line after fade out
    }
}