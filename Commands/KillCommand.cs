namespace RustPP.Commands
{
    using Fougerite;
    using System;
    using RustPP;
    using RustPP.Permissions;
    using System.Collections.Generic;

    internal class KillCommand : ChatCommand
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
                pl.MessageFrom(Core.Name, "Use " + cyan + "/kill \"player\"" + white + " - to kill a specific player.");
            }
            PList list = new PList();
            list.Add(new PList.Player(0, "Cancel"));
            foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
            {
                if (client.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                {
                    KillPlayer(client, pl);
                    return;
                }
                if (client.Name.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
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
            pl.MessageFrom(Core.Name, "☢ " + cyan + "0" + white + " - Cancel");
            pl.MessageFrom(Core.Name, "☢ " + cyan + "Please enter the number matching the player.");
            Core.killWaitList[pl.UID] = list;
        }

        public void PartialNameKill(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.MessageFrom(Core.Name, yellow + "☢" + green + "Cancelled!");
                return;
            }
            PList list = (PList)Core.killWaitList[pl.UID];
            Fougerite.Player client = Fougerite.Server.GetServer().FindPlayer(list.PlayerList[id].UserID.ToString());
            if (client != null)
                KillPlayer(client, pl);
        }

        public void KillPlayer(Fougerite.Player victim, Fougerite.Player myAdmin)
        {
            if (victim == myAdmin)
            {
                myAdmin.MessageFrom(Core.Name, yellow + "☢ " + red + "Suicide isn't painless. " + yellow + Core.Name + red + " won't let you kill yourself.");
            }
            else if (Administrator.IsAdmin(victim.UID) && !Administrator.GetAdmin(victim.UID).HasPermission("RCON"))
            {
                myAdmin.MessageFrom(Core.Name, yellow + "☢ " + red + "You can't kill: " + yellow + victim.Name + red + " he/she is an administrator.");
            }
            else
            {
                Administrator.NotifyAdmins(string.Format(yellow + "☢ " + green + "Player: " + yellow + "{1}" + green + " has been killed by " + yellow + "{0}" + green + ".", myAdmin.Name, victim.Name));
                victim.Kill();
                //TakeDamage.Kill(myAdmin.PlayerClient.netUser.playerClient, victim.PlayerClient.netUser.playerClient, null);
            }
        }
    }
}