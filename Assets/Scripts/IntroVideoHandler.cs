using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events; 
using System.Collections; // Required for Coroutines

public class IntroVideoHandler : MonoBehaviour
{
    [Header("Video Setup")]
    public VideoClip introClip;
    public Camera targetCamera;
    
    [Header("Game Objects")]
    public GameObject gameCanvas;       
    public GameObject dialogueObject;   
    public GameObject puzzleObject;     

    [Header("Behavior")]
    [Range(0f, 1f)]
    public float fadedAlpha = 0.3f;     
    public float finalFadeDuration = 2f; // How long it takes to fade out at the very end
    public UnityEvent onVideoFaded;     

    private VideoPlayer videoPlayer;
    private bool hasFaded = false;      

    void Awake()
    {
        if (targetCamera == null) targetCamera = Camera.main;

        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.clip = introClip;

        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.targetCamera = targetCamera;
        
        videoPlayer.targetCameraAlpha = 1.0f;

        videoPlayer.loopPointReached += OnVideoFinished;

        if (dialogueObject != null) dialogueObject.SetActive(false);
        if (puzzleObject != null) puzzleObject.SetActive(false);
    }

    void Start()
    {
        if (introClip != null)
        {
            videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("No Video Clip assigned to IntroVideoHandler!");
            OnVideoFinished(videoPlayer);
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (hasFaded) return;
        hasFaded = true;

        videoPlayer.targetCameraAlpha = fadedAlpha;
        videoPlayer.isLooping = true;
        videoPlayer.Play();

        if (gameCanvas != null) gameCanvas.SetActive(true);
        if (dialogueObject != null) dialogueObject.SetActive(true);

        onVideoFaded?.Invoke();
    }

    /// <summary>
    /// Triggered by the UnityEvent in your JsonTextLoader script
    /// </summary>
    public void FinishDialogueAndLoadPuzzle()
    {
        // Start the fading process instead of immediately destroying
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float startAlpha = videoPlayer.targetCameraAlpha;
        float elapsed = 0f;

        // Smoothly transition the alpha from its current state (0.3) to 0 over 2 seconds
        while (elapsed < finalFadeDuration)
        {
            elapsed += Time.deltaTime;
            videoPlayer.targetCameraAlpha = Mathf.Lerp(startAlpha, 0f, elapsed / finalFadeDuration);
            yield return null; // Wait for the next frame
        }

        // Ensure it's completely invisible at the end
        videoPlayer.targetCameraAlpha = 0f;

        // Now that the fade is done, load the puzzle and destroy the video
        if (puzzleObject != null)
        {
            puzzleObject.SetActive(true);
        }

        if (dialogueObject != null) dialogueObject.SetActive(false);

        Destroy(gameObject);
    }
}