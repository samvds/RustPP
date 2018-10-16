namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Permissions;
    using System;
    using System.Collections.Generic;

    internal class UnmuteCommand : ChatCommand
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
                pl.MessageFrom(Core.Name, "Use " + cyan + "/unmute \"player\"" + white + " - to unmute a specific player.");
                return;
            }
            PList list = new PList();
            list.Add(0, "Cancel");
            foreach (PList.Player muted in Core.muteList.PlayerList)
            {
                Logger.LogDebug(string.Format("[UnmuteCommand] muted.DisplayName={0}, playerName={1}", muted.DisplayName, playerName));
                if (muted.DisplayName.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                {
                    UnmutePlayer(muted, pl);
                    return;
                }
                if (muted.DisplayName.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                    list.Add(muted);
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
            Core.unmuteWaitList[pl.UID] = list;
        }

        public void PartialNameUnmute(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.MessageFrom(Core.Name, yellow + "☢" + green + "Cancelled!");
                return;
            }
            PList list = (PList)Core.unmuteWaitList[pl.UID];
            UnmutePlayer(list.PlayerList[id], pl);
        }

        public void UnmutePlayer(PList.Player unmute, Fougerite.Player myAdmin)
        {
            Core.muteList.Remove(unmute.UserID);
            Administrator.NotifyAdmins(string.Format(yellow + "☢ " + green + "Player: " + yellow + "{0}" + green + " has been unmuted by " + yellow + "{1}" + green + ".", unmute.DisplayName, myAdmin.Name));
            Fougerite.Player client = Fougerite.Server.GetServer().FindPlayer(unmute.UserID.ToString());
            if (client != null)
                client.MessageFrom(Core.Name, string.Format(yellow + "☢ " + green + "You have been unmuted by: " + yellow + " {0} " + green + ".", myAdmin.Name));
        }
    }
}