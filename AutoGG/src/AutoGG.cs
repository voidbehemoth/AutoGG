using Game.Interface;
using Game.Services;
using Game.Simulation;
using HarmonyLib;
using Server.Shared.Info;
using Server.Shared.State;
using Services;
using SML;
using static Utils.Storage;

namespace AutoGG
{
    public static class GameEndMessage
    {

        private static GameResults results;
        public static void HandleGameResults(GameResults gameResults)
        {
            if (results.entries.Count >= 1)
            {
                results = gameResults;
            }
        }

        public static void HandleGameInfoChanged(GameInfo gameInfo)
        {
            if (results == null) return;

            if (gameInfo.gamePhase == GamePhase.RESULTS && ModSettings.GetBool("Send Game Over Message"))
            {
                Service.Game.Sim.simulation.SendChat(AutoGGUtils.GetGameOverMessage(results));
                results = null;
            }
        }


    }

    [HarmonyPatch(typeof(PickNamesPanel), "HandleSubmitName")]
    public class AutoGG
    {
        [HarmonyPostfix]
        public static void PostPhasefix(PickNamesPanel __instance, string name)
        {
            if (ModSettings.GetBool("Send Game Start Message"))
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
        

        public static string GetGameOverMessage(GameResults results)
        {
            GameSimulation sim = Services.Service.Game.Sim.simulation;

            string wonMessage = ModSettings.GetString("Won Game Message");
            string lostMessage = ModSettings.GetString("Lost Game Message");
            string endMessage = ModSettings.GetString("Game Over Message");

            bool won = results.entries.Find(entry => entry.playerId.Equals(Services.Service.Home.UserService.UserInfo.AccountID)).won;
            if (!string.IsNullOrEmpty(wonMessage) && won)
            {
                return wonMessage;
            } else if (!string.IsNullOrEmpty(lostMessage) && !won)
            {
                return lostMessage;
            } else if (!string.IsNullOrEmpty(endMessage)) {
                return endMessage;
            } else return l10n("DEFAULT_GAME_END");
        }

        public static string GetGameStartMessage()
        {
            string startMessage = ModSettings.GetString("Game Start Message");

            return (string.IsNullOrEmpty(startMessage)) ? l10n("DEFAULT_START_GAME") : startMessage;
        }

        public static string l10n(string key)
        {
            return Service.Home.LocalizationService.GetLocalizedString(key);
        }
    }

}