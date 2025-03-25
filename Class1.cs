using BepInEx;
using Menu;
using RainMeadow.Game;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Permissions;
using UnityEngine;

[assembly: AssemblyVersion(DllMeadow.DllMeadow.MeadowVersionStr)]
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
namespace DllMeadow
{
    [BepInPlugin("liz.dllmeadow", "DllMeadow", MeadowVersionStr)]
    [BepInDependency("henpemaz.rainmeadow", BepInDependency.DependencyFlags.SoftDependency)]
    public partial class DllMeadow : BaseUnityPlugin
    {
        public const string MeadowVersionStr = "0.1.0";
        public static DllMeadow instance;
        private PlopMachine PlopMachine;

        internal static MenuScene.SceneID Slugcat_MeadowLongLegs = new("Slugcat_MeadowLongLegs", true);
        internal static SoundID RM_LongLegs_Call = new("RM_LongLegs_Call", true);
        public static RainMeadow.MeadowProgression.Character DaddyLongLegs = new("DaddyLongLegs", true, new()
        {
            displayName = "DADDY LONG LEGS",
            emotePrefix = "longlegs_",
            emoteAtlas = "emotes_longlegs",
            emoteColor = Extensions.ColorFromHex(0x2f2ac9),
            //todo: custom long legs call (how do they even???)
            voiceId = RM_LongLegs_Call,
            selectSpriteIndexes = new[] { 2 },
            startingCoords = new WorldCoordinate("SS_B01", 32, 32, -1),
        });
        public static RainMeadow.MeadowProgression.Skin DaddyLongLegs_Purple = new("DaddyLongLegs_Purple", true, new()
        {
            character = Character.DaddyLongLegs,
            displayName = "Purple",
            creatureType = CreatureTemplate.Type.DaddyLongLegs,
            randomSeed = 9814,
            previewColor = Extensions.ColorFromHex(0x460fdb),
        });
        public static RainMeadow.MeadowProgression.Skin DaddyLongLegs_Brown = new("DaddyLongLegs_Brown", true, new()
        {
            character = Character.DaddyLongLegs,
            displayName = "Brown",
            creatureType = CreatureTemplate.Type.DaddyLongLegs,
            randomSeed = 9562,
            previewColor = Extensions.ColorFromHex(0x7d5f37),
        });
        public static RainMeadow.MeadowProgression.Skin DaddyLongLegs_Cyan = new("DaddyLongLegs_Cyan", true, new()
        {
            character = Character.DaddyLongLegs,
            displayName = "Cyan",
            creatureType = CreatureTemplate.Type.DaddyLongLegs,
            randomSeed = 9236,
            previewColor = Extensions.ColorFromHex(0x2796c2),
        });

        public void OnEnable()
        {
            instance = this;

            LongLegsController.EnableLongLegs();
            
            On.RWCustom.Custom.Log += Custom_Log;
            On.RWCustom.Custom.LogImportant += Custom_LogImportant;
            On.RWCustom.Custom.LogWarning += Custom_LogWarning;

            DeathContextualizer.CreateBindings();
        }

        private void Custom_LogWarning(On.RWCustom.Custom.orig_LogWarning orig, string[] values)
        {
            Logger.LogWarning(string.Join(" ", values));
            orig(values);
        }

        private void Custom_LogImportant(On.RWCustom.Custom.orig_LogImportant orig, string[] values)
        {
            Logger.LogInfo(string.Join(" ", values));
            orig(values);
        }

        private void Custom_Log(On.RWCustom.Custom.orig_Log orig, string[] values)
        {
            Logger.LogInfo(string.Join(" ", values));
            orig(values);
        }
    }
}
