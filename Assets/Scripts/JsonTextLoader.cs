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

    [Header("Settings")]
    public float typeSpeed = 0.05f;

    private DialogueData data;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (jsonFile == null)
        {
            Debug.LogError("No JSON file attached!");
            return;
        }

        data = JsonUtility.FromJson<DialogueData>(jsonFile.text);

        if (data != null && data.lines.Length > 0)
        {
            PlayCurrentLine();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            
            textComponent.text = data.lines[currentLineIndex];
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
                textComponent.gameObject.SetActive(false);
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

        foreach (char c in lineToType)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }
}