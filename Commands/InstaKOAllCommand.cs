using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RustPP.Commands;

namespace RustPP
{
    public class InstaKOAllCommand : ChatCommand
    {
        public System.Collections.Generic.List<ulong> userIDs = new System.Collections.Generic.List<ulong>();

        string cyan = "[color #00FFFF]";
        string green = "[color #00FF00]";
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        string white = "[color #FFFFFF]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (pl.CommandCancelList.Contains("instakoall"))
            {
                if (userIDs.Contains(pl.UID))
                {
                    userIDs.Remove(pl.UID);
                    pl.MessageFrom(Core.Name, yellow + "☢ " + green + "Instant Knock-Out (all) mode has been deactivated.");
                }
                return;
            }
            if (!this.userIDs.Contains(pl.UID))
            {
                this.userIDs.Add(pl.UID);
                pl.MessageFrom(Core.Name, yellow + "☢ " + green + "Instant Knock-Out (all) mode has been activated.");
            }
            else
            {
                this.userIDs.Remove(pl.UID);
                pl.MessageFrom(Core.Name, yellow + "☢ " + green + "Instant Knock-Out (all) mode has been deactivated.");
            }
        }

        public bool IsOn(ulong uid)
        {
            return this.userIDs.Contains(uid);
        }
    }
}
