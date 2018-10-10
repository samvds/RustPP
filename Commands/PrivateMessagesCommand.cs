/// <summary>
/// Tried to work around the system that was in place, without ruining the perfectly fine PM system.
/// </summary>
namespace RustPP.Commands
{
    using Facepunch.Utility;
    using Fougerite;
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    public class PrivateMessagesCommand : ChatCommand
    {
        string teal = "[color #00FFFF]";
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        string white = "[color #FFFFFF]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            Fougerite.Player sender = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (ChatArguments.Length < 2)
            {
                sender.MessageFrom(Core.Name, "Use " + teal + "/pm \"player\" \"message\"" + white + " - to send a private message.");
                return;
            }
            string search = ChatArguments[0];
            Fougerite.Player recipient = Fougerite.Player.FindByName(search);
            if (recipient == null)
            {
                sender.MessageFrom(Core.Name, yellow + "☢ " + red + "Couldn't find: " + yellow + search);
                return;
            }
            List<string> wth = ChatArguments.ToList();
            wth.Remove(wth[0]);
            string message;
            try
            {
                message = string.Join(" ", wth.ToArray()).Replace(search, "").Trim(new char[] { ' ', '"' }).Replace('"', 'ˮ');
            }
            catch
            {
                sender.MessageFrom(Core.Name, yellow + "☢ " + red + "Something went wrong. Try again.");
                return;
            }
            if (message == string.Empty)
            {
                sender.MessageFrom(Core.Name, "Use " + teal + "/pm \"player\" \"message\"" + white + " - to send a private message.");
            }
            else
            {
                recipient.MessageFrom("[PM]", teal + "[ " + white + sender.Name + teal + "→ " + white + "You " + teal + "]: " + white + message);
                sender.MessageFrom("[PM]", teal + "[ " + white + "You " + teal + "→ " + white + recipient.Name + teal + " ]: " + white + message);
                //Util.say(recipient.netPlayer, string.Format("\"PM from {0}\"", Arguments.argUser.displayName.Replace('"', 'ˮ')), string.Format("\"{0}\"", message));
                //Util.say(Arguments.argUser.networkPlayer,string.Format("\"PM to {0}\"", recipient.netUser.displayName.Replace('"', 'ˮ')),string.Format("\"{0}\"", message));
                Hashtable replies = (ChatCommand.GetCommand("r") as ReplyCommand).GetReplies();
                if (replies.ContainsKey(recipient.Name))
                    replies[recipient.Name] = sender.Name;
                else
                    replies.Add(recipient.Name, sender.Name);
            }
        }
    }
}