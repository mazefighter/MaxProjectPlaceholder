using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class M_DialogueManager : MonoBehaviour
{

    [Header("Dialogue UI")] 
    
    [SerializeField] private GameObject dialoguePanel;

    [SerializeField] private M_Text dialogueText;

    [Header("Choices UI")] 
    
    [SerializeField] private GameObject[] choices;

    private M_Text[] choicesText;
    private Button[] choiceButtons;

    private Story currentStory;
    
    //For demo purposes
    [SerializeField] private TextAsset inkJson;

    public bool dialogueIsPlaying { get; private set; }
    
    private static M_DialogueManager instance;

    private string tempDialogueText;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager");
        }
        instance = this;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        choicesText = new M_Text[choices.Length];
        for (int i = 0; i < choices.Length; i++)
        {
            choicesText[i] = choices[i].GetComponentInChildren<M_Text>();
        }

        
        
        //For demo purposes
        EnterDialogueMode(inkJson);
    }

    public static M_DialogueManager GetInstance()
    {
        return instance;
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }
        

        
        if (M_InputManager.GetInstance().GetSubmitPressed())
        {
            ContinueStory();
            dialogueText.skipAppear = false;
        }

        if (M_InputManager.GetInstance().GetSkipPressed())
        {
            if (!dialogueText.skipAppear)
            {
                dialogueText.skipAppear = true;

            }
            
        }
        
        
        
    }
    
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        ContinueStory();
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            tempDialogueText = currentStory.Continue();
            dialogueText.TextOnCodeChange(tempDialogueText);
            DisplayChoices();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }
    
    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        
        //demo
        SceneManager.LoadScene("DemoSceneAuswahl");
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More Choices than space");
        }

        int index = 0;
        for (int i = 0; i < currentChoices.Count; i++)
        {
            choices[i].gameObject.SetActive(true);
            choicesText[i].TextOnCodeChange(currentChoices[i].text);
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }
    
    public void MakeChoice(int index)
    {
        currentStory.ChooseChoiceIndex(index);
    }
}
