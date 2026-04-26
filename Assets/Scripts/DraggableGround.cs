using UnityEngine;
using System.Collections;

public class DraggableGround : MonoBehaviour
{
    public float gridSize = 1f;

    [Header("Player Check")]
    public LayerMask playerLayer;
    public float checkRadius = 0.4f;

    [Header("Lock Settings")]
    public bool isLocked = false;
    public bool alwaysLocked = false;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;
    [Header("Juice - Scale")]
    public float scaleUpAmount = 1.1f;
    public float scaleSpeed = 10f;

    private Vector3 offset;
    private bool isDragging;

    private Vector3 originalScale;
    private Vector3 targetScale;

    // tracking
    private Vector3 startDragPosition;
    private Vector3 startPosition;

    // color handling
    private SpriteRenderer sr;
    private Color originalColor;
    private Color lockedColor;
    private Color alwaysLockedColor = new Color32(0xEC, 0x3B, 0x00, 255);

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        startPosition = transform.position;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;

            lockedColor = new Color32(84, 84, 84, 255); // normal locked color
        }

        if (alwaysLocked)
        {
            isLocked = true;

            if (sr != null)
                sr.color = alwaysLockedColor;
        }
    }

    void OnMouseDown()
{
    if (alwaysLocked || isLocked)
    {
        StartCoroutine(Shake());
        return;
    }

    if (PlayerManager.Instance != null && PlayerManager.Instance.IsMoving())
        return;

    if (IsPlayerOnTile())
        return;

    Vector3 mouseWorld = GetMouseWorldPosition();
    offset = transform.position - mouseWorld;

    isDragging = true;
    startDragPosition = transform.position;

    targetScale = originalScale * scaleUpAmount;
}

    void OnMouseUp()
    {
        if (!isDragging) return;

        StopDragging();
    }

    void Update()
    {
        if (isDragging && PlayerManager.Instance != null && PlayerManager.Instance.IsMoving())
        {
            StopDragging();
            return;
        }

        if (isDragging)
        {
            Vector3 mouseWorld = GetMouseWorldPosition();
            transform.position = mouseWorld + offset;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    void StopDragging()
    {
        isDragging = false;

        Vector3 snapped = transform.position;
        snapped.x = Mathf.Round(snapped.x / gridSize) * gridSize;
        snapped.y = Mathf.Round(snapped.y / gridSize) * gridSize;
        snapped.z = 0f;

        transform.position = snapped;

        // lock ONLY if actually moved
        if (Vector3.Distance(startDragPosition, snapped) > 0.01f)
        {
            isLocked = true;

            if (sr != null)
            {
                sr.color = alwaysLocked ? alwaysLockedColor : lockedColor;
            }
        }

        targetScale = originalScale;
    }

    public void ResetWindow()
{
    isDragging = false;

    transform.position = startPosition;

    targetScale = originalScale;
    transform.localScale = originalScale;

    if (!alwaysLocked)
    {
        isLocked = false;

        if (sr != null)
{
    sr.color = alwaysLocked ? alwaysLockedColor : originalColor;
}
    }
    else
    {
        isLocked = true;

        if (sr != null)
    {
        sr.color = alwaysLocked ? alwaysLockedColor : originalColor;
    }
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

    IEnumerator Shake()
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.position = originalPos + new Vector3(offsetX, 0f, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}