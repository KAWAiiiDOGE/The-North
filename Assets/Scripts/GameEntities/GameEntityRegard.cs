using System;

namespace TheNorth
{
    [Flags]
    public enum GameEntityRegard
    {
        Neutral = 0, // default for all entities
        Fear = 1 << 0,
        Hostile = 1 << 1,
        Friendly = 1 << 2,
    }
}
