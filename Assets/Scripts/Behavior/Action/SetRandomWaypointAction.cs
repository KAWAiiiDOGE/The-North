using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;
using TheNorth;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Set Waypoints", 
    story: "Set [Waypoints] if [EnemyBehavior] exists. Instead set [Count] random around [Center]",
    description: "Sets [Waypoints] if [EnemyBehavior] exists. Or [Count] random around [Center] instead",
    category: "Action/Custom", 
    id: "017e75db362606381633c8f2e01d63d2")]
public partial class SetRandomWaypointsAction : Action
{
    [SerializeReference] public BlackboardVariable<List<Vector3>> Waypoints;
    [SerializeReference] public BlackboardVariable<EnemyBehavior> EnemyBehavior;
    [SerializeReference] public BlackboardVariable<Transform> Center;
    [SerializeReference] public BlackboardVariable<int> Count = new(3);
    [SerializeReference] public BlackboardVariable<float> Radius = new(5f);

    protected override Status OnStart()
    {
        List<Vector3> newWaypoints;

        if (EnemyBehavior.Value.HasWaypoints)
        {
            newWaypoints = new(EnemyBehavior.Value.Waypoints.Count);
            foreach (var point in EnemyBehavior.Value.Waypoints)
                newWaypoints.Add(point.position);
        }
        else
        {
            newWaypoints = new(Count);
            for (int i = 0; i < Count; i++)
            {
                Vector3 point = Center.Value.position + Random.insideUnitSphere * Radius;
                newWaypoints.Add(point);
            }
        }
        
        Waypoints.Value.Clear();
        Waypoints.Value = newWaypoints;

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

