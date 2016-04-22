using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Freenex.FeexAFK
{
    public class CommandExp : IRocketCommand
    {
        public string Name
        {
            get { return "afk"; }
        }

        public string Help
        {
            get { return "Set or check afk"; }
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
                    "afk.check"
                };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 1 && command[0] == "checkall" && caller.HasPermission("afk.check"))
            {
                if (FeexAFK.listAFK.Count == 0)
                {
                    if (FeexAFK.Instance.Translations.Instance.Translate("afk_checkall_caller_false") != "afk_checkall_caller_false")
                    {
                        UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_checkall_caller_false"));
                    }
                }
                else
                {
                    System.Text.StringBuilder afkDisplayNameList = new System.Text.StringBuilder();

                    bool firstPlayer = true;
                    foreach (Steamworks.CSteamID afkPlayer in FeexAFK.listAFK)
                    {
                        if (firstPlayer) { firstPlayer = false; }
                        else { afkDisplayNameList.Append(", "); }

                        afkDisplayNameList.Append(UnturnedPlayer.FromCSteamID(afkPlayer).DisplayName);
                    }
                    if (FeexAFK.Instance.Translations.Instance.Translate("afk_checkall_caller_true") != "afk_checkall_caller_true")
                    {
                        UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_checkall_caller_true", FeexAFK.listAFK.Count, afkDisplayNameList.ToString()));
                    }
                }

                return;
            }
            else if (command.Length == 2 && (caller.HasPermission("afk.set") || caller.HasPermission("afk.check")))
            {
                UnturnedPlayer player = UnturnedPlayer.FromName(command[1]);

                if (player == null)
                {
                    if (FeexAFK.Instance.Translations.Instance.Translate("afk_general_not_found") != "afk_general_not_found")
                    {
                        UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_general_not_found"));
                    }
                    return;
                }

                if (command[0] == "set" && caller.HasPermission("afk.set"))
                {
                    if (FeexAFK.listAFK.Contains(player.CSteamID))
                    {
                        if (FeexAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_afk") != "afk_set_caller_error_afk")
                        {
                            UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_afk", player.DisplayName));
                        }
                    }

                    if (caller.Id == player.Id)
                    {
                        if (FeexAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_self") != "afk_set_caller_error_self")
                        {
                            UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_self", player.DisplayName));
                        }
                        return;
                    }

                    if (player.IsAdmin && !FeexAFK.Instance.Configuration.Instance.afkCheckAdmins)
                    {
                        if (FeexAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_admin") != "afk_set_caller_error_admin")
                        {
                            UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_set_caller_error_admin", player.DisplayName));
                        }
                        return;
                    }

                    FeexAFK.listAFK.Add(player.CSteamID);

                    if (FeexAFK.Instance.Translations.Instance.Translate("afk_general_true") != "afk_general_true")
                    {
                        UnturnedChat.Say(FeexAFK.Instance.Translations.Instance.Translate("afk_general_true", player.DisplayName));
                    }
                    if (FeexAFK.Instance.Translations.Instance.Translate("afk_set_player") != "afk_set_player")
                    {
                        UnturnedChat.Say(player, FeexAFK.Instance.Translations.Instance.Translate("afk_set_player", caller.DisplayName));
                    }
                    if (FeexAFK.Instance.Translations.Instance.Translate("afk_set_caller") != "afk_set_caller")
                    {
                        UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_set_caller", player.DisplayName));
                    }
                }
                else if (command[0] == "check" && caller.HasPermission("afk.check"))
                {
                    if (FeexAFK.listAFK.Contains(player.CSteamID))
                    {
                        if (FeexAFK.Instance.Translations.Instance.Translate("afk_check_caller_true") != "afk_check_caller_true")
                        {
                            UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_check_caller_true", player.DisplayName));
                        }
                    }
                    else
                    {
                        if (FeexAFK.Instance.Translations.Instance.Translate("afk_check_caller_false") != "afk_check_caller_false")
                        {
                            UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_check_caller_false", player.DisplayName));
                        }
                    }
                }
            }
            else
            {
                if (FeexAFK.Instance.Translations.Instance.Translate("afk_general_invalid_parameter") != "afk_general_invalid_parameter")
                {
                    UnturnedChat.Say(caller, FeexAFK.Instance.Translations.Instance.Translate("afk_general_invalid_parameter"));
                }
            }
        }
    }
}