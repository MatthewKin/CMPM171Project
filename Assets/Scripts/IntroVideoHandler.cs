using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoHandler : MonoBehaviour
{
    [Header("Video Setup")]
    public VideoClip introClip;
    public Camera targetCamera;
    public GameObject gameCanvas;

    [Header("Behavior")]
    public bool destroyOnEnd = true;

    private VideoPlayer videoPlayer;

    void Awake()
    {
        if (targetCamera == null) targetCamera = Camera.main;

        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.clip = introClip;

        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.targetCamera = targetCamera;

        videoPlayer.loopPointReached += OnVideoFinished;
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
        if (gameCanvas != null)
            {
                gameCanvas.SetActive(true);
                
            }
        if (destroyOnEnd)
            {
                Destroy(gameObject);
            }
    }
}