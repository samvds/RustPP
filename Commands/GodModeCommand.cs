﻿namespace RustPP.Commands
{
    using Fougerite;
    using System;
    using System.Collections.Generic;

    public class GodModeCommand : ChatCommand
    {
        public List<ulong> userIDs = new List<ulong>();

        string cyan = "[color #00FFFF]";
        string green = "[color #00FF00]";
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        string white = "[color #FFFFFF]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (pl.CommandCancelList.Contains("god"))
            {
                if (userIDs.Contains(pl.UID))
                {
                    userIDs.Remove(pl.UID);
                    pl.PlayerClient.controllable.character.takeDamage.SetGodMode(false);
                }
                return;
            }
            if (!this.userIDs.Contains(pl.UID))
            {
                this.userIDs.Add(pl.UID);
                pl.PlayerClient.controllable.character.takeDamage.SetGodMode(true);
                if (pl.FallDamage != null) { pl.FallDamage.ClearInjury();}
                pl.MessageFrom(Core.Name, yellow + "☢ " + green + "God mode has been activated!");
            }
            else
            {
                this.userIDs.Remove(pl.UID);
                pl.PlayerClient.controllable.character.takeDamage.SetGodMode(false);
                pl.MessageFrom(Core.Name, yellow + "☢ " + green + "God mode has been deactivated!");
            }
        }

        public bool IsOn(ulong uid)
        {
            return this.userIDs.Contains(uid);
        }
    }
}