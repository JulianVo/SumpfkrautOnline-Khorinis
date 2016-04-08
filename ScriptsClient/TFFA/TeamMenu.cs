﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Client.Scripts.Sumpfkraut.Menus.MainMenus;
using GUC.Client.Scripts.Sumpfkraut.GUI.MainMenu;
using GUC.Network;
using GUC.Scripts.TFFA;

namespace GUC.Client.Scripts.TFFA
{
    class TeamMenu : GUCMainMenu
    {
        public static readonly TeamMenu Menu = new TeamMenu();

        public void SetCounts(int tSpec, int tAL, int tNL)
        {
            bTeamAL.Text = "Team Gomez: " + tAL;
            bTeamAL.Enabled = tAL <= tNL;

            bTeamNL.Text = "Tetriandoch: " + tNL;
            bTeamNL.Enabled = tAL >= tNL;

            bSpec.Text = "Zuschauer: " + tSpec;
        }

        MainMenuButton bSpec, bTeamAL, bTeamNL;

        protected override void OnCreate()
        {
            Back.CreateTextCenterX("Team Wählen", 70);

            const int offset = 200;
            const int dist = 38;
            bTeamAL = AddButton("Team Gomez", "", offset + dist * 0, () => SelectTeam(Team.AL));
            bTeamNL = AddButton("Tetriandoch", "", offset + dist * 1, () => SelectTeam(Team.NL));
            bSpec = AddButton("Zuschauer", "", offset + dist * 2, () => SelectTeam(Team.Spec));

            OnEscape = MainMenu.Menu.Open;
        }

        public override void Open()
        {
            PacketWriter stream = GameClient.Client.GetMenuMsgStream();
            stream.Write((byte)MenuMsgID.OpenTeamMenu);
            GameClient.Client.SendMenuMsg(stream);
            base.Open();
        }

        public override void Close()
        {
            PacketWriter stream = GameClient.Client.GetMenuMsgStream();
            stream.Write((byte)MenuMsgID.CloseTeamMenu);
            GameClient.Client.SendMenuMsg(stream);
            base.Close();
        }

        void SelectTeam(Team team)
        {
            PacketWriter stream = GameClient.Client.GetMenuMsgStream();
            stream.Write((byte)MenuMsgID.SelectTeam);
            stream.Write((byte)team);
            GameClient.Client.SendMenuMsg(stream);
        }
    }
}