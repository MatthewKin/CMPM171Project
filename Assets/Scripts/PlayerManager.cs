using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public enum PlayerState
    {
        Idle,
        Moving
    }

    public PlayerState currentState = PlayerState.Idle;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        HandleState();
    }

    void HandleState()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        // basic example logic (replace with your actual controller later)
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            currentState = PlayerState.Moving;
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    public bool IsMoving()
    {
        return currentState == PlayerState.Moving;
    }
}