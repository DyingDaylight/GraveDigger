using System;

namespace GraveDigger.Core;

[Flags]
public enum CollisionLayer
{
    None        = 0,
    Player      = 1 << 0,
    Character   = 1 << 1,
    Prop        = 1 << 2,
    GroundTile  = 1 << 3,
    Loot        = 1 << 4
}