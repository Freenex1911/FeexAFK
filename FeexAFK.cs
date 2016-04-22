using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Freenex.FeexAFK
{
    public class FeexAFK : RocketPlugin<FeexAFKConfiguration>
    {
        public static FeexAFK Instance;

        public static readonly List<Steamworks.CSteamID> listAFK = new List<Steamworks.CSteamID>();
        private static readonly Dictionary<Steamworks.CSteamID, Thread> dicCheckPlayers = new Dictionary<Steamworks.CSteamID, Thread>();
        private static readonly Dictionary<Steamworks.CSteamID, DateTime> dicLastActivity = new Dictionary<Steamworks.CSteamID, DateTime>();
        private DateTime lastFixedUpdate = DateTime.Now;

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList() {
                    {"afk_general_true","{0} is now afk."},
                    {"afk_general_false","{0} is no longer afk."},
                    {"afk_general_kick_msg","Kicked for being afk."},
                    {"afk_general_kick_chat","{0} is afk and has been kicked."},
                    {"afk_general_not_found","Player not found."},
                    {"afk_general_invalid_parameter","Invalid parameter."},
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

            U.Events.OnPlayerConnected += PlayerConnected;
            U.Events.OnPlayerDisconnected += PlayerDisconnected;
            
            foreach (SteamPlayer player in Provider.Players)
            {
                UnturnedPlayer UPplayer = UnturnedPlayer.FromSteamPlayer(player);

                if (!dicLastActivity.ContainsKey(UPplayer.CSteamID))
                    dicLastActivity.Add(UPplayer.CSteamID, DateTime.Now);
                else
                    dicLastActivity[UPplayer.CSteamID] = DateTime.Now;
                if (!dicCheckPlayers.ContainsKey(UPplayer.CSteamID))
                {
                    Thread t = new Thread(new ThreadStart(() =>
                    {
                        while (true)
                        {
                            Thread.Sleep(Configuration.Instance.afkCheckInterval);
                            playerCheckAFK(UPplayer);
                        }
                    }))
                    {
                        IsBackground = true
                    };
                    dicCheckPlayers.Add(UPplayer.CSteamID, t);
                    t.Start();
                }
            }

            Logger.Log("Freenex's FeexAFK has been loaded!");
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= PlayerConnected;
            U.Events.OnPlayerDisconnected -= PlayerDisconnected;
            listAFK.Clear();
            dicCheckPlayers.Clear();
            dicLastActivity.Clear();

            Logger.Log("Freenex's FeexAFK has been unloaded!");
        }

        void FixedUpdate()
        {
            if ((DateTime.Now - lastFixedUpdate).TotalMilliseconds > Configuration.Instance.afkCheckInterval)
            {
                lastFixedUpdate = DateTime.Now;
                Thread t = new Thread(delegate () {
                    foreach (SteamPlayer steamPlayer in Provider.Players)
                    {
                        UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                        if (player.Stance != EPlayerStance.SWIM)
                        {
                            updatePlayerActivity(player);
                        }
                    }
                });
                t.Start();
            }
        }

        private void playerCheckAFK(UnturnedPlayer player)
        {
            try
            {
                if (DateTime.Now.Subtract(dicLastActivity[player.CSteamID]).TotalSeconds >= (Configuration.Instance.afkSeconds))
                {
                    if (!listAFK.Contains(player.CSteamID))
                    {
                        if (player.HasPermission("afk.prevent") && !player.IsAdmin || !Configuration.Instance.afkCheckAdmins && player.IsAdmin) { return; }

                        if (Configuration.Instance.afkKick && !player.IsAdmin || Configuration.Instance.afkKickAdmins && player.IsAdmin)
                        {
                            if (!(Provider.Players.Count > Configuration.Instance.afkKickMinPlayers)) { return; }

                            if (FeexAFK.Instance.Translations.Instance.Translate("afk_general_kick_chat") != "afk_general_kick_chat")
                            {
                                UnturnedChat.Say(FeexAFK.Instance.Translations.Instance.Translate("afk_general_kick_chat", player.DisplayName), Color.yellow);
                            }
                            Logger.Log(player.CSteamID + " [" + player.CharacterName + "] has been kicked while afk.");
                            player.Kick(FeexAFK.Instance.Translations.Instance.Translate("afk_general_kick_msg"));
                        }
                        else
                        {
                            listAFK.Add(player.CSteamID);
                            if (FeexAFK.Instance.Translations.Instance.Translate("afk_general_true") != "afk_general_true")
                            {
                                UnturnedChat.Say(FeexAFK.Instance.Translations.Instance.Translate("afk_general_true", player.DisplayName), Color.yellow);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void updatePlayerActivity(UnturnedPlayer player)
        {
            try
            {
                dicLastActivity[player.CSteamID] = DateTime.Now;
                if (listAFK.Contains(player.CSteamID))
                {
                    listAFK.Remove(player.CSteamID);
                    if (FeexAFK.Instance.Translations.Instance.Translate("afk_general_false") != "afk_general_false")
                    {
                        UnturnedChat.Say(FeexAFK.Instance.Translations.Instance.Translate("afk_general_false", player.DisplayName), Color.yellow);
                    }
                }
            }
            catch { }
        }

        private void PlayerConnected(UnturnedPlayer player)
        {
            if (!dicLastActivity.ContainsKey(player.CSteamID))
                dicLastActivity.Add(player.CSteamID, DateTime.Now);
            else
                dicLastActivity[player.CSteamID] = DateTime.Now;
            if (!dicCheckPlayers.ContainsKey(player.CSteamID))
            {
                Thread t = new Thread(new ThreadStart(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(Configuration.Instance.afkCheckInterval);
                        playerCheckAFK(player);
                    }
                }))
                {
                    IsBackground = true
                };
                dicCheckPlayers.Add(player.CSteamID, t);
                t.Start();
            }
        }

        private void PlayerDisconnected(UnturnedPlayer player)
        {
            if (dicLastActivity.ContainsKey(player.CSteamID))
                dicLastActivity.Remove(player.CSteamID);
            if (dicCheckPlayers.ContainsKey(player.CSteamID))
            {
                dicCheckPlayers[player.CSteamID].Abort();
                dicCheckPlayers.Remove(player.CSteamID);
            }
            if (listAFK.Contains(player.CSteamID))
                listAFK.Remove(player.CSteamID);
        }
    }
}
