using Ink.Runtime;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager _instance;
    public static DialogueManager Instance => _instance;
    private bool _isDialoguePlaying;
    public bool IsDialoguePlayer => _isDialoguePlaying;
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _dialoguePanelText;
    private Story _currentStory;

    private void Awake()
    {
        if(_instance != null) 
        {
            Debug.LogWarning("More than one dialogue manager are present");
        }
        _instance = this;
        _isDialoguePlaying = false;
        _dialoguePanel.SetActive(false);
    }
    public void OnSubmitPressed() 
    {
        if(_isDialoguePlaying) 
        {
            TryContinueStory();
        }
    }
    public void EnterDialogue(TextAsset inkJSON) 
    {
        _currentStory = new Story(inkJSON.text);
        _isDialoguePlaying = true;
        _dialoguePanel.SetActive(true);
        TryContinueStory();
    }
    private void TryContinueStory() 
    {
        if(_currentStory.canContinue) 
        {
            _dialoguePanelText.text = _currentStory.Continue();
        }
        else 
        {
            EndDialogue();
        }
    }
    private void EndDialogue() 
    {
        _dialoguePanel.SetActive(false);
        _isDialoguePlaying = false;
        _dialoguePanelText.text = "";
    }
}
