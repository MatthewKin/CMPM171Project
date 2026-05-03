using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerInteract : MonoBehaviour
{
    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("End Settings")]
    public float delayBeforeSceneLoad = 2f;
    public string nextSceneName;

    [Header("Audio")]
    public AudioClip endSound;
    private AudioSource audioSource;

    private bool hasTriggered = false;
    private Animator animator;
    private TeleportWindow currentTeleporter = null;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Get or add AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        animator.SetBool("IsEnding", false);
    }

    void Update()
    {
        if (hasTriggered) return;

        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (hit == null)
        {
            // Player stepped off ANY tile
            currentTeleporter = null;
            return;
        }

        // END TILE
        if (hit.CompareTag("End"))
        {
            hasTriggered = true;
            StartCoroutine(HandleLevelEnd());
            return;
        }

        // TELEPORT WINDOW
        TeleportWindow teleporter = hit.GetComponent<TeleportWindow>();

        if (teleporter != null)
        {
            // Only teleport if it's a NEW teleporter
            if (currentTeleporter != teleporter)
            {
                teleporter.Teleport(gameObject);

                // Set the destination as current so we don't bounce back
                currentTeleporter = teleporter.GetLinkedTeleport();
            }
        }
    }


    IEnumerator HandleLevelEnd()
    {
        // Disable movement
        TopDownPlayerWithBounds movement = GetComponent<TopDownPlayerWithBounds>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        // Stop motion
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;

        // Play animation
        animator.SetBool("IsEnding", true);

        // Play sound
        if (endSound != null)
        {
            audioSource.PlayOneShot(endSound);
        }

        yield return new WaitForSeconds(delayBeforeSceneLoad);

        SceneManager.LoadScene(nextSceneName);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (groundCheck != null)
        {
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}