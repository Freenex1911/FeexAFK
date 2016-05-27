using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;

namespace Freenex.FeexAFK
{
    public class CommandAFK : IRocketCommand
    {
        public string Name
        {
            get { return "afk"; }
        }

        public string Help
        {
            get { return "/afk <set/check/checkall> [<player>]"; }
        }

        public string Syntax
        {
            get { return "<set/check/checkall> [<player>]"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>()
                {
                    "afk.set",
                    "afk.check",
                    "afk.checkall"
                };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 2 && command[0] == "set" && caller.HasPermission("afk.set"))
            {
                UnturnedPlayer player = UnturnedPlayer.FromName(command[1]);
                if (player == null)
                {
                    UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("general_not_found"));
                }
                else
                {
                    if (player.GetComponent<FeexAFKPlayerComponent>().isAFK)
                    {
                        UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_afk", player.DisplayName)); return;
                    }
                    if (caller.Id == player.Id)
                    {
                        UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_self", player.DisplayName)); return;
                    }
                    if (FeexAFK.Instance.Configuration.Instance.IgnoreAdmins && player.IsAdmin)
                    {
                        UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_admin", player.DisplayName)); return;
                    }

                    player.GetComponent<FeexAFKPlayerComponent>().lastActivity = DateTime.Now.AddSeconds(-FeexAFK.Instance.Configuration.Instance.Seconds);
                    
                    UnturnedChat.Say(player, FeexAFK.Instance.Translations.Instance.Translate("afk_set_player", caller.DisplayName));
                    UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_set_caller", player.DisplayName));
                }
            }
            else if (command.Length == 2 && command[0] == "check" && caller.HasPermission("afk.check"))
            {
                UnturnedPlayer player = UnturnedPlayer.FromName(command[1]);
                if (player == null)
                {
                    UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("general_not_found"));
                }
                else
                {
                    if (player.GetComponent<FeexAFKPlayerComponent>().isAFK)
                    {
                        UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_check_caller_true", player.DisplayName));
                    }
                    else
                    {
                        UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_check_caller_false", player.DisplayName));
                    }
                }
            }
            else if (command.Length == 1 && command[0] == "checkall" && caller.HasPermission("afk.checkall"))
            {
                int playerCount = 0;
                StringBuilder stringBuilder = new StringBuilder();

                foreach (SteamPlayer steamPlayer in Provider.Players)
                {
                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    if (player.GetComponent<FeexAFKPlayerComponent>().isAFK)
                    {
                        playerCount ++;
                        if (stringBuilder.ToString() != string.Empty) { stringBuilder.Append(", "); }
                        stringBuilder.Append(player.DisplayName);
                    }
                }

                if (playerCount == 0)
                {
                    UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_checkall_caller_false"));
                }
                else
                {
                    UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_checkall_caller_true", Provider.Players.Count, stringBuilder.ToString()));
                }
            }
            else
            {
                UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("general_invalid_parameter"));
            }
        }
    }
}