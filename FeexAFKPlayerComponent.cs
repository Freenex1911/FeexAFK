using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using UnityEngine;

namespace Freenex.FeexAFK
{
    public class FeexAFKPlayerComponent : UnturnedPlayerComponent
    {
        public DateTime lastActivity = DateTime.Now;
        public bool isAFK { get; private set; }

        private DateTime lastCheck = DateTime.Now;
        private Vector3 lastPosition = new Vector3(0, 0, 0);
        
        void FixedUpdate()
        {
            if ((DateTime.Now - lastCheck).TotalMilliseconds >= FeexAFK.Instance.Configuration.Instance.CheckInterval && FeexAFK.Instance.State == PluginState.Loaded)
            {
                if (lastPosition != Player.Player.transform.position && Player.Stance != EPlayerStance.SWIM)
                {
                    lastPosition = Player.Player.transform.position;
                    lastActivity = DateTime.Now;
                }

                if ((Player.IsAdmin && FeexAFK.Instance.Configuration.Instance.IgnoreAdmins) || (!Player.IsAdmin && Player.HasPermission("afk.prevent"))) { return; }

                if ((DateTime.Now - lastActivity).TotalSeconds >= FeexAFK.Instance.Configuration.Instance.Seconds) { if (!isAFK) { AFK_true(); } }
                else { if (isAFK) { AFK_false(); } }

                lastCheck = DateTime.Now;
            }
        }

        public void AFK_true()
        {
            isAFK = true;

            if (FeexAFK.Instance.Configuration.Instance.MessageEnabled)
                UnturnedChat.Say(FeexAFK.Instance.Translations.Instance.Translate("afk_true", Player.CharacterName), Color.yellow);
            if (FeexAFK.Instance.Configuration.Instance.KickEnabled && Provider.clients.Count >= FeexAFK.Instance.Configuration.Instance.KickMinPlayers)
                Provider.kick(Player.CSteamID, FeexAFK.Instance.Translations.Instance.Translate("afk_kick"));
        }

        public void AFK_false()
        {
            isAFK = false;

            if (FeexAFK.Instance.Configuration.Instance.MessageEnabled)
                UnturnedChat.Say(FeexAFK.Instance.Translations.Instance.Translate("afk_false", Player.CharacterName), Color.yellow);
        }
    }
}
