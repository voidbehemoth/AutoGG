using Game.Interface;
using HarmonyLib;
using Home.Services;
using Server.Shared.State;
using Services;
using SML;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using BetterTOS2;

namespace AutoGG
{

    [HarmonyPatch(typeof(EndgameWrapupOverlayController))]
    public class HandleListeners
    {
        [HarmonyPostfix]
        [HarmonyPatch("InitializeListeners")]
        public static void InitializeListeners()
        {
            AutoGG.InitializeAutoGGListeners();
        }

        [HarmonyPostfix]
        [HarmonyPatch("OnDestroy")]
        public static void OnDestroy()
        {
            AutoGG.RemoveAutoGGListeners();
        }
    }

    public static class AutoGG
    {

        public static void InitializeAutoGGListeners()
        {
            StateProperty<GameResults> gameResults = Service.Game.Sim.simulation.gameResults;
            gameResults.OnChanged = (Action<GameResults>)Delegate.Combine(gameResults.OnChanged, new Action<GameResults>(HandleGameResults));
            AutoGGUtils.ModLog("Successfully initialized listener!");
        }

        public static void RemoveAutoGGListeners()
        {
            StateProperty<GameResults> gameResults = Service.Game.Sim.simulation.gameResults;
            gameResults.OnChanged = (Action<GameResults>)Delegate.Remove(gameResults.OnChanged, new Action<GameResults>(HandleGameResults));
            AutoGGUtils.ModLog("Successfully removed listener!");
        }

        private static void HandleGameResults(GameResults results)
        {
            if (results.entries.Count < 1) return; // If gameresults sent falsely

            if (ModSettings.GetBool("Send Gameover Message", "voidbehemoth.autogg"))
            {
                Service.Game.Sim.simulation.SendChat(AutoGGUtils.GetFancyGameOverMessage(results));
            }
        }
    }

    [HarmonyPatch(typeof(PickNamesPanel))]
    public class GameStartMessage
    {
        [HarmonyPatch(nameof(PickNamesPanel.HandleSubmitName))]
        [HarmonyPostfix]
        public static void PostSubmitNamefix(PickNamesPanel __instance, string name)
        {
            if (ModSettings.GetBool("Send Game Start Message", "voidbehemoth.autogg") && !Pepper.IsMyInGameNameSet())
            {
                Service.Game.Sim.simulation.SendChat(AutoGGUtils.GetGameStartMessage());
            }
        }

        [HarmonyPatch(nameof(PickNamesPanel.Start))]
        [HarmonyPostfix]
        public static void PostStartFix(PickNamesPanel __instance)
        {
            if (ModStates.IsLoaded("curtis.tuba.better.tos2") && Pepper.GetCurrentGameSim().roleDeckBuilder.Data.modifierCards.Contains(RolePlus.ANON_PLAYERS)) return;

            __instance.StartCoroutine(DelayedSubmitName(__instance));
        }

        private static IEnumerator DelayedSubmitName(PickNamesPanel __instance)
        {
            int setting = ModSettings.GetInt("Auto Choose Name Delay", "voidbehemoth.autogg");

            if (setting < 1) yield break;

            yield return new WaitForSeconds(setting);
            __instance.HandleSubmitName(__instance.nameInput.text);
        }
    }

    public static class MyPluginInfo
    {
        public const string PLUGIN_GUID = "AutoGG";

        public const string PLUGIN_NAME = "AutoGG";

        public const string PLUGIN_VERSION = "1.0.0";
    }

    public static class AutoGGUtils
    {
        // Default values to be sent if config is left blank
        public static readonly string DEFAULT_GAME_END = "gg";
        public static readonly string DEFAULT_GAME_START = "gl";

        public static string GetFancyGameOverMessage(GameResults results)
        {
            HomeLocalizationService localizationService = Service.Home.LocalizationService;

            FactionType faction = results.winningFaction;

            // whether to pluralize the faction name or not
            bool plural = results.entries.FindAll((GameResults.PlayerEntry player) => { return player.factionType == faction; }).Count() > 1;
            string pluralString = (plural) ? "PLURAL_" : "";

            int num = (int)results.winningFaction;
            string localizedString = localizationService.GetLocalizedString("AUTOGG_FACTION_" + pluralString + num);

            string msg = GetGameOverMessage(results).Replace("%faction%", results.winType == WinType.DRAW ? "" : localizedString).Replace("%role%", "[[#" + ((byte)Pepper.GetMyRole()) + "]]");
            Debug.Log($"[AutoGG] {msg}");

            return msg;
        }
        public static string GetGameOverMessage(GameResults results)
        {
            // Get messages from the config
            string[] wonMessages = ModSettings.GetString("Won Game Message", "voidbehemoth.autogg").Split('|');
            string[] lostMessages = ModSettings.GetString("Lost Game Message", "voidbehemoth.autogg").Split('|');
            string[] drawnMessages = ModSettings.GetString("Drawn Game Message", "voidbehemoth.autogg").Split('|');
            string[] endMessages = (ModSettings.GetString("Gameover Message", "voidbehemoth.autogg") ?? DEFAULT_GAME_END).Split('|');

            string wonMessage = wonMessages[UnityEngine.Random.Range(0, wonMessages.Length)];
            string lostMessage = lostMessages[UnityEngine.Random.Range(0, lostMessages.Length)];
            string drawnMessage = drawnMessages[UnityEngine.Random.Range(0, drawnMessages.Length)];
            string endMessage = endMessages[UnityEngine.Random.Range(0, endMessages.Length)];

            // Determine if the player won
            bool won = results.entries[Pepper.GetMyPosition()].won;

            if (wonMessages.Length > 0 && won)
            {
                return wonMessage;
            }
            else if (lostMessages.Length > 0 && !won)
            {
                return lostMessage;
            }
            else if (drawnMessages.Length > 0 && results.winType == WinType.DRAW)
            {
                return drawnMessage;
            }
            else
            {
                return endMessage;
            }
        }

        public static string GetGameStartMessage()
        {
            string[] startMessages = (ModSettings.GetString("Game Start Message", "voidbehemoth.autogg") ?? DEFAULT_GAME_START).Split('|');
            
            string startMessage = startMessages[UnityEngine.Random.Range(0, startMessages.Length)];

            return startMessage.Replace("%name%", Service.Home.UserService.GetGameNameFromStorage());
        }

        public static void ModLog(string message)
        {
            Console.WriteLine("[" + MyPluginInfo.PLUGIN_GUID + "] " + message);
        }
    }

}