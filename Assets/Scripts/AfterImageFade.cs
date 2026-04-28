using UnityEngine;

public class AfterImageFade : MonoBehaviour
{
    public float fadeSpeed = 2f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (sr == null) return;

        Color c = sr.color;
        c.a -= fadeSpeed * Time.deltaTime;
        sr.color = c;

        if (c.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}