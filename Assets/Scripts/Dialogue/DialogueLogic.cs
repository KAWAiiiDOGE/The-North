using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace TheNorth 
{
    [RequireComponent(typeof(DialoguePanel_Buttons))]
    [RequireComponent(typeof(DialoguePanel_Main))]
    public class DialogueLogic : MonoBehaviour
    {
        private static DialogueLogic _instance;
        public static DialogueLogic Instance => _instance;
        private List<Story> _dialogueStories;
        private Dictionary<string, Story> _dialogueStoriesByTag;
        public event Action<Story> OnChangedStory;
        public event Action OnEndedDialogue;
        public bool IsDialoguePlaying;

        private void Awake()
        {
            _instance = this;
        }
        public void StartDialogue(List<Story> dialogues, Action dialogueCallback) 
        {
            _dialogueStories = dialogues;
            _dialogueStoriesByTag = _dialogueStories.ToDictionary(keySelector: key => key.tag, elementSelector: element => element);
            ChangeStory(_dialogueStories[0].tag);
            OnEndedDialogue += dialogueCallback;
        }

        public void ChangeStory(string tag) 
        {
            if (_dialogueStoriesByTag.Keys.Contains(tag) == false) 
            {
                OnEndedDialogue?.Invoke();
            }
            else 
            {
                OnChangedStory?.Invoke(_dialogueStoriesByTag[tag]);
            }
        }
    }
}