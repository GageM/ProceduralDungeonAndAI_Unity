using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This contains all the enums that are necessary for Dungeon Generation

public enum CycleType
{
    TWO_PATHS,
    TWO_KEYS,
    HIDDEN_SHORTCUT,
    DANGEROUS_ROUTE,
    FORESHADOW_LOOP,
    LOCK_KEY,
    BLOCKED_RETREAT,
    MONSTER_PATROL,
    ALTERED_RETURN,
    FALSE_GOAL,
    SIMPLE_LOCK_KEY,
    GAMBIT
}

public enum BarrierType
{
    PHYSICAL,
    MAGICAL,
    ONE_WAY_PATH,
    ONE_WAY_TRAP,
    DANGEROUS_MONSTER,
    DANGEROUS_TRAP,
    DANGEROUS_HAZARD
}

public enum CycleTheme
{
    NECROMANCER,
    CORRUPT_PALADIN,

}


public enum RoomType
{
    EMPTY_SPACE,
    DEFAULT,
    CYCLE_START,
    CYCLE_GOAL,
    LOCK_ROOM,
    KEY_ROOM,
    STRONG_ENEMY_ROOM,
    TRAP_ROOM,
    BARRIER_ROOM,
    TREASURE_ROOM
}

// The type of connection between rooms
public enum EdgeType
{ 
    CORRIDOR,
    WINDOW,
    PORTAL,
}

