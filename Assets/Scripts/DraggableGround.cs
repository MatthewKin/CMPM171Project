using UnityEngine;

public class DraggableGround : MonoBehaviour
{
    public float gridSize = 1f;

    [Header("Player Check")]
    public LayerMask playerLayer;
    public float checkRadius = 0.4f;

    private Vector3 offset;
    private bool isDragging;

    void OnMouseDown()
    {
        // 🚫 Block dragging if player is on tile
        if (IsPlayerOnTile())
            return;

        Vector3 mouseWorld = GetMouseWorldPosition();
        offset = transform.position - mouseWorld;

        isDragging = true;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;

        // Snap when released
        Vector3 snapped = transform.position;
        snapped.x = Mathf.Round(snapped.x / gridSize) * gridSize;
        snapped.y = Mathf.Round(snapped.y / gridSize) * gridSize;
        snapped.z = 0f;

        transform.position = snapped;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mouseWorld = GetMouseWorldPosition();
            transform.position = mouseWorld + offset;
        }
    }

    bool IsPlayerOnTile()
    {
        return Physics2D.OverlapCircle(transform.position, checkRadius, playerLayer);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 world = Camera.main.ScreenToWorldPoint(mouseScreen);
        world.z = 0f;

        return world;
    }
}