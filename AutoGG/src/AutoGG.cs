using Game.Interface;
using HarmonyLib;
using Home.Services;
using Server.Shared.Extensions;
using Server.Shared.State;
using Services;
using SML;
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Threading;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

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
            if (ModStates.IsLoaded("curtis.tuba.better.tos2") && Pepper.GetCurrentGameSim().roleDeckBuilder.Data.modifierCards.Contains((Role)214)) return;

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

            return GetGameOverMessage(results).Replace("%faction%", results.winType == WinType.DRAW ? "" : localizedString).Replace("%role%", "[[#" + ((byte)Pepper.GetMyRole()) + "]]");
        }
        public static string GetGameOverMessage(GameResults results)
        {
            // Get messages from the config
            string wonMessage = ModSettings.GetString("Won Game Message", "voidbehemoth.autogg");
            string lostMessage = ModSettings.GetString("Lost Game Message", "voidbehemoth.autogg");
            string drawnMessage = ModSettings.GetString("Drawn Game Message", "voidbehemoth.autogg");
            string endMessage = ModSettings.GetString("Gameover Message", "voidbehemoth.autogg");

            // Determine if the player won
            bool won = results.entries[Pepper.GetMyPosition()].won;

            if (!string.IsNullOrEmpty(wonMessage) && won)
            {
                return wonMessage;
            }
            else if (!string.IsNullOrEmpty(lostMessage) && !won)
            {
                return lostMessage;
            }
            else if (!string.IsNullOrEmpty(drawnMessage) && results.winType == WinType.DRAW)
            {
                return drawnMessage;
            }
            else if (!string.IsNullOrEmpty(endMessage))
            {
                return endMessage;
            }
            else return DEFAULT_GAME_END;
        }

        public static string GetGameStartMessage()
        {
            string startMessage = ModSettings.GetString("Game Start Message", "voidbehemoth.autogg");
            startMessage = (string.IsNullOrEmpty(startMessage)) ? DEFAULT_GAME_START : startMessage;


            return startMessage.Replace("%name%", Service.Home.UserService.GetGameNameFromStorage());
        }

        public static void ModLog(string message)
        {
            Console.WriteLine("[" + MyPluginInfo.PLUGIN_GUID + "] " + message);
        }
    }

}