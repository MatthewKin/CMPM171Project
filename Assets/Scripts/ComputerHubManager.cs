using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

/*
    Player spawns in with player spawn annimation (use teleport annimation as placeholder for now)\
    Player can walk around for funsies cause cool
    Dialogue comes up 
    When Dialogue finishes players can click on the file behind the dialogue box
    File button plays glitch effect and takes them to tutorial 1
*/

public class ComputerHubManager : MonoBehaviour
{
    [Header("Dialogue")]
    public JsonTextLoader dialogue0;

    [Header("UI")]
    public GameObject clickableFile;
    public GameObject dialogueBox;

    [Header("Animatior")]
    public Animator animator;

    public ScreenGlitchEffect screenGlitch;

    [Header("Player")]
    public TopDownPlayerWithBounds playerController;
    IEnumerator Start()
    {
         dialogueBox.SetActive(false);
        clickableFile.SetActive(false);
 
        // Lock player during opening dialogue
        if (playerController != null)
            playerController.enabled = false;
 
        
        StartCoroutine(TeleportInAnimation());

        if (dialogue0.dialogueFinished == null)
            dialogue0.dialogueFinished = new UnityEvent();
 
        dialogue0.dialogueFinished.AddListener(() => DialogueFinished(0));

        yield return new WaitForSeconds(1.5f); 

        dialogueBox.SetActive(true);
        dialogue0.waitForStart = false;
        dialogue0.PlayNextLine();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TeleportInAnimation()
    {
        print("does this work?");
        animator.SetBool("IsEnding", true);
        animator.Play("TeleportReversed");
        yield return new WaitForSeconds(1.2f);
        animator.SetBool("IsEnding", false);
        print("why no work");
    }

    public void DialogueFinished(int dialogueIndex)
    {
          dialogueBox.SetActive(false);
 
        switch (dialogueIndex)
        {
            case 0:
                // Dialogue done — unlock player, show the clickable file
                if (playerController != null)
                {
                    playerController.enabled = true;
                }
                clickableFile.SetActive(true);
                break;
        }
    }

   public void FileClicked()
    {
        // Lock player again, then fire glitch sequence
        if (playerController != null)
            playerController.enabled = false;
 
        clickableFile.SetActive(false);
        StartCoroutine(GlitchAndContinue());
    }
 

    public IEnumerator GlitchAndContinue()
    {
        screenGlitch.StartGlitchSequence(); 
        yield return null;
    }
}
