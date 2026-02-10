using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [Header("Characters")]
    public string[] characters = new string[2];
    private List<Image> characterImages = new List<Image>();
    [Header("Dialogue")]
    public string[] lines;
    public List<Line> dialogueLines = new List<Line>();
    private List<string> dialgue = new List<string>();
    public TextAsset script;

    [Header("UI")]
    public float typingSpeed = 0.1f;
    public GameObject dialogBox;
    public TextMeshProUGUI characterName;
    private TextMeshProUGUI dialogText;
    public int maxLineLength = 50;
    public Image CharacterAImage;
    public Image CharacterBImage;
    public float characterOpacityValue = 0.5f;
    public Image nextButton;


    private int dialogueIndex = 0;
    private bool canContinue;
    private bool dialogueIsPlaying;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogText = dialogBox.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(dialogText);
        dialogueIndex = 0;
        canContinue = true;

        //ProccessTextFile(script);

        if(CharacterAImage == null || CharacterBImage == null){
            Debug.LogError("No images set");
        }

        characterImages.Add(CharacterAImage);
        characterImages.Add(CharacterBImage);
 
        ContinueDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ContinueDialogue();
        }
    }

    void ProccessTextFile(TextAsset textFile)
    {
        string[] tempLines;
        tempLines = textFile.text.Split('\n');

        for(int i = 0; i < tempLines.Length; i++)
        {
            Line dialogueLine = new Line();
            Debug.Log(tempLines[i]);
            if(tempLines[i].ToLower() == characters[0].ToString().ToLower() || tempLines[i].ToLower() == characters[1].ToString().ToLower())
            {
                dialogueLine.speaker = tempLines[i];
            }
            dialogueLines.Add(dialogueLine);
        }
        
    }

    public void ContinueDialogue()
    {
        Debug.Log("Continue Dialogue");
        if(!canContinue) return;

        ClearCurrentDialogue();
        if (dialogueIndex < lines.Length)
        {
            
            characterName.text = GetCurrentSpeakingCharacter();
            StartCoroutine(DisplayLines(lines[dialogueIndex]));
        }
        else
        {
            dialogBox.SetActive(false);
        }
        dialogueIndex++;
    }



    private void ClearCurrentDialogue()
    {
        dialogText.text = "";
    }

    public void StartDialogue()
    {
        ClearCurrentDialogue();
        dialogBox.SetActive(true);
        characterName.text = GetCurrentSpeakingCharacter();
        StartCoroutine(DisplayLines(lines[dialogueIndex]));
    }

    string GetCurrentSpeakingCharacter()
    {
        Debug.Log("Current Speaker: " + characters[dialogueIndex % characters.Length] + " Index: " + dialogueIndex + " mod: " + (dialogueIndex % characters.Length));
        if(dialogueIndex % characters.Length == 0)
        {
            characterImages[(dialogueIndex) % characters.Length].color = new Color(1, 1, 1, 1);
            characterImages[(dialogueIndex + 1) % characters.Length].color = new Color(1, 1, 1, characterOpacityValue);
        }
        else
        {
            Debug.Log("Took Else path");
            characterImages[(dialogueIndex + 1) % characters.Length].color = new Color(1, 1, 1, characterOpacityValue);
            characterImages[(dialogueIndex) % characters.Length].color = new Color(1, 1, 1, 1);
        }
        return characters[dialogueIndex % characters.Length];
    }

    void HandleOverflow()
    {
        
    }

    bool OverflowCheck()
    {
        if (lines[dialogueIndex].Length > maxLineLength)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator DisplayLines(string dialogue)
    {
        canContinue = false;

        nextButton.color = new Color(1, 1, 1, characterOpacityValue);
        
        
        foreach (char letter in dialogue.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        canContinue = true;
        nextButton.color = new Color(1, 1, 1, 1);
    }
}
