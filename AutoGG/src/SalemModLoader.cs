using AutoGG;
using HarmonyLib;
using Home.Services;
using Mono.Cecil.Cil;
using SML;
using System;
using System.Security.Policy;


namespace SalemModLoader
{

    [Mod.SalemMod]
    public class SalemModLoader
    {
        public static void Start()
        {
            AutoGGUtils.ModLog("ain't no way");

            DefineUndefinedSettings();
        }

        private static void DefineUndefinedSettings()
        {
            try
            {
                ModSettings.GetBool("Send Gameover Message", "voidbehemoth.autogg");
            }
            catch
            {
                ModSettings.SetBool("Send Gameover Message", true, "voidbehemoth.autogg");
            }

            try
            {
                ModSettings.GetBool("Send Game Start Message", "voidbehemoth.autogg");
            }
            catch
            {
                ModSettings.SetBool("Send Game Start Message", false, "voidbehemoth.autogg");
            }
        }
    }

    [HarmonyPatch(typeof(HomeLocalizationService), "RebuildStringTables")]
    public class StringTable
    {
        [HarmonyPostfix]
        public static void Postfix(HomeLocalizationService __instance)
        {
            __instance.stringTable_.Add("AUTOGG_FACTION_0", "");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_0", "");
            __instance.stringTable_.Add("AUTOGG_FACTION_1", "Town");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_1", "Town");
            __instance.stringTable_.Add("AUTOGG_FACTION_2", "Coven");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_2", "Coven");
            __instance.stringTable_.Add("AUTOGG_FACTION_3", "Serial Killer");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_3", "Serial Killers");
            __instance.stringTable_.Add("AUTOGG_FACTION_4", "Arsonist");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_4", "Arsonists");
            __instance.stringTable_.Add("AUTOGG_FACTION_5", "Werewolf");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_5", "Werewolves");
            __instance.stringTable_.Add("AUTOGG_FACTION_6", "Shroud");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_6", "Shrouds");
            __instance.stringTable_.Add("AUTOGG_FACTION_7", "Apocalypse");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_7", "Apocalypse");
            __instance.stringTable_.Add("AUTOGG_FACTION_8", "Executioner");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_8", "Executioners");
            __instance.stringTable_.Add("AUTOGG_FACTION_9", "Jester");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_9", "Jesters");
            __instance.stringTable_.Add("AUTOGG_FACTION_10", "Pirate");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_10", "Pirates");
            __instance.stringTable_.Add("AUTOGG_FACTION_11", "Doomsayer");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_11", "Doomsayers");
            __instance.stringTable_.Add("AUTOGG_FACTION_12", "Vampire");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_12", "Vampires");
            __instance.stringTable_.Add("AUTOGG_FACTION_13", "Cursed Soul");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_13", "Cursed Souls");
            __instance.stringTable_.Add("AUTOGG_FACTION_14", "");
            __instance.stringTable_.Add("AUTOGG_FACTION_PLURAL_14", "");
        }
    }

    [DynamicSettings]
    public class Settings
    {
        public ModSettings.TextInputSetting GameOverMessage
        {
            get
            {
                ModSettings.TextInputSetting GameOverMessage = new()
                {
                    Name = "Gameover Message",
                    Description = "This is the message that will automatically be sent when a game ends. '%faction%' will be replaced with the winning faction. '%faction%' will be replaced with the winning faction. '%role%' will be replaced with your role.",
                    DefaultValue = "gg",
                    Regex = @"^(?!.*[<>]).*",
                    CharacterLimit = 140,
                    AvailableInGame = ModSettings.GetBool("Send Gameover Message", "voidbehemoth.autogg"),
                    Available = ModSettings.GetBool("Send Gameover Message", "voidbehemoth.autogg"),
                    OnChanged = (s) => { }
                };
                return GameOverMessage;
            }
        }
        public ModSettings.TextInputSetting GameWonMessage
        {
            get
            {
                ModSettings.TextInputSetting GameWonMessage = new()
                {
                    Name = "Won Game Message",
                    Description = "This is the message that will automatically be sent when a game ends if you win. Overrides the 'Gameover Message' setting if set. '%faction%' will be replaced with the winning faction. '%role%' will be replaced with your role.",
                    DefaultValue = "",
                    Regex = @"^(?!.*[<>]).*",
                    CharacterLimit = 140,
                    AvailableInGame = ModSettings.GetBool("Send Gameover Message", "voidbehemoth.autogg"),
                    Available = ModSettings.GetBool("Send Gameover Message", "voidbehemoth.autogg"),
                    OnChanged = (s) => { }
                };
                return GameWonMessage;
            }
        }
        public ModSettings.TextInputSetting GameLostMessage
        {
            get
            {
                ModSettings.TextInputSetting GameLostMessage = new()
                {
                    Name = "Lost Game Message",
                    Description = "This is the message that will automatically be sent when a game ends if you lose. Overrides the 'Gameover Message' setting if set. '%faction%' will be replaced with the winning faction. '%role%' will be replaced with your role.",
                    DefaultValue = "",
                    Regex = @"^(?!.*[<>]).*",
                    CharacterLimit = 140,
                    AvailableInGame = ModSettings.GetBool("Send Gameover Message", "voidbehemoth.autogg"),
                    Available = ModSettings.GetBool("Send Gameover Message", "voidbehemoth.autogg"),
                    OnChanged = (s) => { }
                };
                return GameLostMessage;
            }
        }
        public ModSettings.TextInputSetting GameDrawnMessage
        {
            get
            {
                ModSettings.TextInputSetting GameDrawnMessage = new()
                {
                    Name = "Drawn Game Message",
                    Description = "This is the message that will automatically be sent when a game ends if no one wins. Overrides the 'Gameover Message' setting if set. '%role%' will be replaced with your role.",
                    DefaultValue = "",
                    Regex = @"^(?!.*[<>]).*",
                    CharacterLimit = 140,
                    AvailableInGame = ModSettings.GetBool("Send Gameover Message", "voidbehemoth.autogg"),
                    Available = ModSettings.GetBool("Send Gameover Message", "voidbehemoth.autogg"),
                    OnChanged = (s) => { }
                };
                return GameDrawnMessage;
            }
        }
        public ModSettings.TextInputSetting GameStartMessage
        {
            get
            {
                ModSettings.TextInputSetting GameStartMessage = new()
                {
                    Name = "Game Start Message",
                    Description = "This is the message that will automatically be sent once the first day begins.",
                    DefaultValue = "gl",
                    Regex = @"^(?!.*[<>]).*",
                    CharacterLimit = 140,
                    AvailableInGame = ModSettings.GetBool("Send Game Start Message", "voidbehemoth.autogg"),
                    Available = ModSettings.GetBool("Send Game Start Message", "voidbehemoth.autogg"),
                    OnChanged = (s) => { }
                };
                return GameStartMessage;
            }
        }
    }
}