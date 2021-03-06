namespace CataParser.Constants;

public enum UnitFlags : uint
{
    TYPE_OBJECT = 0x00004000,
    TYPE_GUARDIAN = 0x00002000,
    TYPE_PET = 0x00001000,
    TYPE_NPC = 0x00000800,
    TYPE_PLAYER = 0x00000400,
    CONTROL_NPC = 0x00000200,
    CONTROL_PLAYER = 0x00000100,
    REACTION_HOSTILE = 0x00000040,
    REACTION_NEUTRAL = 0x00000020,
    REACTION_FRIENDLY = 0x00000010,
    AFFILIATION_OUTSIDER = 0x00000008,
    AFFILIATION_RAID = 0x00000004,
    AFFILIATION_PARTY = 0x00000002,
    AFFILIATION_MINE = 0x00000001,
    NONE = 0x80000000,
    TARGET_SKULL = 0x08000000,
    TARGET_CROSS = 0x04000000,
    TARGET_SQUARE = 0x02000000,
    TARGET_MOON = 0x01000000,
    TARGET_TRIANGLE = 0x00800000,
    TARGET_DIAMOND = 0x00400000,
    TARGET_CIRCLE = 0x00200000,
    TARGET_START = 0x00100000,
    MAIN_ASSIST = 0x00080000,
    MAIN_TANK = 0x00040000,
    FOCUS_TARGET = 0x00020000,
    TARGET = 0x00010000
}
