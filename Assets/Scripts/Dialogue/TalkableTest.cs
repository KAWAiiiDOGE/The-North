using UnityEngine;

public class TalkableTest : MonoBehaviour
{
    [SerializeField] private GameObject _visualCue;
    [SerializeField] private TextAsset _inkJSON;
    [SerializeField] private bool _canTalk;
    public void StartDialogue() 
    {
        if(_canTalk) 
        {
            DialogueManager.Instance.EnterDialogue(_inkJSON);
            _canTalk = false;
            _visualCue.SetActive(false);
        }
    }
    private void Update()
    {
        _visualCue.SetActive(_canTalk);
        _visualCue.transform.LookAt(Camera.main.transform.position);
    }
}