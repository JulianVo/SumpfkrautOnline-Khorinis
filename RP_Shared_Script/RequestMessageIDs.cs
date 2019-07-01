namespace RP_Shared_Script
{
    /// <summary>
    /// Commands the client can send
    /// </summary>
    public enum RequestMessageIDs : byte
    {
        JumpFwd,
        JumpRun,
        JumpUp,

        ClimbLow,
        ClimbMid,
        ClimbHigh,

        MaxGuidedMessages, // everything above can be used by guided vobs & the hero, everything below is hero exclusive

        DrawFists,
        DrawWeapon,

        AttackForward,
        AttackLeft,
        AttackRight,
        AttackRun,

        Parry,
        Dodge,

        Aim,
        Unaim,
        Shoot,

        TakeItem,
        DropItem,
        EquipItem,
        UnequipItem,
        UseItem,

        Voice,
        HelpUp,

        MaxNPCRequests,
    }
}