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
            if (command.Length == 0 || command.Length > 1)
            {
                return;
            }

            if (command.Length == 1)
            {
                if (!(caller.HasPermission("afk"))) { return; }
                UnturnedPlayer player = UnturnedPlayer.FromName(command[0]);

                if (player == null)
                {
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_other_caller_not_found") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_other_caller_not_found"), Color.yellow);
                    }
                    return;
                }

                if (caller.Id == player.Id)
                {
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_other_caller_error_self") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_other_caller_error_self", player.DisplayName), Color.yellow);
                    }
                    return;
                }

                if (player.IsAdmin)
                {
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_other_caller_error_admin") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_other_caller_error_admin", player.DisplayName), Color.yellow);
                    }
                    return;
                }

                if (EasyAFK.listAFK.Contains(player.SteamName))
                {
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_other_caller_already_afk") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_other_caller_already_afk", player.DisplayName), Color.yellow);
                    }
                    return;
                }

                EasyAFK.listAFK.Add(player.SteamName);
                if (player.Id == caller.Id)
                {
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_self") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_self"), Color.yellow);
                    }
                }
                else
                {
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_afk_chat") == string.Empty))
                    {
                        UnturnedChat.Say(EasyAFK.Instance.Translations.Instance.Translate("afk_afk_chat", player.DisplayName), Color.yellow);
                    }
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_other_player") == string.Empty))
                    {
                        UnturnedChat.Say(player, EasyAFK.Instance.Translations.Instance.Translate("afk_other_player", caller.DisplayName), Color.yellow);
                    }
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_other_caller") == string.Empty))
                    {
                        UnturnedChat.Say(caller, EasyAFK.Instance.Translations.Instance.Translate("afk_other_caller", player.DisplayName), Color.yellow);
                    }
                }
            }
        }

        public string Help
        {
            get { return "Set yourself or others afk"; }
        }

        public string Name
        {
            get { return "afk"; }
        }

        public string Syntax
        {
            get { return "<player>"; }
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
                    "afk"
                };
            }
        }
    }
}