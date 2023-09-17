using Game.Interface;
using Game.Services;
using Game.Simulation;
using HarmonyLib;
using Server.Shared.Info;
using Server.Shared.Messages;
using Server.Shared.State;
using Services;
using SML;
using System;
using System.Collections;

namespace AutoGG
{

    [HarmonyPatch(typeof(EndgameWrapupOverlayController), "InitializeListeners")]
    public class IntitializeListeners
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            AutoGG.InitializeAutoGGListeners();
        }
    }

    public static class AutoGG
    {

        public static void InitializeAutoGGListeners()
        {
            StateProperty<GameResults> gameResults = Service.Game.Sim.simulation.gameResults;
            gameResults.OnChanged = (Action<GameResults>)Delegate.Combine(gameResults.OnChanged, new Action<GameResults>(HandleGameResults));
        }

        private static void HandleGameResults(GameResults results)
        {
            if (results.entries.Count < 1) return;

            if (ModSettings.GetBool("Send Game Over Message", "behemoth.autogg"))
            {
                AutoGGUtils.ModLog("Sending gameover message...");

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
            if (ModSettings.GetBool("Send Game Start Message", "behemoth.autogg"))
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

            string wonMessage = ModSettings.GetString("Won Game Message", "behemoth.autogg");
            string lostMessage = ModSettings.GetString("Lost Game Message", "behemoth.autogg");
            string drawnMessage = ModSettings.GetString("Drawn Game Message", "behemoth.autogg");
            string endMessage = ModSettings.GetString("Game Over Message", "behemoth.autogg");

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
            string startMessage = ModSettings.GetString("Game Start Message", "behemoth.autogg");

            return (string.IsNullOrEmpty(startMessage)) ? DEFAULT_GAME_START : startMessage;
        }

        public static void ModLog(string message)
        {
            Console.WriteLine("[" + MyPluginInfo.PLUGIN_GUID + "] " + message);
        }
    }

}