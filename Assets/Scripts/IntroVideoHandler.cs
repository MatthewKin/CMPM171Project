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
    public float finalFadeDuration = 2f; // Total duration of the cross-fade transition
    public UnityEvent onVideoFaded;     
    public UnityEvent onDialogueFinished; // Optional event for when the puzzle is fully visible after cross-fade

    private VideoPlayer videoPlayer;
    private bool hasFaded = false;      
    public UnityEvent onFadeFinished;

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
        
        // Ensure puzzle starts hidden and completely transparent if it has a CanvasGroup
        if (puzzleObject != null)
        {
            puzzleObject.SetActive(false);
            CanvasGroup canvasGroup = puzzleObject.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }
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
        // Start the cross-fading process instead of immediately destroying
        StartCoroutine(CrossFadeAndDestroy());
    }

    private IEnumerator CrossFadeAndDestroy()
    {
        onDialogueFinished?.Invoke(); // Optional: Notify that the dialogue has finished and puzzle is about to appear
        float startVideoAlpha = videoPlayer.targetCameraAlpha;
        float elapsed = 0f;

        // Instantly hide dialogue UI so it doesn't clutter the cross-fade
        if (dialogueObject != null) dialogueObject.SetActive(false);

        CanvasGroup puzzleCanvasGroup = null;

        // Activate puzzle object and grab its CanvasGroup for fading
        if (puzzleObject != null)
        {
            puzzleObject.SetActive(true);
            puzzleCanvasGroup = puzzleObject.GetComponent<CanvasGroup>();
            if (puzzleCanvasGroup != null)
            {
                puzzleCanvasGroup.alpha = 0f; // Start completely see-through
            }
            else
            {
                Debug.LogWarning("PuzzleObject is missing a CanvasGroup component! It will appear instantly during the cross-fade.");
            }
        }

        // SIMULTANEOUS CROSS-FADE LOOP
        // Both the video fading out and puzzle fading in happen inside this single loop
        while (elapsed < finalFadeDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / finalFadeDuration;

            // Fade video out (from 0.3 down to 0)
            videoPlayer.targetCameraAlpha = Mathf.Lerp(startVideoAlpha, 0f, normalizedTime);

            // Fade puzzle in (from 0 up to 1)
            if (puzzleCanvasGroup != null)
            {
                puzzleCanvasGroup.alpha = Mathf.Lerp(0f, 1f, normalizedTime);
            }

            yield return null; // Wait for the next frame
        }

        // Hard-set final values to ensure absolute precision at the end of the loop
        videoPlayer.targetCameraAlpha = 0f;
        if (puzzleCanvasGroup != null)
        {
            puzzleCanvasGroup.alpha = 1f;
        }

        onFadeFinished?.Invoke();
        // Destroy the video handler now that the visual transition is finished
        Destroy(gameObject);
    }
}
