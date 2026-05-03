using UnityEngine;
using TMPro;

public class LocalTextFade : MonoBehaviour
{
    public float fadeSpeed = 1f; // how fast it fades

    private TextMeshProUGUI text;
    private bool isFading = false;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Detect WASD input
        if (!isFading && (
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D)))
        {
            isFading = true;
        }

        // Fade out
        if (isFading)
        {
            Color c = text.color;
            c.a -= fadeSpeed * Time.deltaTime;
            c.a = Mathf.Clamp01(c.a);
            text.color = c;
        }
    }
}