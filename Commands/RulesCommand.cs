namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using System;
    using System.IO;

    public class RulesCommand : ChatCommand
    {
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (File.Exists(RustPPModule.GetAbsoluteFilePath("rules.txt")))
            {
                foreach (string str in File.ReadAllLines(RustPPModule.GetAbsoluteFilePath("rules.txt")))
                {
                    pl.MessageFrom(Core.Name, str);
                }
            }
            else
            {
                pl.MessageFrom(Core.Name, yellow +"☢ " + red + "No rules are currently set.");
            }
        }
    }
}