using System;
using UnityEngine;

namespace TheNorth
{
    [CreateAssetMenu(fileName = "NewGameEntityRegard", menuName = "The North/Entity Utils/Game Entity Regard Data", order = 0)]
    public class GameEntityRegardData : ScriptableObject
    {
        public GameEntityRegard[] Regards;

        private void OnValidate()
        {
            var squads = Enum.GetNames(typeof(GameEntitySquad)).Length;
            Array.Resize(ref Regards, squads);
        }

        public bool Any(GameEntitySquad squad, GameEntityRegard flags)
        {
            var gameSquad = (byte)squad;
            return gameSquad < Regards.Length ? (Regards[gameSquad] & flags) != 0 : false;
        }
    }
}
