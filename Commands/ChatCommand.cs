namespace RustPP.Commands
{
    using Fougerite;
    using RustPP;
    using RustPP.Permissions;
    using System;
    using System.Collections.Generic;

    public abstract class ChatCommand
    {
        private string _adminFlags;
        private bool _adminRestricted;
        private string _cmd;
        private static List<ChatCommand> classInstances = new List<ChatCommand>();

        public static void AddCommand(string cmdString, ChatCommand command)
        {
            command.Command = cmdString;
            classInstances.Add(command);
        }

        public static void CallCommand(string cmd, ref ConsoleSystem.Arg arg, ref string[] chatArgs)
        {
            string red = "[color #FF0000]";
            string yellow = "[color #FFFF00]";
            var pl = Fougerite.Server.Cache[arg.argUser.userID];
            if (pl.CommandCancelList.Contains(cmd)) { return; }
            foreach (ChatCommand command in classInstances)
            {
                if (command.Command == cmd)
                {
                    if (command.Enabled)
                    {
                        if (command.AdminRestricted)
                        {
                            if (command.AdminFlags == "RCON")
                            {
                                if (arg.argUser.admin)
                                {
                                    command.Execute(ref arg, ref chatArgs);
                                }
                                else
                                {
                                    pl.MessageFrom(RustPP.Core.Name, yellow + "☢ " + red + "You need RCON to be able to use this command.");
                                }
                            }
                            else if (Administrator.IsAdmin(arg.argUser.userID))
                            {
                                if (Administrator.GetAdmin(arg.argUser.userID).HasPermission(command.AdminFlags))
                                {
                                    command.Execute(ref arg, ref chatArgs);
                                }
                                else
                                {
                                    pl.MessageFrom(RustPP.Core.Name, string.Format(yellow + "☢ " + red + "Only administrators with the" + yellow + " {0} " + red + "permission can use that command.", command.AdminFlags));
                                }
                            }
                            else
                            {
                                pl.MessageFrom(RustPP.Core.Name, yellow + "☢ " + red + "You are not permitted to use this command.");
                            }
                        }
                        else
                        {
                            command.Execute(ref arg, ref chatArgs);
                        }
                    }
                    break;
                }
            }
        }

        public abstract void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments);

        public static ChatCommand GetCommand(string cmdString)
        {
            foreach (ChatCommand command in classInstances)
            {
                if (command.Command.Remove(0, 1) == cmdString)
                {
                    return command;
                }
            }
            return null;
        }

        public string AdminFlags
        {
            get
            {
                return this._adminFlags;
            }
            set
            {
                this._adminRestricted = true;
                this._adminFlags = value;
            }
        }

        public bool AdminRestricted
        {
            get
            {
                return this._adminRestricted;
            }
        }

        public string Command
        {
            get
            {
                return this._cmd;
            }
            set
            {
                this._cmd = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return RustPP.Core.config.isCommandOn(this.Command.Remove(0, 1));
            }
        }
    }
}