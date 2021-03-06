﻿using System.Collections.Generic;

namespace RustPP.Commands
{
    using Fougerite;
    using RustPP.Social;
    using System;
    using System.Collections;

    public class FriendsCommand : ChatCommand
    {
        public static Hashtable friendsLists = new Hashtable();
        private List<ulong> _exc = new List<ulong>();

        public void AddTempException(ulong id)
        {
            if (!_exc.Contains(id)) _exc.Add(id);
        }

        public void RemoveTempException(ulong id)
        {
            if (_exc.Contains(id)) _exc.Remove(id);
        }

        public bool ContainsException(ulong id)
        {
            return _exc.Contains(id);
        }

        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (!friendsLists.ContainsKey(Arguments.argUser.userID))
            {
                pl.MessageFrom(Core.Name, yellow + "☢ " + red + "You currently have no friends.");
            }
            else
            {
                ((FriendList)friendsLists[pl.UID]).OutputList(ref Arguments);
            }
        }

        public Hashtable GetFriendsLists()
        {
            return friendsLists;
        }

        public void SetFriendsLists(Hashtable fl)
        {
            friendsLists = fl;
        }
    }
}