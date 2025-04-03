using System;
using TheNorth;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.Windows;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Attack Character", 
    story: "[Character] attacks [Target]",
    description: "Attacks [Target] using [Character] attack methods.",
    category: "Action/Custom", 
    id: "37648d4c98f7c1b11453c342f535c171")]
public partial class AttackCharacterAction : Action
{
    [SerializeReference] public BlackboardVariable<Character> Character;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private bool _isHit = false;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Character.Value.CanAttack)
        {
            _isHit = Character.Value.TryAttack(Character.Value.AttackSource);
        }

        if (_isHit)
            return Status.Success;

        return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

