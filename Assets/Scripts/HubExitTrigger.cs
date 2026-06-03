using UnityEngine;

public class HubExitTrigger : MonoBehaviour
{
    private ComputerHubManager hubManager;

    void Start()
    {
        hubManager = FindObjectOfType<ComputerHubManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (hubManager != null)
            hubManager.FileTriggered();
    }
}