using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDetector : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius = 10f;
    public LayerMask playerLayer;

    [Header("Scanner")]
    public float rotationSpeed = 180f;
    public float rotationOffset = 0f;
    public Transform scannerVisual;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Debug")]
    public bool drawRay = true;

    private Transform targetPlayer;
    private Rigidbody2D rb;

    private float currentAngle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        RotateScanner();
        DetectPlayer();
    }

    void FixedUpdate()
    {
        MoveTowardsPlayer();
    }

    void RotateScanner()
    {
        // Rotate continuously around enemy
        currentAngle += rotationSpeed * Time.deltaTime;

        if (currentAngle >= 360f)
            currentAngle -= 360f;

        // Apply offset
        float finalAngle = currentAngle + rotationOffset;

        // Rotate scanner visual
        if (scannerVisual != null)
        {
            scannerVisual.rotation = Quaternion.Euler(0, 0, finalAngle);
        }
    }

    void DetectPlayer()
    {
        // Stop scanning if player already found
        if (targetPlayer != null)
            return;

        // Match ray direction with scanner rotation
        Vector2 direction =
            Quaternion.Euler(0, 0, currentAngle + rotationOffset) * Vector2.right;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction,
            detectionRadius,
            playerLayer
        );

        // Draw debug ray
        if (drawRay)
        {
            Debug.DrawRay(
                transform.position,
                direction * detectionRadius,
                hit.collider ? Color.red : Color.green
            );
        }

        // Detect player
        if (hit.collider != null)
        {
            targetPlayer = hit.transform;
            Debug.Log("Player detected!");
        }
    }

    void MoveTowardsPlayer()
    {
        if (targetPlayer == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction =
            (targetPlayer.position - transform.position).normalized;

        rb.linearVelocity = direction * moveSpeed;
    }

    // Restart scene on collision with player
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }
    }

    // If using triggers instead of collisions
    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }
    }

    // Draw detection radius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}