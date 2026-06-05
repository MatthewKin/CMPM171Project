using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    private Vector3 playerStartPos;

    [Header("Windows / Tiles")]
    public DraggableGround[] windows;

    [Header("Glitch Windows")]
    public GlitchWindow[] glitchWindows;

    [Header("Animatior")]
    public Animator animator;


    void Start()
    {
        player.gameObject.SetActive(false);
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

        IntroVideoHandler introVideo = FindObjectOfType<IntroVideoHandler>();
        if (introVideo != null)
        {
            // Wait for video to finish before playing teleport
           if (introVideo.onFadeFinished == null)
            {
                introVideo.onFadeFinished = new UnityEvent();
            }
           introVideo.onFadeFinished.AddListener(() => StartCoroutine(TeleportInAnimation()));
        }
        else
        {
            // No intro video in this scene, play immediately
            StartCoroutine(TeleportInAnimation());
        }
        
    }

     IEnumerator TeleportInAnimation()
    {
        player.gameObject.SetActive(true);
        yield return null;
        Debug.Log("TELEPORT ANIM STARTING AT: " + Time.time);
        animator.SetBool("IsEnding", true);
        animator.Play("TeleportReversed");
        yield return new WaitForSeconds(1.2f);
        animator.SetBool("IsEnding", false);
        Debug.Log("TeleportInAnimation finished");
    }

    void Update()
    {
        // Press R to reset
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
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