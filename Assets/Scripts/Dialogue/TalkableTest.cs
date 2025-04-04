using System;
using System.Collections.Generic;
using TheNorth;
using UnityEngine;

public class TalkableTest : MonoBehaviour
{
    [SerializeField] private List<Story> _dialogues;
    [SerializeField] private GameObject _visualCue;
    [SerializeField] private bool _canTalk;
    public bool TryStartDialogue(Action dialogueCallback)
    {
        if(_canTalk) 
        {
            _canTalk = false;
            _visualCue.SetActive(false);
            DialogueLogic.Instance.StartDialogue(_dialogues, dialogueCallback);
            return true;
        }
        return false;
    }
    private void Update()
    {
        _visualCue.SetActive(_canTalk);
        _visualCue.transform.LookAt(Camera.main.transform.position);
    }
}