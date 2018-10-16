namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Permissions;
    using System;
    using System.Collections.Generic;

    internal class KickCommand : ChatCommand
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
                pl.MessageFrom(Core.Name, "Use " + cyan + "/kick \"player\"" + white + " - to kick a specific player from the server.");
            }
            PList list = new PList();
            list.Add(0, "Cancel");
            foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
            {
                if (client.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                {
                    KickPlayer(client, pl);
                    return;
                } else if (client.Name.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                    list.Add(client.UID, client.Name);
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
            Core.kickWaitList[pl.UID] = list;
        }

        public void PartialNameKick(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.MessageFrom(Core.Name, yellow + "☢" + green + "Cancelled!");
                return;
            }
                
            PList list = (PList)Core.kickWaitList[pl.UID];
            Fougerite.Player client = Fougerite.Server.GetServer().FindPlayer(list.PlayerList[id].UserID.ToString());
            if (client != null) KickPlayer(client, pl);
        }

        public void KickPlayer(Fougerite.Player badPlayer, Fougerite.Player myAdmin)
        {
            if (badPlayer == myAdmin)
            {
                myAdmin.MessageFrom(Core.Name, yellow + "☢ " + red + "You can't kick yourself.");
            } else if (Administrator.IsAdmin(badPlayer.UID) && !Administrator.GetAdmin(myAdmin.UID).HasPermission("RCON"))
            {
                myAdmin.MessageFrom(Core.Name, yellow + "☢ " + red + "You can't kick: " + yellow + badPlayer.Name + red + " he/she is an administrator.");
            } else
            {
                Administrator.NotifyAdmins(string.Format(yellow + "☢ " + green + "Player: " + yellow + "{0}" + green + " has been kicked by " + yellow + "{1}" + green + ".", badPlayer.Name, myAdmin.Name));
                badPlayer.Disconnect();
            }
        }
    }
}