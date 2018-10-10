namespace RustPP.Commands
{
    using Fougerite;
    using System;

    public class PingCommand : ChatCommand
    {
        string cyan = "[color #00FFFF]";
        string white = "[color #FFFFFF]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            pl.MessageFrom(Core.Name, cyan + "☢ " + white + "Pong! Your ping is: " + cyan + pl.Ping);
        }
    }
}