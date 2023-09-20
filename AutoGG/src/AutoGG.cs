using Game.Interface;
using HarmonyLib;
using Server.Shared.State;
using Services;
using SML;
using System;
using System.Linq;

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

            if (ModSettings.GetBool("Send Game Over Message", "voidbehemoth.autogg"))
            {
                Service.Game.Sim.simulation.SendChat(AutoGGUtils.GetFancyGameOverMessage(results));
            }
        }
    }

    [HarmonyPatch(typeof(PickNamesPanel), "HandleSubmitName")]
    public class GameStartMessage
    {
        [HarmonyPostfix]
        public static void PostSubmitNamefix(PickNamesPanel __instance, string name)
        {
            if (ModSettings.GetBool("Send Game Start Message", "voidbehemoth.autogg"))
            {
                Service.Game.Sim.simulation.SendChat(AutoGGUtils.GetGameStartMessage());
            }
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
            string faction = results.winningFaction.ToString();

            return GetGameOverMessage(results).Replace("%faction%", results.winType == WinType.DRAW ? "" : "[[#" + faction + "]]");
        }
        public static string GetGameOverMessage(GameResults results)
        {

            int myPosition = Pepper.GetMyPosition();

            // Get messages from the config
            string wonMessage = ModSettings.GetString("Won Game Message", "voidbehemoth.autogg");
            string lostMessage = ModSettings.GetString("Lost Game Message", "voidbehemoth.autogg");
            string drawnMessage = ModSettings.GetString("Drawn Game Message", "voidbehemoth.autogg");
            string endMessage = ModSettings.GetString("Game Over Message", "voidbehemoth.autogg");

            // Determine if the player won
            bool won = (myPosition >= results.entries.Count || myPosition < 0) ? false : (results.entries[myPosition].won) ? true : false;

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

            return (string.IsNullOrEmpty(startMessage)) ? DEFAULT_GAME_START : startMessage;
        }

        public static void ModLog(string message)
        {
            Console.WriteLine("[" + MyPluginInfo.PLUGIN_GUID + "] " + message);
        }
    }

}