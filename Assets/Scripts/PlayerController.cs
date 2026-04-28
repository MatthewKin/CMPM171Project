using UnityEngine;

public class TopDownPlayerWithBounds : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Animation")]
    public Animator animator;
    private Vector2 lastMoveDir;

    [Header("Ground Offset")]
    public float groundCheckYOffset = -0.296f;

    [Header("Afterimage")]
    public GameObject afterImagePrefab;
    public float afterImageRate = 0.05f;
    private float afterImageTimer;

    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        Vector2 input = movement.normalized;

        if (input != Vector2.zero)
        {
            lastMoveDir = input;
        }

        animator.SetFloat("MoveX", input.x);
        animator.SetFloat("MoveY", input.y);
        animator.SetFloat("Speed", input.sqrMagnitude);

        if (input == Vector2.zero)
        {
            animator.SetFloat("MoveX", lastMoveDir.x);
            animator.SetFloat("MoveY", lastMoveDir.y);
        }

        HandleAfterImage();
    }

    void FixedUpdate()
    {
        Vector2 currentPosition = rb.position;
        Vector2 desiredMove = movement.normalized * moveSpeed * Time.fixedDeltaTime;

        Vector2 newPosition = currentPosition;

        Vector2 offsetPosition = currentPosition + new Vector2(0, groundCheckYOffset);

        Vector2 checkPosX = offsetPosition + new Vector2(desiredMove.x, 0);
        if (IsGroundAtPosition(checkPosX))
        {
            newPosition.x += desiredMove.x;
        }

        Vector2 checkPosY = offsetPosition + new Vector2(0, desiredMove.y);
        if (IsGroundAtPosition(checkPosY))
        {
            newPosition.y += desiredMove.y;
        }

        rb.MovePosition(newPosition);
    }

    void HandleAfterImage()
    {
        if (movement.sqrMagnitude > 0.01f)
        {
            afterImageTimer -= Time.deltaTime;

            if (afterImageTimer <= 0f)
            {
                SpawnAfterImage();
                afterImageTimer = afterImageRate;
            }
        }
        else
        {
            afterImageTimer = 0f;
        }
    }

    void SpawnAfterImage()
    {
        GameObject ghost = new GameObject("AfterImage");

        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;
        ghost.transform.localScale = transform.localScale;

        SpriteRenderer ghostSR = ghost.AddComponent<SpriteRenderer>();

        ghostSR.sprite = sr.sprite;
        ghostSR.flipX = sr.flipX;
        ghostSR.flipY = sr.flipY;
        ghostSR.sortingLayerID = sr.sortingLayerID;
        ghostSR.sortingOrder = sr.sortingOrder - 1;

        Color c = sr.color;
        c.a = 0.45f;
        ghostSR.color = c;

        ghost.AddComponent<AfterImageFade>();
    }

    bool IsGroundAtPosition(Vector2 position)
    {
        return Physics2D.OverlapCircle(position, checkRadius, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (Application.isPlaying)
        {
            Vector2 offsetPosition = (Vector2)transform.position + new Vector2(0, groundCheckYOffset);
            Gizmos.DrawWireSphere(offsetPosition, checkRadius);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, checkRadius);
        }
    }
}