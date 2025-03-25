﻿using BepInEx;
using Menu;
using RainMeadow.Game;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Security.Permissions;
using UnityEngine;

using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

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
        private bool init;
        private bool fullyInit;
        private bool addedMod = false;
        internal static MenuScene.SceneID Slugcat_MeadowLongLegs = new("Slugcat_MeadowLongLegs", true);
        internal static SoundID RM_LongLegs_Call = new("RM_LongLegs_Call", true);
        public static RainMeadow.MeadowProgression.Character DaddyLongLegs = new("DaddyLongLegs", true, new()
        {
            displayName = "DADDY LONG LEGS",
            emotePrefix = "longlegs_",
            emoteAtlas = "emotes_longlegs",
            emoteColor = RainMeadow.Extensions.ColorFromHex(0x2f2ac9),
            //todo: custom long legs call (how do they even???)
            voiceId = RM_LongLegs_Call,
            selectSpriteIndexes = new[] { 2 },
            startingCoords = new WorldCoordinate("GW_C02", 3, 3, -1),
        });
        public static RainMeadow.MeadowProgression.Skin DaddyLongLegs_Purple = new("DaddyLongLegs_Purple", true, new()
        {
            character = DaddyLongLegs,
            displayName = "Purple",
            creatureType = CreatureTemplate.Type.DaddyLongLegs,
            randomSeed = 9814,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x460fdb),
        });
        public static RainMeadow.MeadowProgression.Skin DaddyLongLegs_Brown = new("DaddyLongLegs_Brown", true, new()
        {
            character = DaddyLongLegs,
            displayName = "Brown",
            creatureType = CreatureTemplate.Type.DaddyLongLegs,
            randomSeed = 9562,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x7d5f37),
        });
        public static RainMeadow.MeadowProgression.Skin DaddyLongLegs_Cyan = new("DaddyLongLegs_Cyan", true, new()
        {
            character = DaddyLongLegs,
            displayName = "Cyan",
            creatureType = CreatureTemplate.Type.DaddyLongLegs,
            randomSeed = 9236,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x2796c2),
        });

        public void OnEnable()
        {
            instance = this;
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.Menu.SlugcatSelectMenu.SlugcatPage.AddImage += SlugcatPage_AddImage;
            On.Menu.MenuScene.BuildScene += MenuScene_BuildScene;
            DeathContextualizer.CreateBindings();
        }

        public class DllHooks
        {
            private static void BindAvatar(Creature creature, RainMeadow.OnlineCreature oc, RainMeadow.MeadowAvatarData customization)
            {
                if (creature is DaddyLongLegs player)
                {
                    new LongLegsController(player, oc, 0, customization);
                }
            }
        }

        private void MenuScene_BuildScene(On.Menu.MenuScene.orig_BuildScene orig, MenuScene self)
        {
            orig(self);
            if (!string.IsNullOrEmpty(self.sceneFolder))
            {
                return;
            }
            if (self.sceneID == Slugcat_MeadowLongLegs)
            {
                self.sceneFolder = "Scenes" + Path.DirectorySeparatorChar.ToString() + "meadow - longlegs";
                if (self.flatMode)
                {
                    self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "MeadowMouse - Flat", new Vector2(683f, 384f), false, true));
                }
                else
                {
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "rmlonglegs bg", new Vector2(0f, 0f), 3.5f, MenuDepthIllustration.MenuShader.Normal));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "rmlonglegs lights", new Vector2(0f, 0f), 2.4f, MenuDepthIllustration.MenuShader.SoftLight));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "rmlonglegs noot", new Vector2(0f, 0f), 2.2f, MenuDepthIllustration.MenuShader.Normal));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "rmlonglegs fg", new Vector2(0f, 0f), 2.1f, MenuDepthIllustration.MenuShader.LightEdges));
                    (self as InteractiveMenuScene).idleDepths.Add(3.2f);
                    (self as InteractiveMenuScene).idleDepths.Add(2.2f);
                    (self as InteractiveMenuScene).idleDepths.Add(2.1f);
                    (self as InteractiveMenuScene).idleDepths.Add(2.0f);
                    (self as InteractiveMenuScene).idleDepths.Add(1.5f);
                }
            }
            if (string.IsNullOrEmpty(self.sceneFolder))
            {
                return;
            }
            string path2 = AssetManager.ResolveFilePath(self.sceneFolder + Path.DirectorySeparatorChar.ToString() + "positions_ims.txt");
            if (!File.Exists(path2) || !(self is InteractiveMenuScene))
            {
                path2 = AssetManager.ResolveFilePath(self.sceneFolder + Path.DirectorySeparatorChar.ToString() + "positions.txt");
            }
            if (File.Exists(path2))
            {
                string[] array3 = File.ReadAllLines(path2);

                for (int num3 = 0; num3 < array3.Length && num3 < self.depthIllustrations.Count; num3++)
                {
                    self.depthIllustrations[num3].pos.x = float.Parse(Regex.Split(RWCustom.Custom.ValidateSpacedDelimiter(array3[num3], ","), ", ")[0], NumberStyles.Any, CultureInfo.InvariantCulture);
                    self.depthIllustrations[num3].pos.y = float.Parse(Regex.Split(RWCustom.Custom.ValidateSpacedDelimiter(array3[num3], ","), ", ")[1], NumberStyles.Any, CultureInfo.InvariantCulture);
                    self.depthIllustrations[num3].lastPos = self.depthIllustrations[num3].pos;
                }
            }
        }

        private void SlugcatPage_AddImage(On.Menu.SlugcatSelectMenu.SlugcatPage.orig_AddImage orig, SlugcatSelectMenu.SlugcatPage self, bool ascended)
        {
            if (self.slugcatNumber == RainMeadow.RainMeadow.Ext_SlugcatStatsName.OnlineSessionPlayer && self is RainMeadow.MeadowCharacterSelectPage mcsp && mcsp.character == DaddyLongLegs)
            {
                var sceneID = Slugcat_MeadowLongLegs;
                self.sceneOffset = new Vector2(-10f, 100f);
                self.slugcatDepth = 3.1000001f;
                //
				self.slugcatImage = new InteractiveMenuScene(self.menu, self, sceneID);
				self.subObjects.Add(self.slugcatImage);
				if (self.HasMark)
				{
					self.markSquare = new FSprite("pixel", true);
					self.markSquare.scale = 14f;
					self.markSquare.color = Color.Lerp(self.effectColor, Color.white, 0.7f);
					self.Container.AddChild(self.markSquare);
					self.markGlow = new FSprite("Futile_White", true);
					self.markGlow.shader = self.menu.manager.rainWorld.Shaders["FlatLight"];
					self.markGlow.color = self.effectColor;
					self.Container.AddChild(self.markGlow);
				}
            }
            else
            {
                orig(self, ascended);
            }
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            if (init) return;
            init = true;
            try
            {
                LongLegsController.EnableLongLegs();
                var d = new Hook(
                    typeof(RainMeadow.CreatureController).GetMethod("BindAvatar", BindingFlags.NonPublic | BindingFlags.Static),
                    typeof(DllHooks).GetMethod("BindAvatar", BindingFlags.NonPublic | BindingFlags.Static)
                );
                fullyInit = true;
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                fullyInit = false;
            }
        }
    }
}
