using UnityEngine;
using System.Collections;

public class TriggerActivate : MonoBehaviour
{
    public GameObject targetObject;   // Object to enable/disable
    public float activeTime = 2f;     // Duration

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip triggerSound;

    private bool isRunning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isRunning)
        {
            StartCoroutine(ActivateSequence());
        }
    }

    IEnumerator ActivateSequence()
    {
        isRunning = true;

        // Activate object
        targetObject.SetActive(true);

        // Play sound
        if (audioSource != null && triggerSound != null)
        {
            audioSource.PlayOneShot(triggerSound);
        }

        // Wait
        yield return new WaitForSeconds(activeTime);

        // Deactivate object
        targetObject.SetActive(false);

        // Deactivate THIS trigger box
        gameObject.SetActive(false);
    }
}