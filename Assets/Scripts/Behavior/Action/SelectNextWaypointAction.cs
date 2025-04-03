using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.AppUI.UI;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Select Next Waypoint", 
    story: "Select next [Target] from [Waypoints]", 
    description: "Selects next random position from [Waypoints] and sets it to [Target].",
    category: "Action/Custom", 
    id: "f64393e546db8fbc8f29c5cf668e5678")]
public partial class SelectNextWaypointAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> Target;
    [SerializeReference] public BlackboardVariable<List<Vector3>> Waypoints;
    protected override Status OnStart()
    {
        Target.Value = Waypoints.Value[Random.Range(0, Waypoints.Value.Count)];

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

