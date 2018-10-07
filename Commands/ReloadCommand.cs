namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Permissions;
    using System;
    using System.IO;
    using System.Collections.Generic;

    public class ReloadCommand : ChatCommand
    {
        string cyan = "[color #00FFFF]";
        string green = "[color #00FF00]";
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        string white = "[color #FFFFFF]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            Fougerite.Player sender = Fougerite.Server.Cache[Arguments.argUser.userID];
            sender.MessageFrom(Core.Name, "☢ " + cyan + "Reloading: [ " + white + "admins.xml | whitelist.xml | bans.xml " + cyan + "]");
            TimedEvents.startEvents();
            if (File.Exists(RustPPModule.GetAbsoluteFilePath("admins.xml")))
            {
                Administrator.AdminList = Helper.ObjectFromXML<List<Administrator>>(RustPPModule.GetAbsoluteFilePath("admins.xml"));
            }
            if (File.Exists(RustPPModule.GetAbsoluteFilePath("whitelist.xml")))
            {
                Core.whiteList = new PList(Helper.ObjectFromXML<List<PList.Player>>(RustPPModule.GetAbsoluteFilePath("whitelist.xml")));
            }
            else
            {
                Core.whiteList = new PList();
            }
            if (File.Exists(RustPPModule.GetAbsoluteFilePath("bans.xml")))
            {
                Core.blackList = new PList(Helper.ObjectFromXML<List<PList.Player>>(RustPPModule.GetAbsoluteFilePath("bans.xml")));
            }
            else
            {
                Core.blackList = new PList();
            }
            sender.MessageFrom(Core.Name, "☢ " + cyan + "Reloaded!");
        }
    }
}