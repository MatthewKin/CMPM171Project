using UnityEngine;

public class AfterImageFade : MonoBehaviour
{
    public float fadeSpeed = 0f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (fadeSpeed == 0f) {
            fadeSpeed = 1f;
        }
    }

    void Update()
    {
        if (sr == null) return;

        Color c = sr.color;
        c = new Color32(208, 0, 210, (byte)(c.a * 255)); // set to red with original alpha
        c.a -= fadeSpeed * Time.deltaTime;
        sr.color = c;

        if (c.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}