namespace RustPP.Commands
{
    using Fougerite;
    using System;
    using RustPP;
    using RustPP.Permissions;
    using System.Collections.Generic;

    internal class WhiteListAddCommand : ChatCommand
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
                pl.MessageFrom(Core.Name, "Use " + cyan + "/addwl \"player\"" + white + " - to add a player to the whitelist.");
                return;
            }
            PList list = new PList();
            list.Add(0, "Cancel");
            foreach (KeyValuePair<ulong, string> entry in Core.userCache)
            {
                if (entry.Value.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                {
                    Whitelist(new PList.Player(entry.Key, entry.Value), pl);
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
                        Whitelist(new PList.Player(client.UID, client.Name), pl);
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
            pl.MessageFrom(Core.Name, string.Format("{0}  player{1} {2}: ", ((list.Count - 1)).ToString(), (((list.Count - 1) > 1) ? "s match" : " matches"), playerName));
            for (int i = 1; i < list.Count; i++)
            {
                pl.MessageFrom(Core.Name, string.Format("{0} - {1}", i, list.PlayerList[i].DisplayName));
            }
            pl.MessageFrom(Core.Name, "☢ " + cyan + "0 - Cancel");
            pl.MessageFrom(Core.Name, "☢ " + cyan + "Please enter the number matching the player.");
            Core.whiteWaitList[pl.UID] = list;
        }

        public void PartialNameWhitelist(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (Core.whiteWaitList.Contains(pl.UID))
            {
                if (id == 0)
                {
                    pl.MessageFrom(Core.Name, yellow + "☢" + green + "Cancelled!");
                    return;
                }
                PList list = (PList)Core.whiteWaitList[pl.UID];
                Whitelist(list.PlayerList[id], pl);
            }
        }

        public void Whitelist(PList.Player white, Fougerite.Player myAdmin)
        {
            if (Core.whiteList.Contains(white.UserID))
            {
                myAdmin.MessageFrom(Core.Name, yellow + "☢" + green + "Player: " + yellow + white.DisplayName + green + " is already whitelisted.");
            } else
            {
                Core.whiteList.Add(white);
                Administrator.NotifyAdmins(string.Format("{0} has been whitelisted by {1}.", white.DisplayName, myAdmin.Name));
            }
        }
    }
}