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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        Vector2 currentPosition = rb.position;
        Vector2 desiredMove = movement.normalized * moveSpeed * Time.fixedDeltaTime;

        Vector2 newPosition = currentPosition;

        // Check X movement
        Vector2 checkPosX = currentPosition + new Vector2(desiredMove.x, 0);
        if (IsGroundAtPosition(checkPosX))
        {
            newPosition.x += desiredMove.x;
        }

        // Check Y movement
        Vector2 checkPosY = currentPosition + new Vector2(0, desiredMove.y);
        if (IsGroundAtPosition(checkPosY))
        {
            newPosition.y += desiredMove.y;
        }

        rb.MovePosition(newPosition);
    }

    bool IsGroundAtPosition(Vector2 position)
    {
        return Physics2D.OverlapCircle(position, checkRadius, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}