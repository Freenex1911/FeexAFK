﻿using System;
using System.Collections.Generic;
using System.Threading;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using UnityEngine;

namespace Freenex.EasyAFK
{
    public class EasyAFK : RocketPlugin<EasyAFKConfiguration>
    {
        public static EasyAFK Instance;

        public static readonly List<string> listAFK = new List<string>();
        private static readonly Dictionary<string, Thread> dicCheackPlayers = new Dictionary<string, Thread>();
        private static readonly Dictionary<string, DateTime> dicLastActivity = new Dictionary<string, DateTime>();
        private static List<string> playerList = new List<string>();

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList() {
                    {"afk_afk_chat","{0} is now afk."},
                    {"afk_back_chat","{0} is no longer afk."},
                    {"afk_kick_msg","Kicked for being afk."},
                    {"afk_self","You are now afk."},
                    {"afk_other_player","You were set to afk by {0}."},
                    {"afk_other_caller","You set {0} to afk."},
                    {"afk_other_caller_not_found","Player not found."},
                    {"afk_other_caller_already_afk","{0} is already afk."},
                    {"afk_kick_chat","{0} is afk and has been kicked."}
                };
            }
        }

        protected override void Load()
        {
            U.Events.OnPlayerConnected += PlayerConnected;
            U.Events.OnPlayerDisconnected += PlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerChatted += UnturnedPlayerEvents_OnPlayerChatted;
            UnturnedPlayerEvents.OnPlayerInventoryAdded += UnturnedPlayerEvents_OnPlayerInventoryAdded;
            UnturnedPlayerEvents.OnPlayerInventoryRemoved += UnturnedPlayerEvents_OnPlayerInventoryRemoved;
            UnturnedPlayerEvents.OnPlayerInventoryResized += UnturnedPlayerEvents_OnPlayerInventoryResized;
            UnturnedPlayerEvents.OnPlayerInventoryUpdated += UnturnedPlayerEvents_OnPlayerInventoryUpdated;
            UnturnedPlayerEvents.OnPlayerUpdatePosition += UnturnedPlayerEvents_OnPlayerUpdatePosition;
            UnturnedPlayerEvents.OnPlayerUpdateStat += UnturnedPlayerEvents_OnPlayerUpdateStat;

            foreach (string playername in playerList)
            {
                UnturnedPlayer player = UnturnedPlayer.FromName(playername);
                if (!dicLastActivity.ContainsKey(player.SteamName))
                    dicLastActivity.Add(player.SteamName, DateTime.Now);
                else
                    dicLastActivity[player.SteamName] = DateTime.Now;
                if (!dicCheackPlayers.ContainsKey(player.SteamName))
                {
                    Thread checkThread = new Thread(new ThreadStart(() =>
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
                    dicCheackPlayers.Add(player.SteamName, checkThread);
                    checkThread.Start();
                }
            }

            Logger.Log("Freenex's EasyAFK has been loaded!");
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= PlayerConnected;
            U.Events.OnPlayerDisconnected -= PlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerChatted -= UnturnedPlayerEvents_OnPlayerChatted;
            UnturnedPlayerEvents.OnPlayerInventoryAdded -= UnturnedPlayerEvents_OnPlayerInventoryAdded;
            UnturnedPlayerEvents.OnPlayerInventoryRemoved -= UnturnedPlayerEvents_OnPlayerInventoryRemoved;
            UnturnedPlayerEvents.OnPlayerInventoryResized -= UnturnedPlayerEvents_OnPlayerInventoryResized;
            UnturnedPlayerEvents.OnPlayerInventoryUpdated -= UnturnedPlayerEvents_OnPlayerInventoryUpdated;
            UnturnedPlayerEvents.OnPlayerUpdatePosition -= UnturnedPlayerEvents_OnPlayerUpdatePosition;
            UnturnedPlayerEvents.OnPlayerUpdateStat -= UnturnedPlayerEvents_OnPlayerUpdateStat;
            dicCheackPlayers.Clear();

            Logger.Log("Freenex's EasyAFK has been unloaded!");
        }

        private void playerCheckAFK(UnturnedPlayer player)
        {
            try
            {
                if (DateTime.Now.Subtract(dicLastActivity[player.SteamName]).TotalSeconds >= (Configuration.Instance.afkSeconds))
                {
                    if (!listAFK.Contains(player.SteamName))
                    {
                        if (Configuration.Instance.afkKick)
                        {
                            player.Kick(EasyAFK.Instance.Translations.Instance.Translate("afk_kick_msg"));
                            if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_kick_chat") == string.Empty))
                            {
                                UnturnedChat.Say(EasyAFK.Instance.Translations.Instance.Translate("afk_kick_chat", player.DisplayName));
                            }
                        }
                        else
                        {
                            listAFK.Add(player.SteamName);
                            if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_afk_chat") == string.Empty))
                            {
                                UnturnedChat.Say(EasyAFK.Instance.Translations.Instance.Translate("afk_afk_chat", player.DisplayName));
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void updatePlayerActivity(UnturnedPlayer player)
        {
            if (player.HasPermission("afk.prevent")) { return; }

            try
            {
                dicLastActivity[player.SteamName] = DateTime.Now;
                if (listAFK.Contains(player.SteamName))
                {
                    listAFK.Remove(player.SteamName);
                    if (!(EasyAFK.Instance.Translations.Instance.Translate("afk_back_chat") == string.Empty))
                    {
                        UnturnedChat.Say(EasyAFK.Instance.Translations.Instance.Translate("afk_back_chat", player.DisplayName));
                    }
                }
            }
            catch { }
        }

        private void PlayerConnected(UnturnedPlayer player)
        {
            playerList.Add(player.CharacterName);
            if (!dicLastActivity.ContainsKey(player.SteamName))
                dicLastActivity.Add(player.SteamName, DateTime.Now);
            else
                dicLastActivity[player.SteamName] = DateTime.Now;
            if (!dicCheackPlayers.ContainsKey(player.SteamName))
            {
                Thread t = new Thread(new ThreadStart(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(1000);
                        playerCheckAFK(player);
                    }
                }))
                {
                    IsBackground = true
                };
                dicCheackPlayers.Add(player.SteamName, t);
                t.Start();
            }
        }

        private void PlayerDisconnected(UnturnedPlayer player)
        {
            playerList.Remove(player.CharacterName);
            if (dicLastActivity.ContainsKey(player.SteamName))
                dicLastActivity.Remove(player.SteamName);
            if (dicCheackPlayers.ContainsKey(player.SteamName))
            {
                dicCheackPlayers[player.SteamName].Abort();
                dicCheackPlayers.Remove(player.SteamName);
            }
            if (listAFK.Contains(player.SteamName))
                listAFK.Remove(player.SteamName);
        }

        private void UnturnedPlayerEvents_OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, SDG.Unturned.EChatMode chatMode, ref bool cancel)
        {
            updatePlayerActivity(player);
        }

        private void UnturnedPlayerEvents_OnPlayerInventoryAdded(UnturnedPlayer player, Rocket.Unturned.Enumerations.InventoryGroup inventoryGroup, byte inventoryIndex, SDG.Unturned.ItemJar P)
        {
            updatePlayerActivity(player);
        }

        private void UnturnedPlayerEvents_OnPlayerInventoryRemoved(UnturnedPlayer player, Rocket.Unturned.Enumerations.InventoryGroup inventoryGroup, byte inventoryIndex, SDG.Unturned.ItemJar P)
        {
            updatePlayerActivity(player);
        }

        private void UnturnedPlayerEvents_OnPlayerInventoryResized(UnturnedPlayer player, Rocket.Unturned.Enumerations.InventoryGroup inventoryGroup, byte O, byte U)
        {
            updatePlayerActivity(player);
        }

        private void UnturnedPlayerEvents_OnPlayerInventoryUpdated(UnturnedPlayer player, Rocket.Unturned.Enumerations.InventoryGroup inventoryGroup, byte inventoryIndex, SDG.Unturned.ItemJar P)
        {
            updatePlayerActivity(player);
        }

        private void UnturnedPlayerEvents_OnPlayerUpdatePosition(UnturnedPlayer player, Vector3 position)
        {
            updatePlayerActivity(player);
        }

        private void UnturnedPlayerEvents_OnPlayerUpdateStat(UnturnedPlayer player, SDG.Unturned.EPlayerStat stat)
        {
            updatePlayerActivity(player);
        }
    }
}