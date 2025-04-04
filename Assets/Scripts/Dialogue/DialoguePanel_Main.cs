using TMPro;
using Unity;
using Unity.VisualScripting;
using UnityEngine;

namespace TheNorth 
{
    [RequireComponent(typeof(DialogueLogic))]
    [RequireComponent(typeof(DialoguePanel_Buttons))]
    public class DialoguePanel_Main: MonoBehaviour
    {
        private DialogueLogic _dialogueLogic;
        private TMP_Text _text;
        private void Awake()
        {
            _text = GetComponentInChildren<TMP_Text>();
            _dialogueLogic = GetComponent<DialogueLogic>();
            _dialogueLogic.OnChangedStory += ChangeText;
            _dialogueLogic.OnEndedDialogue += EndDialogue;
        }
        private void Start()
        {
            gameObject.SetActive(false);
        }
        private void ChangeText(Story story) 
        {
            gameObject.SetActive(true);
            _text.text = story.text;
        }
        private void EndDialogue() 
        {
            gameObject.SetActive(false);
        }
    }
}