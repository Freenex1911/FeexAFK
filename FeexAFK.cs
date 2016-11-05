using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;

namespace Freenex.FeexAFK
{
    public class FeexAFK : RocketPlugin<FeexAFKConfiguration>
    {
        public static FeexAFK Instance;

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList() {
                    {"general_not_found","Player not found."},
                    {"general_invalid_parameter","Invalid parameter."},
                    {"afk_true","{0} is now afk."},
                    {"afk_false","{0} is no longer afk."},
                    {"afk_kick","Kicked for being afk."},
                    {"afk_set_player","You were set afk by {0}."},
                    {"afk_set_caller","You set {0} afk."},
                    {"afk_set_caller_error_afk","{0} is already afk."},
                    {"afk_set_caller_error_self","You can't set yourself afk."},
                    {"afk_set_caller_error_admin","You can't set Admins afk."},
                    {"afk_check_caller_true","{0} is afk."},
                    {"afk_check_caller_false","{0} is not afk."},
                    {"afk_checkall_caller_true","{0} player/s afk: {1}"},
                    {"afk_checkall_caller_false","No players are afk."}
                };
            }
        }

        protected override void Load()
        {
            Instance = this;
            UnturnedPlayerEvents.OnPlayerChatted += UnturnedPlayerEvents_OnPlayerChatted;

            Logger.Log("Freenex's FeexAFK has been loaded!");
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerChatted -= UnturnedPlayerEvents_OnPlayerChatted;

            Logger.Log("Freenex's FeexAFK has been unloaded!");
        }

        private void UnturnedPlayerEvents_OnPlayerChatted(UnturnedPlayer player, ref UnityEngine.Color color, string message, EChatMode chatMode, ref bool cancel)
        {
            player.GetComponent<FeexAFKPlayerComponent>().lastActivity = DateTime.Now;
        }
    }
}
