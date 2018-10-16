﻿namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class ShareCommand : ChatCommand
    {
        public static Hashtable shared_doors = new Hashtable();

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
                pl.MessageFrom(Core.Name, "Use " + cyan + "/share \"player\"" + white + " - to share your doors with a specific player.");
                return;
            }
            PList list = new PList();
            list.Add(0, "Cancel");
            foreach (KeyValuePair<ulong, string> entry in Core.userCache)
            {
                if (entry.Value.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                {
                    DoorShare(new PList.Player(entry.Key, entry.Value), pl);
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
                        DoorShare(new PList.Player(client.UID, client.Name), pl);
                        return;
                    }
                    if (client.Name.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                        list.Add(client.UID, client.Name);
                }
            }
            if (list.Count == 1)
            {
                pl.MessageFrom(Core.Name, string.Format(yellow + "☢ " + white + "No player matches the name: " + yellow + "{0}", playerName));
                return;
            }
            pl.MessageFrom(Core.Name, string.Format("☢ " + cyan + "{0} " + white + "{1}:", ((list.Count - 1)).ToString(), (((list.Count - 1) > 1) ? "Player was found:" : "Players were found:")));
            for (int i = 1; i < list.Count; i++)
            {
                pl.MessageFrom(Core.Name, string.Format("☢ " + cyan + "{0} " + white + "- {1}", i, list.PlayerList[i].DisplayName));
            }
            pl.MessageFrom(Core.Name, "☢ " + cyan + "0" + white + " - Cancel");
            pl.MessageFrom(Core.Name, "☢ " + cyan + "Please enter the number matching the player.");
            Core.shareWaitList[pl.UID] = list;
        }

        public void PartialNameDoorShare(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.MessageFrom(Core.Name, yellow + "☢" + green + "Cancelled!");
                return;
            }
            PList list = (PList)Core.shareWaitList[pl.UID];
            DoorShare(list.PlayerList[id], pl);
        }

        public void DoorShare(PList.Player friend, Fougerite.Player sharing)
        {
            if (friend.UserID == sharing.UID)
            {
                sharing.MessageFrom(Core.Name, yellow + "☢ " + red + "You can't share doors with yourself.");
                return;
            }
            ArrayList shareList = (ArrayList)shared_doors[sharing.UID];
            if (shareList == null)
            {
                shareList = new ArrayList();
                shared_doors.Add(sharing.UID, shareList);

            }
            if (shareList.Contains(friend.UserID))
            {
                sharing.MessageFrom(Core.Name, string.Format(yellow + "☢ " + red + "You have already shared doors with" + yellow + " {0}" + red + ".", friend.DisplayName));
                return;
            }
            shareList.Add(friend.UserID);
            shared_doors[sharing.UID] = shareList;
            sharing.MessageFrom(Core.Name, string.Format(yellow + "☢ " + green + "You have shared doors with" + yellow + " {0}" + green + ".", friend.DisplayName));
            Fougerite.Player client = Fougerite.Server.GetServer().FindPlayer(friend.UserID.ToString());
            if (client != null)
                client.MessageFrom(Core.Name, string.Format(yellow + "☢ " + green + "Player: " + yellow + "{0} " + green + "has shared doors with you.", sharing.Name));
        }

        public Hashtable GetSharedDoors()
        {
            return shared_doors;
        }

        public void SetSharedDoors(Hashtable sd)
        {
            shared_doors = sd;
        }
    }
}