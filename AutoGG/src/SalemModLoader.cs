using AutoGG;
using HarmonyLib;
using Home.Services;
using Server.Shared.Info;
using Server.Shared.State;
using Services;
using SML;


namespace SalemModLoader
{

    [Mod.SalemMod]
    public class SalemModLoader
    {
        public static void Start()
        {
            GameInfoObservation gameInfo = Service.Game.Sim.info.gameInfo;
            gameInfo.OnDataChanged = (Action<GameInfo>)Delegate.Combine(gameInfo.OnDataChanged, new Action<GameInfo>(GameEndMessage.HandleGameInfoChanged));
            StateProperty<GameResults> gameResults = Service.Game.Sim.simulation.gameResults;
            gameResults.OnChanged = (Action<GameResults>)Delegate.Combine(gameResults.OnChanged, new Action<GameResults>(GameEndMessage.HandleGameResults));


            Console.WriteLine("ain't no way");
        }
    }

    [HarmonyPatch(typeof(HomeLocalizationService), "RebuildStringTables")]
    public class LocalizationKeys
    {
        // Token: 0x06000013 RID: 19 RVA: 0x000027A0 File Offset: 0x000009A0
        [HarmonyPostfix]
        public static void Postfix(HomeLocalizationService __instance)
        {
            __instance.stringTable_.Add("DEFAULT_GAME_END", "gg");
            __instance.stringTable_.Add("DEFAULT_GAME_START", "gl");
        }
    }
}