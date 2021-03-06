﻿namespace RP_Shared_Script
{
    public enum ScriptMessages
    {
        GameInfo,

        //Chat
        ChatMessage,
        ChatTeamMessage,
        ChatPrivateMessage,

        PlayerInfo,
        PlayerInfoTeam,
        PlayerQuit,



        ModeStart,

        //Login
        Login,
        LoginAcknowledge,
        LoginDenied,
        Logout,
        LogoutAcknowledged,

        //Account
        CreateAccount,
        AccountCreationResult,

        //Character
        RequestCharacterList,
        CharacterListResult,
        CreateCharacter,
        CharacterCreationResult,

        //Join Game
        JoinGame,
        JoinGameResult,
        LeaveGame,
    }
}
