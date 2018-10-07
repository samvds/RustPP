namespace RustPP.Commands
{
    using Fougerite;
    using Rust;
    using System;

    public class AnnounceCommand : ChatCommand
    {
        string cyan = "[color #00FFFF]";
        string green = "[color #00FF00]";
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        string white = "[color #FFFFFF]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string strText = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });

            if (strText == string.Empty)
            {
                pl.MessageFrom(Core.Name, "Use " + cyan + "/announce" + white + " - to make an announcement.");
            }
            else
            {
                char ch = '☢';
                foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
                {
                    client.Notice(ch.ToString(), strText, 5f);
                }
            }   
        }
    }
}