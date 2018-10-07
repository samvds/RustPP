namespace RustPP.Commands
{
    using Fougerite;
    using System;
    using System.Collections;

    public class ReplyCommand : ChatCommand
    {
        private Hashtable replies = new Hashtable();

        string teal = "[color #00FFFF]";
        string green = "[color #00FF00]";
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        string white = "[color #FFFFFF]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            Fougerite.Player sender = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (ChatArguments.Length >= 1)
            {
                if (this.replies.ContainsKey(sender.Name))
                {
                    string replyTo = (string) this.replies[sender.Name];
                    Fougerite.Player recipient = Fougerite.Server.GetServer().FindPlayer(replyTo);
                    if (recipient == null)
                    {
                        sender.MessageFrom(Core.Name, yellow + "☢ " + red + "Couldn't find: " + yellow + replyTo);
                        this.replies.Remove(sender.Name);
                        return;
                    }
                    string message = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' }).Replace('"', 'ˮ');
                    if (message == string.Empty)
                    {
                        sender.MessageFrom(Core.Name, "Use " + teal + "/r \"message\"" + white + " - to respond to a private message.");
                        return;
                    }

                    recipient.MessageFrom("[PM]", teal + "[ " + white + sender.Name + teal + "→ " + white + "You " + teal + "]: " + white + message);
                    sender.MessageFrom("[PM]", teal + "[ " + white + "You " + teal + "→ " + white + recipient.Name + teal + " ]: " + white + message);
                    if (this.replies.ContainsKey(replyTo))
                    {
                        this.replies[replyTo] = sender.Name;
                    }
                    else
                    {
                        this.replies.Add(replyTo, sender.Name);
                    }
                }
                else
                {
                    sender.MessageFrom(Core.Name, yellow + "☢ " + red + "There's no message to be answered.");
                }
            }
            else
            {
                sender.MessageFrom(Core.Name, "Use " + teal + "/r \"message\"" + white + " - to respond to a private message.");
            }
        }

        public Hashtable GetReplies()
        {
            return this.replies;
        }

        public void SetReplies(Hashtable rep)
        {
            this.replies = rep;
        }
    }
}