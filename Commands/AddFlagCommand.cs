﻿namespace RustPP.Commands
{
    using Facepunch.Utility;
    using Fougerite;
    using RustPP.Permissions;
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    public class AddFlagCommand : ChatCommand
    {
        string cyan = "[color #00FFFF]";
        string green = "[color #00FF00]";
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        string white = "[color #FFFFFF]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (ChatArguments.Length <= 1)
            {
                pl.MessageFrom(Core.Name, "Use " + cyan + "/addflag \"player\" \"flag\"" + white + " - to add a flag to a specific player.");
                return;
            }

            List<string> flags = new List<string>();
            List<string> name = new List<string>();
            List<Administrator> admins = new List<Administrator>();
            admins.Add(new Administrator(0, "Cancel"));
            foreach (string argument in ChatArguments)
            {
                string arg = argument.Trim(new char[] { ' ', '"' });
                if (Administrator.IsValidFlag(arg))
                {
                    flags.Add(Administrator.GetProperName(arg));
                } else if (arg.Equals("ALL", StringComparison.InvariantCultureIgnoreCase))
                {
                    flags.Add("ALL");
                } else
                {
                    name.Add(arg);
                }            
            }
            if (flags.Count == 0)
            {
                pl.MessageFrom(Core.Name, yellow + "☢ " + red + "No valid flags were given.");
                return;
            }
            if (flags.Contains("ALL"))
            {
                flags.Clear();
                flags.AddRange(Administrator.PermissionsFlags);
            }
            Fougerite.Player matchingplayer = Fougerite.Server.GetServer().FindPlayer(name[0]);
            if (matchingplayer != null)
            {
                if (Administrator.IsAdmin(matchingplayer.UID))
                {
                    Core.adminFlagWaitList[pl.UID] = admins;
                    Core.adminFlagsList[pl.UID] = flags;
                    AddFlags(Administrator.GetAdmin(matchingplayer.UID), pl);
                }
                else
                {
                    pl.MessageFrom(Core.Name, string.Format(yellow + "☢ " + red + "You can't add any flags to:" + yellow + " {0} " + red + "as he/she is not an administrator.", string.Join(" ", name.ToArray())));
                }
                return;
            }
            List<Administrator> match = new List<Administrator>();
            for (int i = 0; i < name.Count; i++)
            {
                match.AddRange(Administrator.AdminList.FindAll(delegate(Administrator a)
                {
                    if (i == 0)
                        return a.DisplayName.Contains(name[0], true);

                    return a.DisplayName.Contains(string.Join(" ", name.GetRange(0, i).ToArray()), true);
                })
                );
            }
            if (match.Count == 0)
            {
                pl.MessageFrom(Core.Name, string.Format(yellow + "☢ " + red + "You can't add any flags to:" + yellow + " {0} " + red + "as he/she is not an administrator.", string.Join(" ", name.ToArray())));
                return;
            }
            if (match.Count == 1)
            {
                Core.adminFlagsList.Add(pl.UID, flags);
                AddFlags(match[0], pl);
                return;
            }
            admins.AddRange(match.Distinct());
            pl.MessageFrom(Core.Name,
                string.Format("☢ " + cyan + "{0} " + white + "{1}", ((admins.Count - 1)).ToString(), (((admins.Count - 1) == 1) ? "Player was found:" : "Players were found:")));

            for (int i = 1; i < admins.Count; i++)
            {
                pl.MessageFrom(Core.Name, string.Format("☢ " + cyan + "{0} " + white + "- {1}", i, admins[i].DisplayName));
            }
            pl.MessageFrom(Core.Name, "☢ " + cyan + "0" + white + " - Cancel");
            pl.MessageFrom(Core.Name, "☢ " + cyan + "Please enter the number matching the player.");
            Core.adminFlagWaitList[pl.UID] = admins;
            Core.adminFlagsList[pl.UID] = flags;
        }

        public void PartialNameAddFlags(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.MessageFrom(Core.Name, yellow + "☢" + green + "Cancelled!");
                return;
            }
            List<Administrator> list = (List<Administrator>)Core.adminFlagWaitList[pl.UID];
            AddFlags(list[id], pl);
        }

        public void AddFlags(Administrator administrator, Fougerite.Player myAdmin)
        {         
            List<string> flags = (List<string>)Core.adminFlagsList[myAdmin.UID];
            Core.adminFlagsList.Remove(myAdmin.UID);

            foreach (string properName in flags)
            {
                if (properName == "RCON" && !Administrator.GetAdmin(myAdmin.UID).HasPermission("RCON"))
                {
                    myAdmin.MessageFrom(Core.Name, yellow + "☢ " + red + "You can't add the RCON flag to anyone's permissions.");
                    Administrator.NotifyAdmins(string.Format(yellow + "☢ " + red + "Player: " + yellow + "{0}" + red + " attempted to add the " + yellow + "{1}" + red + " flag to " + yellow + "{2}'s " + red + "permissions.", myAdmin.Name, properName, administrator.DisplayName));
                } else if (administrator.HasPermission(properName))
                {
                    myAdmin.MessageFrom(Core.Name, string.Format(yellow + "☢ " + red + "You can't add this to:" + yellow + " {0} " + red + " as he/she already has the " + yellow + "{1}" + red + " flag.", administrator.DisplayName, properName));
                } else
                {
                    administrator.Flags.Add(properName);
                    Administrator.NotifyAdmins(string.Format(yellow + "☢ " + green + "Player: " + yellow + "{0}" + green + " added the " + yellow + "{1}" + green + " flag to " + yellow + "{2}'s " + green + "permissions.", myAdmin.Name, properName, administrator.DisplayName));
                    if (properName == "RCON")
                    {                           
                        Fougerite.Player adminclient = Fougerite.Server.GetServer().FindPlayer(administrator.UserID.ToString());
                        if (adminclient != null)
                            adminclient.PlayerClient.netUser.admin = true;
                    }
                }
            }
        }
    }
}
