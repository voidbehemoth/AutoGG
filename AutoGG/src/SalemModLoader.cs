using AutoGG;
using HarmonyLib;
using Home.Services;
using Mono.Cecil.Cil;
using SML;
using System.Security.Policy;


namespace SalemModLoader
{

    [Mod.SalemMod]
    public class SalemModLoader
    {
        public static void Start()
        {
            AutoGGUtils.ModLog("ain't no way");
        }
    }

    [HarmonyPatch(typeof(HomeLocalizationService), "RebuildStringTables")]
    public class StringTable
    {
        [HarmonyPostfix]
        public static void Postfix(HomeLocalizationService __instance)
        {
            __instance.stringTable_.Add("FACTION_0", "");
            __instance.stringTable_.Add("FACTION_PLURAL_0", "");
            __instance.stringTable_.Add("FACTION_1", "Town");
            __instance.stringTable_.Add("FACTION_PLURAL_1", "Town");
            __instance.stringTable_.Add("FACTION_2", "Coven");
            __instance.stringTable_.Add("FACTION_PLURAL_2", "Coven");
            __instance.stringTable_.Add("FACTION_3", "Serial Killer");
            __instance.stringTable_.Add("FACTION_PLURAL_3", "Serial Killers");
            __instance.stringTable_.Add("FACTION_4", "Arsonist");
            __instance.stringTable_.Add("FACTION_PLURAL_4", "Arsonists");
            __instance.stringTable_.Add("FACTION_5", "Werewolf");
            __instance.stringTable_.Add("FACTION_PLURAL_5", "Werewolves");
            __instance.stringTable_.Add("FACTION_6", "Shroud");
            __instance.stringTable_.Add("FACTION_PLURAL_6", "Shrouds");
            __instance.stringTable_.Add("FACTION_7", "Apocalypse");
            __instance.stringTable_.Add("FACTION_PLURAL_7", "Apocalypse");
            __instance.stringTable_.Add("FACTION_8", "Executioner");
            __instance.stringTable_.Add("FACTION_PLURAL_8", "Executioners");
            __instance.stringTable_.Add("FACTION_9", "Jester");
            __instance.stringTable_.Add("FACTION_PLURAL_9", "Jesters");
            __instance.stringTable_.Add("FACTION_10", "Pirate");
            __instance.stringTable_.Add("FACTION_PLURAL_10", "Pirates");
            __instance.stringTable_.Add("FACTION_11", "Doomsayer");
            __instance.stringTable_.Add("FACTION_PLURAL_11", "Doomsayers");
            __instance.stringTable_.Add("FACTION_12", "Vampire");
            __instance.stringTable_.Add("FACTION_PLURAL_12", "Vampires");
            __instance.stringTable_.Add("FACTION_13", "Cursed Soul");
            __instance.stringTable_.Add("FACTION_PLURAL_13", "Cursed Souls");
            __instance.stringTable_.Add("FACTION_14", "");
            __instance.stringTable_.Add("FACTION_PLURAL_14", "");
        }
    }
}