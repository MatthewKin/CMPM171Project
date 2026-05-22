
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

/*
    Player spawn in annimation
    dialogue 0
    single file sits in the center that will just sit behind the dialogue pop up
    when player clicks on the file screen glitch again and switch scenes to tutorial 1
*/

public class IntroCutsceneManager : MonoBehaviour
{
    [Header("Dialogue")]
    public JsonTextLoader Dialogue0;

    [Header("UI")]
    public GameObject tutorialFile;
    public GameObject dialogueBox;

    [Header("Animatior")]
    public Animator animator;

    public ScreenGlitchEffect screenGlitch;
    void Start()
    {
        dialogueBox.SetActive(false);

        if (dialogue0.dialogueFinished == null)
            dialogue0.dialogueFinished = new UnityEvent();

        dialogue0.dialogueFinished.AddListener(() => DialogueFinished(0));

        dialogueBox.SetActive(true);
    }

    void Update()
    {
        
    }

    public void DialogueFinished(int dialogueIndex)
    {
        print("Finished dialogue" + dialogueIndex);
        dialogueBox.SetActive(false); //hides when dialgoue ends

        switch(dialogueIndex) {
            case 0:
                //Button starts to glow
                //Enable the button to turn on computer
                tutorialFile.SetActive(true);
                break;
            case 1:
                //Player sucked in animation
                screenGlitch.StartGlitchSequence();
                break;
        }
    }

    public void PowerButtonClicked()
    {
        StartCoroutine(GlitchAndContinue());
    }

    public IEnumerator GlitchAndContinue()
    {
        //will play glitch annimation and do dialogue 3
        animator.Play("ComputerGlitch");
        yield return null;
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);

        dialogueBox.SetActive(true);
        dialogue2.waitForStart = false; 
        dialogue2.PlayNextLine(); 
    }
}
