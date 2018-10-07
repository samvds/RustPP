namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using System;

    public class AboutCommand : ChatCommand
    {
        string cyan = "[color #00FFFF]";
        string green = "[color #00FF00]";
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        string white = "[color #FFFFFF]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)

        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            pl.MessageFrom(Core.Name, "☢ Fougerite is currently running Rust++ " + cyan + "v" + Core.Version + white + ".");
            pl.MessageFrom(Core.Name, "☢ Brought to you by" + cyan + " samvds " + white + "on behalf of Fougerite.");
        }
    }
}