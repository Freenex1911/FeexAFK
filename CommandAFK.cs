using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Freenex.EasyAFK
{
    public class CommandExp : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!caller.HasPermission("afk.set") && !caller.HasPermission("afk.check")) { return; }

            if (command.Length == 1 && command[0] == "checkall" && caller.HasPermission("afk.check"))
            {
                if (EasyAFK.listAFK.Count == 0)
                {
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_checkall_caller_false") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_checkall_caller_false"), Color.green);
                    }
                }
                else
                {
                    System.Text.StringBuilder afkDisplayNameList = new System.Text.StringBuilder();

                    bool firstPlayer = true;
                    foreach (Steamworks.CSteamID afkPlayer in EasyAFK.listAFK)
                    {
                        if (firstPlayer) { firstPlayer = false; }
                        else { afkDisplayNameList.Append(", "); }

                        afkDisplayNameList.Append(UnturnedPlayer.FromCSteamID(afkPlayer).DisplayName);
                    }
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_checkall_caller_true") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_checkall_caller_true", EasyAFK.listAFK.Count, afkDisplayNameList.ToString()), Color.green);
                    }
                }

                return;
            }

            UnturnedPlayer player = UnturnedPlayer.FromName(command[1]);

            if (player == null && command[0] != "checkall")
            {
                if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_general_not_found") == string.Empty))
                {
                    UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_general_not_found"), Color.green);
                }
                return;
            }

            if (command.Length == 2 && command[0] == "set" && caller.HasPermission("afk.set"))
            {
                if (EasyAFK.listAFK.Contains(player.CSteamID))
                {
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_afk") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_afk", player.DisplayName), Color.green);
                    }
                }

                if (caller.Id == player.Id)
                {
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_self") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_self", player.DisplayName), Color.green);
                    }
                    return;
                }

                if (player.IsAdmin)
                {
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_admin") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_admin", player.DisplayName), Color.green);
                    }
                    return;
                }

                EasyAFK.listAFK.Add(player.CSteamID);

                if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_general_true") == string.Empty))
                {
                    UnturnedChat.Say(EasyAFK.Instance.Translations.Instance.Translate("afk_general_true", player.DisplayName), Color.yellow);
                }
                if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_set_player") == string.Empty))
                {
                    UnturnedChat.Say(player, EasyAFK.Instance.Translations.Instance.Translate("afk_set_player", caller.DisplayName), Color.green);
                }
                if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_set_caller") == string.Empty))
                {
                    UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_set_caller", player.DisplayName), Color.green);
                }

                return;
            }
            else if (command.Length == 2 && command[0] == "check" && caller.HasPermission("afk.check"))
            {
                if (EasyAFK.listAFK.Contains(player.CSteamID))
                {
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_check_caller_true") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_check_caller_true", player.DisplayName), Color.green);
                    }
                }
                else
                {
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_check_caller_false") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_check_caller_false", player.DisplayName), Color.green);
                    }
                }

                return;
            }
        }

        public string Help
        {
            get { return "Set others afk or check if they're afk"; }
        }

        public string Name
        {
            get { return "afk"; }
        }

        public string Syntax
        {
            get { return "<set/check/checkall> [<player>]"; }
        }

        public bool AllowFromConsole
        {
            get { return false; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }
        public List<string> Permissions
        {
            get
            {
                return new List<string>()
                {
                    "afk.set",
                    "afk.check"
                };
            }
        }
    }
}