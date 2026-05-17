using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

/*
Computer Off
Some Dialogue
Button starts to glow
Click the red button to turn Computer On
Computer turn on animation
Some more dialogue
Blinking cursor in password box (maybe)
Player has to type in password
Computer Launches -> Glitches
Some more dialogue(Maybe?) 
*/

public class IntroCutsceneManager : MonoBehaviour
{
    public JsonTextLoader dialogue0;
    public JsonTextLoader dialogue1;
    public JsonTextLoader dialogue2;

    public GameObject powerButton;
    public Animator animator;

    void Start()
    {
        if (dialogue0.dialogueFinished == null)
            dialogue0.dialogueFinished = new UnityEvent();

        dialogue0.dialogueFinished.AddListener(() => DialogueFinished(0));
/*
        if (dialogue1.dialogueFinished == null)
            dialogue1.dialogueFinished = new UnityEvent();

        dialogue1.dialogueFinished.AddListener(() => DialogueFinished(1));

        if (dialogue2.dialogueFinished == null)
            dialogue2.dialogueFinished = new UnityEvent();

        dialogue2.dialogueFinished.AddListener(() => DialogueFinished(2)); */
    }

    void Update()
    {
        
    }

    public void DialogueFinished(int dialogueIndex)
    {
        print("Finished dialogue" + dialogueIndex);

        switch(dialogueIndex) {
            case 0:
                //Button starts to glow
                //Enable the button to turn on computer
                powerButton.SetActive(true);
                break;
            case 1:
                //Blinking cursor
                //Enable password input
                break;
            case 2:
                //Player sucked in animation
                break;
        }
    }

    public void PowerButtonClicked()
    {
        StartCoroutine(PowerButton());
    }

    public IEnumerator PowerButton()
    {
        print("Click");
        //Computer Turn On Animation
        //Start dialogue 2

        animator.Play("ComputerTurnOn");
        yield return null; 

        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);

        powerButton.GetComponent<Image>().color = Color.white;
        powerButton.GetComponent<Button>().enabled = false;
        powerButton.GetComponent<Blink>().enabled = false;

        dialogue1.waitForStart = false;
    }
}
