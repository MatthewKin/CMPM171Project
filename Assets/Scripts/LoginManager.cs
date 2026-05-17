using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    [Header("References")]
    public GameObject loginBox; // The whole login UI object
    public TextMeshProUGUI passwordText; // The long dark bar where typed text appears
    public GameObject loginButton; // The short dark bar button
    public GameObject cursor; // A TMP object with "|" for blinking cursor
    public JsonTextLoader wrongPasswordDialogue; // Drag your wrong password JsonTextLoader here

    [Header("Settings")]
    public string correctPassword = "86453";
    private string currentInput = "";
    private bool acceptingInput = false; 
    private Coroutine blinkCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //hiding things at start
        loginBox.SetActive(false);
        loginButton.SetActive(false);
        if(cursor != null)
        {
            cursor.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!acceptingInput)
        {
            return;
        }

        if (Input.inputString.Length > 0 && cursor != null)
        {
            cursor.SetActive(false);
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                blinkCoroutine = null;
            }  
        }
        
        foreach(char c in Input.inputString)
        {
            if(c == '\b') //backspace
            {
                if(currentInput.Length > 0)
                {
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
                }
            }
            else if(c != '\n' && c != '\r')
            {
                currentInput += c;
            }
        }
        passwordText.text = new string('*', currentInput.Length);
    }

    public void ShowLogin()
    {
        loginBox.SetActive(true);
    }

    // Called by IntroCutsceneManager after dialogue 1 ends (and after wrong password dialogue ends)
    public void EnableInput()
    {
        acceptingInput = true;
        currentInput = "";
        passwordText.text = "";
        loginButton.SetActive(true);
 
        if (cursor != null)
        {
            cursor.SetActive(true);
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkCursor());
        }
    }
 
    IEnumerator BlinkCursor()
    {
        while (acceptingInput)
        {
            cursor.SetActive(!cursor.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }
        cursor.SetActive(false);
    }
 
    // Hook this up to the login button's OnClick in the Inspector
    public void OnLoginButtonClicked()
    {
        if (currentInput == correctPassword)
        {
            // Correct! Stop accepting input and tell the cutscene manager
            acceptingInput = false;
            loginButton.SetActive(false);
            loginBox.SetActive(false);
            FindObjectOfType<IntroCutsceneManager>().CorrectPassword();
        }
        else
        {
            // Wrong password — stop input, show the dialogue box, play wrong password dialogue
            acceptingInput = false;
            loginButton.SetActive(false);
 
            FindObjectOfType<IntroCutsceneManager>().dialogueBox.SetActive(true);
            wrongPasswordDialogue.ReplayDialogue();
        }
    }
}
