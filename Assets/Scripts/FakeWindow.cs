using UnityEngine;
using System.Collections;

public class FakeWindow : MonoBehaviour
{
    private Animator animator;
    private bool hasClicked = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnMouseDown()
    {
        if (hasClicked) return; // prevent multiple clicks
        hasClicked = true;

        if (animator != null)
        {
            animator.SetTrigger("onClick");
        }

        StartCoroutine(DisableAfterDelay());
    }

    IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(0.75f);
        gameObject.SetActive(false);
    }
}