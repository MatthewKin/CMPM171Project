using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    private Vector3 playerStartPos;

    [Header("Windows / Tiles")]
    public DraggableGround[] windows;

    [Header("Glitch Windows")]
    public GlitchWindow[] glitchWindows;

    void Start()
    {
        // store player start position
        if (player != null)
            playerStartPos = player.position;

        // auto-find draggable windows if not assigned
        if (windows == null || windows.Length == 0)
        {
            windows = FindObjectsOfType<DraggableGround>();
        }

        // auto-find glitch windows if not assigned
        if (glitchWindows == null || glitchWindows.Length == 0)
        {
            glitchWindows = FindObjectsOfType<GlitchWindow>();
        }
    }

    // button commands
    public void ResetGame()
    {
        // reset player
        if (player != null)
            player.position = playerStartPos;

        // reset draggable windows
        foreach (var w in windows)
        {
            if (w == null) continue;
            w.ResetWindow();
        }

        // reactivate glitch windows
        foreach (var gw in glitchWindows)
        {
            if (gw == null) continue;
            gw.Reactivate();
        }
    }
}