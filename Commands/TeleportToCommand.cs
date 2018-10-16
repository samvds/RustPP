using System.Runtime.InteropServices;
using UnityEngine;

namespace RustPP.Commands
{
    using Fougerite;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class TeleportToCommand : ChatCommand
    {
        string cyan = "[color #00FFFF]";
        string green = "[color #00FF00]";
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        string white = "[color #FFFFFF]";
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate IntPtr LoadLibrary_Delegate(string lpFileName);


        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lpFileName);

        
        public static Hashtable tpWaitList = new Hashtable();
        
        public bool V3Equal(Vector3 a, Vector3 b)
        {
            return Vector3.SqrMagnitude(a - b) < 0.0001;
        }

        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (ChatArguments.Length == 3)
            {
                float n, n2, n3;
                bool b = float.TryParse(ChatArguments[0], out n);
                bool b2 = float.TryParse(ChatArguments[1], out n2);
                bool b3 = float.TryParse(ChatArguments[2], out n3);
                if (b && b2 && b3)
                {
                    pl.TeleportTo(n, n2, n3, false);
                    pl.MessageFrom(Core.Name, yellow + "☢ " + green + "You have teleported to the given coordinates!");
                    return;
                }
            }
            string playerName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if (playerName == string.Empty)
            {
                pl.MessageFrom(Core.Name, "Use " + cyan + "/tpto \"player\"" + white + " - to teleport to a specific player's location.");
                return;
            }
            List<string> list = new List<string>();
            list.Add("ToTarget");
            foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
            {
                if (client.Name.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                {
                    if (client.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase))
                    {
                        Arguments.Args = new string[] { pl.Name, client.Name };
                        if (client.IsOnline)
                        {
                            if (V3Equal(client.Location, Vector3.zero))
                            {
                                pl.MessageFrom(Core.Name, yellow + "☢ " + green + "Player: " + yellow + client.Name + " is still loading and has no position.");
                                return;
                            }
                            pl.TeleportTo(client, 1.5f, false);
                            pl.MessageFrom(Core.Name, yellow + "☢ " + green + "You have teleported to " + yellow + client.Name + green + ".");
                        }
                        else
                        {
                            pl.MessageFrom(Core.Name, yellow + "☢ " + green + "Player: " + yellow + client.Name + " seems to be offline.");
                        }
                        return;
                    }
                    list.Add(client.Name);
                }
            }
            if (list.Count != 0)
            {
                pl.MessageFrom(Core.Name, "☢ " + cyan + ((list.Count - 1)).ToString() + white + (((list.Count - 1) == 1) ? "  Player was found:" : " Players were found:"));
                for (int j = 1; j < list.Count; j++)
                {
                    pl.MessageFrom(Core.Name, "☢ " + cyan + j + white + " - " + list[j]);
                }
                pl.MessageFrom(Core.Name, "☢ " + cyan + "0" + white + " - Cancel");
                pl.MessageFrom(Core.Name, "☢ " + cyan + "Please enter the number matching the player.");
                tpWaitList[pl.UID] = list;
            } else
            {
                pl.MessageFrom(Core.Name, yellow + "☢ " + red + "No player matches the name: " + yellow + playerName + red + ".");
            }
        }

        public Hashtable GetTPWaitList()
        {
            return tpWaitList;
        }

        public void PartialNameTP(ref ConsoleSystem.Arg Arguments, int choice)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (tpWaitList.Contains(pl.UID))
            {
                System.Collections.Generic.List<string> list = (System.Collections.Generic.List<string>)tpWaitList[pl.UID];
                string str = list[choice];
                if (choice == 0)
                {
                    pl.MessageFrom(Core.Name, yellow + "☢" + green + "Cancelled!");
                    tpWaitList.Remove(pl.UID);
                }
                else
                {
                    if (list[0] == "ToTarget")
                    {
                        Arguments.Args = new string[] { pl.Name, str };
                    }
                    else
                    {
                        Arguments.Args = new string[] { str, pl.Name };
                    }
                    teleport.toplayer(ref Arguments);
                    tpWaitList.Remove(pl.UID);
                }
            }
        }
    }
}