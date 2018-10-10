namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using System;

    public class SaveAllCommand : ChatCommand
    {
        string green = "[color #00FF00]";
        string yellow = "[color #FFFF00]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            AvatarSaveProc.SaveAll();
            pl.MessageFrom(Core.Name, yellow + "☢ " + green + "Saved all avatar files...");
            ServerSaveManager.AutoSave();
            pl.MessageFrom(Core.Name, yellow + "☢ " + green + "Saved all server files...");
            Helper.CreateSaves();
            pl.MessageFrom(Core.Name, yellow + "☢ " + green + "And saved " + Core.Name + " data!");
        }
    }
}