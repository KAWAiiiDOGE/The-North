using UnityEngine;
using UnityEngine.AI;

namespace TheNorth
{
    public class NPC : Character
    {
        [Header("NPC Settings")]
        [SerializeField] private GameEntityRegardData _relationship;
    }
}
