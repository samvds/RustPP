/// <summary>
/// AdminMode is superior to this specific command, not a whole lot of attention was therfore given to it.
/// </summary>
namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using System;

    public class LoadoutCommand : ChatCommand
    {
        string cyan = "[color #00FFFF]";
        string green = "[color #00FF00]";
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        string white = "[color #FFFFFF]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            int items;
            if (int.TryParse(Core.config.GetSetting("AdminLoadout", "items"), out items))
            {
                var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
                for (int i = 1; i <= items; i++)
                {
                    string name = Core.config.GetSetting("AdminLoadout", "item" + i + "_name");
                    string amount = Core.config.GetSetting("AdminLoadout", "item" + i + "_amount");
                    Arguments.Args = new string[] { name, amount };
                    string newargs = Arguments.ArgsStr;
                    inv.give(ref Arguments);
                    Logger.LogDebug(string.Format("[Loadout] gave {0} to {1}", newargs, Arguments.argUser.displayName));
                }

                pl.MessageFrom(Core.Name, yellow + "☢" + green + "You have spawned an administrative loadout.");
            }
        }
    }
}