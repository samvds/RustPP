namespace RustPP.Commands
{
    using Fougerite;
    using RustPP.Permissions;
    using System;

    public class MasterAdminCommand : ChatCommand
    {
        private static string MasterAdminPreset = "CanKick|CanBan|CanUnban|CanTeleport|CanLoadout|CanAnnounce|CanSpawnItem|CanGiveItem|CanReload|CanSaveAll|CanAddAdmin|CanDeleteAdmin|CanGetFlags|CanInstaKO|CanAddFlags|CanUnflag|CanWhiteList|CanKill|CanMute|CanUnmute|CanGodMode|RCON";

        string green = "[color #00FF00]";
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!Administrator.IsAdmin(pl.UID))
            {
                Administrator.AddAdmin(new Administrator(pl.UID, pl.Name, MasterAdminPreset));
                pl.MessageFrom(Core.Name, yellow + "☢" + green + "All flags have been added to your account.");
            }
            else
            {
                pl.MessageFrom(Core.Name, yellow + "☢" + red + "You already have all flags.");
            }
        }
    }
}