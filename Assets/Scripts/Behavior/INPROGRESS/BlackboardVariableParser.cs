using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Behavior;
using UnityEngine;

namespace TheNorth
{
    /// <summary>
    /// INPROGRESS
    /// </summary>
    [Serializable]
    public class BlackboardVariableParser
    {
        public Dictionary<object, BlackboardVariable> Variables => _variables;

        BehaviorGraphAgent _agent;
        int _variablesCount;
        Dictionary<object, BlackboardVariable> _variables = new();

        public BlackboardVariableParser(BehaviorGraphAgent agent)
        {
            _agent = agent;
            _variablesCount = _agent.BlackboardReference.Blackboard.Variables.Count;
        }

        public void GetVariables()
        {
            for (int i = 0; i < _variablesCount; i++)
            {
                BlackboardVariable var = _agent.BlackboardReference.Blackboard.Variables[i];
                _variables.Add(var.Name, var);
                Debug.Log($"Parsed {var.Name}");
            }
        }

        public void SubscribeVariables()
        {
            foreach (var item in _variables.Values)
            {
                item.OnValueChanged += delegate { };
            }
        }

        public void UnsubscribeVariables()
        {
            foreach (var item in _variables.Values)
            {
                item.OnValueChanged -= delegate { };
            }
        }
    }

    public interface IBehavior
    {

    }
}
