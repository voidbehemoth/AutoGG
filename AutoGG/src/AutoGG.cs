

using System;
using System.IO;
using SML;
using Game.Interface;
using HarmonyLib;
using Server.Shared.State;


namespace AutoGG
{


    /*
    [HarmonyPatch(typeof(HudEndGame), "HandleGameResults")]
    public class AutoGG
    {
        [HarmonyPostfix]
        public static void PostPhaseFix(HudEndGame __instance, GameResults results)
        {
            if (results.entries.Count >= 1)
            {
                Services.Service.Game.Sim.simulation.SendChat("gg");
            }
        }
    }*/

    public static class MyPluginInfo
    {
        public const string PLUGIN_GUID = "AutoGG";

        public const string PLUGIN_NAME = "AutoGG";

        public const string PLUGIN_VERSION = "1.0.0";
    }

}