﻿namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Permissions;
    using System;
    using System.Collections.Generic;

    internal class MuteCommand : ChatCommand
    {
        string cyan = "[color #00FFFF]";
        string green = "[color #00FF00]";
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        string white = "[color #FFFFFF]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string playerName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if (playerName == string.Empty)
            {
                pl.MessageFrom(Core.Name, "Use " + cyan + "/mute \"player\"" + white + " - to mute a specific player.");
                return;
            }
            PList list = new PList();
            list.Add(0, "Cancel");
            foreach (KeyValuePair<ulong, string> entry in Core.userCache)
            {
                if (entry.Value.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                {
                    MutePlayer(new PList.Player(entry.Key, entry.Value), pl);
                    return;
                }
                if (entry.Value.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                    list.Add(entry.Key, entry.Value);
            }
            if (list.Count == 1)
            {
                foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
                {
                    if (client.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                    {
                        MutePlayer(new PList.Player(client.UID, client.Name), pl);
                        return;
                    }
                    if (client.Name.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                        list.Add(client.UID, client.Name);
                }
            }
            if (list.Count == 1)
            {
                pl.MessageFrom(Core.Name, yellow + "☢ " + red + "No player matches the name: " + yellow + playerName + red + ".");
                return;
            }
            pl.MessageFrom(Core.Name, string.Format("☢ " + cyan + "{0} " + white + "{1}:", ((list.Count - 1)).ToString(), (((list.Count - 1) == 1) ? "Player was found:" : "Players were found:")));
            for (int i = 1; i < list.Count; i++)
            {
                pl.MessageFrom(Core.Name, string.Format("☢ " + cyan + "{0} " + white + "- {1}", i, list.PlayerList[i].DisplayName));
            }
            pl.MessageFrom(Core.Name, "☢ " + cyan + "0" + white + " - Cancel");
            pl.MessageFrom(Core.Name, "☢ " + cyan + "Please enter the number matching the player.");
            Core.muteWaitList[pl.UID] = list;
        }

        public void PartialNameMute(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.MessageFrom(Core.Name, yellow + "☢" + green + "Cancelled!");
                return;
            }
            PList list = (PList)Core.muteWaitList[pl.UID];
            MutePlayer(list.PlayerList[id], pl);
        }

        public void MutePlayer(PList.Player mute, Fougerite.Player myAdmin)
        {
            if (mute.UserID == myAdmin.UID)
            {
                myAdmin.MessageFrom(Core.Name, yellow + "☢ " + red + "No point in muting yourself, just be quiet.");
                return;
            }
            if (Core.muteList.Contains(mute.UserID))
            {
                myAdmin.MessageFrom(Core.Name, string.Format("{0} is already muted.", mute.DisplayName));
                return;
            }
            if (Administrator.IsAdmin(mute.UserID))
            {
                Administrator mutingAdmin = Administrator.GetAdmin(myAdmin.UID);
                Administrator mutedAdmin = Administrator.GetAdmin(mute.UserID);
                if (mutedAdmin.HasPermission("CanUnmute") || mutedAdmin.HasPermission("CanAddFlags") || mutedAdmin.HasPermission("RCON"))
                {
                    if (!mutedAdmin.HasPermission("RCON"))
                    {
                        if (mutingAdmin.HasPermission("RCON") || mutingAdmin.HasPermission("CanUnflag"))
                        {
                            mutedAdmin.Flags.Remove("CanUnmute");
                            mutedAdmin.Flags.Remove("CanMute");
                            mutedAdmin.Flags.Remove("CanAddFlags");
                            mutedAdmin.Flags.Remove("CanUnflag");
                        }
                    }
                    else
                    {
                        myAdmin.MessageFrom(Core.Name, string.Format(yellow + "☢ " + red + "Player: " + yellow + "{0}" + red + " is an administrator. You can't mute administrators.", mute.DisplayName));
                        //return;
                    }
                }
            }
            Core.muteList.Add(mute);
            Administrator.NotifyAdmins(string.Format(yellow + "☢ " + green + "Player: " + yellow + "{0}" + green + " has been muted by " + yellow + "{1}" + green + ".", mute.DisplayName, myAdmin.Name));
        }
    }
}