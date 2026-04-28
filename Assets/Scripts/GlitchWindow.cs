using UnityEngine;
using System.Collections;

public class GlitchWindow : MonoBehaviour
{
    public LayerMask playerLayer;
    public float checkRadius = 0.4f;

    private bool playerWasOn = false;

    private DraggableGround draggable;

    [Header("Glitch Effect")]
    public float glitchDuration = 0.3f;
    public float flickerSpeed = 0.05f;
    public float shakeMagnitude = 0.08f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip glitchSound;

    private SpriteRenderer sr;
    private Vector3 originalScale;
    private Vector3 originalPosition;

    private bool isGlitching = false;

    void Start()
    {
        draggable = GetComponent<DraggableGround>();
        sr = GetComponent<SpriteRenderer>();

        originalScale = transform.localScale;
        originalPosition = transform.position;

        // auto-assign if missing
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isGlitching) return;

        // 🚫 Skip logic if dragging
        if (draggable != null && draggable.IsDragging())
            return;

        bool playerIsOn = Physics2D.OverlapCircle(transform.position, checkRadius, playerLayer);

        if (playerWasOn && !playerIsOn)
        {
            StartCoroutine(GlitchDisappear());
        }

        playerWasOn = playerIsOn;
    }

    IEnumerator GlitchDisappear()
    {
        isGlitching = true;

        // 🎵 PLAY SOUND AT START
        if (audioSource != null && glitchSound != null)
        {
            audioSource.PlayOneShot(glitchSound);
        }

        float elapsed = 0f;

        Vector3 lastPos = transform.position;
        Vector3 lastScale = transform.localScale;
        bool lastVisible = true;

        while (elapsed < glitchDuration)
        {
            lastVisible = !lastVisible;
            if (sr != null)
                sr.enabled = lastVisible;

            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude);
            float offsetY = Random.Range(-shakeMagnitude, shakeMagnitude);
            lastPos = transform.position + new Vector3(offsetX, offsetY, 0f);
            transform.position = lastPos;

            float scaleJitter = Random.Range(0.8f, 1.2f);
            lastScale = originalScale * scaleJitter;
            transform.localScale = lastScale;

            yield return new WaitForSeconds(flickerSpeed);
            elapsed += flickerSpeed;
        }

        if (sr != null)
            sr.enabled = true;

        transform.position = lastPos;
        transform.localScale = lastScale;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        this.enabled = false;

        gameObject.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }

    public void Reactivate()
    {
        isGlitching = false;

        transform.localScale = originalScale;
        transform.position = originalPosition;

        if (sr != null)
            sr.enabled = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;

        this.enabled = true;
        gameObject.SetActive(true);

        playerWasOn = false;
    }
}