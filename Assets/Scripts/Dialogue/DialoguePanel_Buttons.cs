using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TheNorth 
{
    [RequireComponent(typeof(DialogueLogic))]
    [RequireComponent(typeof(DialoguePanel_Buttons))]
    public class DialoguePanel_Buttons: MonoBehaviour
    {
        [SerializeField] private List<Button> _buttons;
        private string[] _currentStoryReplyTags;
        private DialogueLogic _dialogueLogic;

        private void Awake()
        {
            _dialogueLogic = GetComponent<DialogueLogic>();
            _dialogueLogic.OnChangedStory += ChangeAnswers;
            _currentStoryReplyTags = new string[_buttons.Count];
            for(int i = 0; i < _buttons.Count; i++) 
            {
                int test = i;
                _buttons[i].onClick.AddListener(() => SendAnswer(test));
            }
        }
        private void SendAnswer(int buttonIndex) => _dialogueLogic.ChangeStory(_currentStoryReplyTags[buttonIndex]);
        private void ChangeAnswers(Story story) 
        {
            for (int i = 0; i < _buttons.Count; i++) 
            {
                if(story.answers.Count() <= i) 
                {
                    _buttons[i].gameObject.SetActive(false);
                    continue;
                }
                TMP_Text buttonText = _buttons[i].gameObject.GetComponentInChildren<TMP_Text>();
                _currentStoryReplyTags[i] = story.answers[i].tagOfNextDialogue;
                buttonText.text = story.answers[i].text;
                _buttons[i].gameObject.SetActive(true);
            }
        }
    }
}