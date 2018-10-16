namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Permissions;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    internal class BanCommand : ChatCommand
    {
        string cyan = "[color #00FFFF]";
        string green = "[color #00FF00]";
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        string white = "[color #FFFFFF]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string queryName = Arguments.ArgsStr.Trim(new char[] { ' ', '"' });
            if (queryName == string.Empty)
            {
                pl.MessageFrom(Core.Name, "Use " + cyan + "/ban \"player\"" + white + " - to ban a player from the server.");
                return;
            }

            var query = from entry in Core.userCache
                        let sim = entry.Value.Similarity(queryName)
                        where sim > 0.4d
                        group new PList.Player(entry.Key, entry.Value) by sim into matches
                        select matches.FirstOrDefault();

            if (query.Count() == 1)
            {
                BanPlayer(query.First(), pl);
            }
            else
            {
                pl.MessageFrom(Core.Name, string.Format("☢ " + cyan + "{0} " + white + "players match " + cyan + "{1}" + white + ": ", query.Count(), queryName));
                for (int i = 1; i < query.Count(); i++)
                {
                    pl.MessageFrom(Core.Name, string.Format("☢ " + cyan + "{0} " + white + "- {1}", i, query.ElementAt(i).DisplayName));
                }
                pl.MessageFrom(Core.Name, "☢ " + cyan + "0" + white + " - Cancel");
                pl.MessageFrom(Core.Name, "☢ " + cyan + "Please enter the number matching the player.");
                Core.banWaitList[pl.UID] = query;
            }
        }

        public void PartialNameBan(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.MessageFrom(Core.Name, yellow + "☢" + green + "Cancelled!");
                return;
            }
            var list = Core.banWaitList[pl.UID] as IEnumerable<PList.Player>;
            BanPlayer(list.ElementAt(id), pl);
        }

        public void BanPlayer(PList.Player ban, Fougerite.Player myAdmin)
        {
            if (ban.UserID == myAdmin.UID)
            {
                myAdmin.MessageFrom(Core.Name, yellow + "☢ " + red + "You can not ban yourself!");
                return;
            }
            if (Administrator.IsAdmin(ban.UserID) && !Administrator.GetAdmin(myAdmin.UID).HasPermission("RCON"))
            {
                myAdmin.MessageFrom(Core.Name, yellow + "☢ " + red + "Player: " + yellow + ban.DisplayName + red + " is an administrator. You can't ban administrators.");
                return;
            }
            if (RustPP.Core.blackList.Contains(ban.UserID))
            {
                Logger.LogError(string.Format("[BanPlayer] {0}, id={1} is already on the blackList.", ban.DisplayName, ban.UserID));
                Core.blackList.Remove(ban.UserID);
            }
            Core.blackList.Add(ban);
            Administrator.DeleteAdmin(ban.UserID);
            Administrator.NotifyAdmins(string.Format(yellow + "☢ " + green + "Player: " + yellow + "{0}" + green + " has been banned by " + yellow + "{1}" + green + ".", ban.DisplayName, myAdmin.Name));
            Fougerite.Player client = Fougerite.Server.GetServer().FindPlayer(ban.UserID.ToString());
            if (client != null)
                client.Disconnect();
        }
    }
}