using System;
using Unity;
using UnityEngine;
namespace TheNorth 
{
    [Serializable]
    public struct Story
    {
        public Action acionOnStoryEnd;
        public string tag;
        public string text;
        public Answer[] answers;
    }
    [Serializable]
    public struct Answer 
    {
        public string text;
        public string tagOfNextDialogue;
    }
}