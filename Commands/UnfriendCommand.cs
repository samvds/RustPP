namespace RustPP.Commands
{
    using Fougerite;
    using RustPP.Social;
    using System;
    using System.Collections.Generic;

    public class UnfriendCommand : ChatCommand
    {
        string cyan = "[color #00FFFF]";
        string green = "[color #00FF00]";
        string red = "[color #FF0000]";
        string yellow = "[color #FFFF00]";
        string white = "[color #FFFFFF]";
        public override void Execute(ref ConsoleSystem.Arg Arguments, ref string[] ChatArguments)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            string playerName = string.Join(" ", ChatArguments).Trim(new char[] { ' ', '"' });
            if (playerName == string.Empty)
            {
                pl.MessageFrom(Core.Name, "Use " + cyan + "/unfriend \"player\"" + white + " - to remove a specific player from your friends list.");
                return;
            }
            FriendsCommand command = (FriendsCommand)ChatCommand.GetCommand("friends");
            FriendList friendsList = (FriendList)command.GetFriendsLists()[pl.UID];
            if (friendsList == null)
            {
                pl.MessageFrom(Core.Name, yellow + "☢ " + red + "You currently have no friends to remove.");
                return;
            }
            if (friendsList.isFriendWith(playerName))
            {
                friendsList.RemoveFriend(playerName);
                pl.MessageFrom(Core.Name, yellow + "☢ " + green + "You have removed: " + yellow + playerName + green + " from your friends list.");
                if (friendsList.HasFriends())
                {
                    command.GetFriendsLists()[pl.UID] = friendsList;
                }
                else
                {
                    command.GetFriendsLists().Remove(pl.UID);
                }
            }
            else
            {
                PList list = new PList();
                list.Add(0, "Cancel");
                foreach (KeyValuePair<ulong, string> entry in Core.userCache)
                {
                    if (friendsList.isFriendWith(entry.Key) && entry.Value.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                        list.Add(entry.Key, entry.Value);
                }
                if (list.Count == 1)
                {
                    foreach (Fougerite.Player client in Fougerite.Server.GetServer().Players)
                    {
                        if (friendsList.isFriendWith(client.UID) && client.Name.ToUpperInvariant().Contains(playerName.ToUpperInvariant()))
                            list.Add(client.UID, client.Name);
                    }
                }
                if (list.Count == 1)
                {
                    pl.MessageFrom(Core.Name, string.Format(yellow + "☢ " + red + "You are not friends with {0}.", playerName));
                    return;
                }

                pl.MessageFrom(Core.Name, string.Format("☢ " + cyan + "{0} " + white + "{1}:", ((list.Count - 1)).ToString(), (((list.Count - 1) == 1) ? "Player was found:" : "Players were found:")));
                for (int i = 1; i < list.Count; i++)
                {
                    pl.MessageFrom(Core.Name, string.Format("☢ " + cyan + "{0} " + white + "- {1}", i, list.PlayerList[i].DisplayName));
                }
                pl.MessageFrom(Core.Name, "☢ " + cyan + "0" + white + " - Cancel");
                pl.MessageFrom(Core.Name, "☢ " + cyan + "Please enter the number matching the player.");
                Core.unfriendWaitList[pl.UID] = list;
            }
        }

        public void PartialNameUnfriend(ref ConsoleSystem.Arg Arguments, int id)
        {
            var pl = Fougerite.Server.Cache[Arguments.argUser.userID];
            if (id == 0)
            {
                pl.MessageFrom(Core.Name, yellow + "☢" + green + "Cancelled!");
                return;
            }
            PList list = (PList)Core.unfriendWaitList[pl.UID];
            Unfriend(list.PlayerList[id], pl);
        }

        public void Unfriend(PList.Player exfriend, Fougerite.Player unfriending)
        {
            FriendsCommand command = (FriendsCommand)ChatCommand.GetCommand("friends");
            FriendList friendsList = (FriendList)command.GetFriendsLists()[unfriending.UID];

            friendsList.RemoveFriend(exfriend.UserID);
            command.GetFriendsLists()[unfriending.UID] = friendsList;
            unfriending.MessageFrom(Core.Name, string.Format(yellow + "☢ " + green + "You have removed: " + yellow + " {0} " + green + " from your friends list.", exfriend.DisplayName));
        }
    }
}