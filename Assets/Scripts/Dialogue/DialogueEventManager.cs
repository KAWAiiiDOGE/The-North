using System;
using System.Collections.Generic;
using Unity;
using UnityEngine;
namespace TheNorth 
{
    public class DialogueEventManager 
    {
        private static DialogueEventManager _instance;
        public static DialogueEventManager Instance 
        {
            get 
            {
                if (_instance == null) 
                {
                    _instance = new DialogueEventManager();
                }
                return _instance;
            }
        }
        private Dictionary<string, Action> _eventsByTag = new();
        public void Subscribe(string tag, Action action) 
        {
            _eventsByTag[tag] = action;
        }
        public void Trigger(string tag) 
        {
            try 
            {
                _eventsByTag[tag]?.Invoke();
            }
            catch (KeyNotFoundException) {
                
            }
        }

    }
}