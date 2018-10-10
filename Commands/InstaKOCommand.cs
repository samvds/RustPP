namespace RustPP.Commands
{
    using Fougerite;
    using System;
    using System.Collections.Generic;

    public class InstaKOCommand : ChatCommand
    {
        public System.Collections.Generic.List<ulong> userIDs = new System.Collections.Generic.List<ulong>();

        string green = "[color #00FF00]";
        string yellow = "[color #FFFF00]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (pl.CommandCancelList.Contains("instako"))
            {
                if (userIDs.Contains(pl.UID))
                {
                    userIDs.Remove(pl.UID);
                    pl.MessageFrom(Core.Name, yellow + "☢ " + green + "Instant Knock-Out mode has been deactivated.");
                }
                return;
            }
            if (!this.userIDs.Contains(pl.UID))
            {
                this.userIDs.Add(pl.UID);
                pl.MessageFrom(Core.Name, yellow + "☢ " + green + "Instant Knock-Out mode has been activated.");
            }
            else
            {
                this.userIDs.Remove(pl.UID);
                pl.MessageFrom(Core.Name, yellow + "☢ " + green + "Instant Knock-Out mode has been deactivated.");
            }
        }

        public bool IsOn(ulong uid)
        {
            return this.userIDs.Contains(uid);
        }
    }
}