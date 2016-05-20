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
            if ((DateTime.Now - lastCheck).TotalMilliseconds >= FeexAFK.Instance.Configuration.Instance.CheckInterval && FeexAFK.Instance.State == Rocket.API.PluginState.Loaded)
            {
                if (lastPosition != Player.Player.transform.position)
                {
                    lastPosition = Player.Player.transform.position;
                    lastActivity = DateTime.Now;
                }

                if (FeexAFK.Instance.Configuration.Instance.IgnoreAdmins && Player.IsAdmin) { return; }

                if ((DateTime.Now - lastActivity).TotalSeconds >= FeexAFK.Instance.Configuration.Instance.Seconds)
                {
                    if (!isAFK) { isAFK = true; AFK_true(); }
                }
                else
                {
                    if (isAFK) { isAFK = false; AFK_false(); }
                }

                lastCheck = DateTime.Now;
            }
        }

        public void AFK_true()
        {
            if (FeexAFK.Instance.Configuration.Instance.MessageEnabled)
            {
                UnturnedChat.Say(FeexAFK.Instance.Translations.Instance.Translate("afk_true", Player.DisplayName), Color.yellow);
            }
            if (FeexAFK.Instance.Configuration.Instance.KickEnabled && Provider.Players.Count >= FeexAFK.Instance.Configuration.Instance.KickMinPlayers)
            {
                Provider.kick(Player.CSteamID, FeexAFK.Instance.Translations.Instance.Translate("afk_kick"));
            }
        }

        public void AFK_false()
        {
            if (FeexAFK.Instance.Configuration.Instance.MessageEnabled)
            {
                UnturnedChat.Say(FeexAFK.Instance.Translations.Instance.Translate("afk_false", Player.DisplayName), Color.yellow);
            }
        }
    }
}
