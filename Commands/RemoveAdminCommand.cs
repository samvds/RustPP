namespace RustPP.Commands
{
    using Fougerite;
    using RustPP.Permissions;
    using System;
    using System.Collections.Generic;

    internal class RemoveAdminCommand : ChatCommand
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
                pl.MessageFrom(Core.Name, "Use " + cyan + "/unadmin \"player\"" + white + " - to remove this player from the list of administrators.");
                return;
            }
            Fougerite.Player p = Server.GetServer().FindPlayer(playerName);
            if (p != null)
            {
                Administrator nadministrator = Administrator.AdminList.Find(delegate (Administrator obj)
                {
                    return obj.UserID == p.UID;
                });
                if (nadministrator != null)
                {
                    RemoveAdmin(nadministrator, pl);
                    return;
                }
            }
            List<Administrator> list = new List<Administrator>();
            list.Add(new Administrator(0, "Cancel"));
            Administrator administrator = Administrator.AdminList.Find(delegate(Administrator obj)
            {
                return obj.DisplayName.Equals(playerName, StringComparison.OrdinalIgnoreCase);
            });
            if (administrator != null)
            {
                RemoveAdmin(administrator, pl);
                return;
            }
            list.AddRange(Administrator.AdminList.FindAll(delegate(Administrator obj)
            {
                return obj.DisplayName.ToUpperInvariant().Contains(playerName.ToUpperInvariant());
            }));
            if (list.Count == 1)
            {
                pl.MessageFrom(Core.Name, string.Format(yellow + "☢ " + white + "No player matches the name: " + yellow + "{0}", playerName));
                return;
            }
            pl.MessageFrom(Core.Name, string.Format("{0}  player{1} {2}: ", ((list.Count - 1)).ToString(), (((list.Count - 1) > 1) ? "s match" : " matches"), playerName));
            for (int i = 1; i < list.Count; i++)
            {
                pl.MessageFrom(Core.Name, string.Format("{0} - {1}", i, list[i].DisplayName));
            }
            pl.MessageFrom(Core.Name, "☢ " + cyan + "0 - Cancel");
            pl.MessageFrom(Core.Name, "☢ " + cyan + "Please enter the number matching the player.");
            Core.adminRemoveWaitList[pl.UID] = list;
        }

        public void PartialNameRemoveAdmin(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.MessageFrom(Core.Name, yellow + "☢" + green + "Cancelled!");
                return;
            }
            List<Administrator> list = (List<Administrator>)Core.adminRemoveWaitList[pl.UID];
            RemoveAdmin(list[id], pl);
        }

        public void RemoveAdmin(Administrator exAdmin, Fougerite.Player myAdmin)
        {
            if (exAdmin.UserID == myAdmin.UID)
            {
                myAdmin.MessageFrom(Core.Name, yellow + "☢" + red + "You can't take your own administrative rank..");
            }
            else
            {
                Administrator.NotifyAdmins(string.Format("{0} is no longer an administrator; removed by {1}.", exAdmin.DisplayName, myAdmin.Name));
                Administrator.DeleteAdmin(exAdmin.UserID);
            }
        }
    }
}