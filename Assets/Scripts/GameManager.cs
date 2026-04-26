using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    private Vector3 playerStartPos;

    [Header("Windows / Tiles")]
    public DraggableGround[] windows;

    void Start()
    {
        // store player start position
        if (player != null)
            playerStartPos = player.position;

        // auto-find all windows if not assigned
        if (windows == null || windows.Length == 0)
        {
            windows = FindObjectsOfType<DraggableGround>();
        }
    }

    // button commands
    public void ResetGame()
    {
        // reset player
        if (player != null)
            player.position = playerStartPos;

        // reset all windows
        foreach (var w in windows)
        {
            if (w == null) continue;

            w.ResetWindow();
        }
    }
}