using System.Security;

namespace RustPP.Commands
{
    using Fougerite;
    using RustPP.Permissions;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    internal class AddAdminCommand : ChatCommand
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
                pl.MessageFrom(Core.Name, "Use " + cyan + "/addadmin \"player\"" + white + " - to add this player to the list of administrators.");
                return;
            }
            List<Administrator> list = new List<Administrator>();
            list.Add(new Administrator(0, "Cancel"));
            Fougerite.Player fplayer = Fougerite.Server.GetServer().FindPlayer(playerName);
            if (fplayer != null)
            {
                NewAdmin(new Administrator(fplayer.UID, fplayer.Name), pl);
                return;
            }
            foreach (KeyValuePair<ulong, string> entry in Core.userCache)
            {
                if (entry.Value.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                {
                    NewAdmin(new Administrator(entry.Key, entry.Value), pl);
                    return;
                }
                if (entry.Value.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                    list.Add(new Administrator(entry.Key, entry.Value));
            }
            if (list.Count == 1)
            {
                foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
                {
                    if (client.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                    {                
                        NewAdmin(new Administrator(client.UID, SecurityElement.Escape(client.Name)), pl);
                        return;
                    }
                    if (client.Name.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                        list.Add(new Administrator(client.UID, SecurityElement.Escape(client.Name)));
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
                pl.MessageFrom(Core.Name, string.Format("☢ " + cyan + "{0} " + white + "- {1}", i, list[i].DisplayName));
            }
            pl.MessageFrom(Core.Name, "☢ " + cyan + "0" + white + " - Cancel");
            pl.MessageFrom(Core.Name, "☢ " + cyan + "Please enter the number matching the player.");
            Core.adminAddWaitList[Arguments.argUser.userID] = list;
        }

        public void PartialNameNewAdmin(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.MessageFrom(Core.Name, yellow + "☢" + green + "Cancelled!");
                return;
            }
            List<Administrator> list = (List<Administrator>)Core.adminAddWaitList[Arguments.argUser.userID];
            NewAdmin(list[id], pl);
        }

        public void NewAdmin(Administrator newAdmin, Fougerite.Player player)
        {
            if (newAdmin.UserID == player.UID)
            {
                player.MessageFrom(Core.Name, yellow + "☢ " + red + "You are already an administrator!");
            }
            else if (Administrator.IsAdmin(newAdmin.UserID))
            {
                player.MessageFrom(Core.Name, yellow + "☢ " + red + "You can't promote: " + yellow + newAdmin.DisplayName + red + " as he/she is already an administrator.");
            }
            else
            {
                string flagstr = Core.config.GetSetting("Settings", "default_admin_flags");

                if (flagstr != null)
                {
                    List<string> flags = new List<string>(flagstr.Split(new char[] { '|' }));
                    newAdmin.Flags = flags;
                }
                Administrator.AddAdmin(newAdmin);
                Administrator.NotifyAdmins(string.Format(yellow + "☢ " + green + "Player: " + yellow + "{0} " + green + "has been made an administrator by" + yellow + " {1}" + green + ".", SecurityElement.Escape(newAdmin.DisplayName), player.Name));
            }
        }
    }
}