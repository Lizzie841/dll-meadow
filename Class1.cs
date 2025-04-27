using BepInEx;
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

[assembly: AssemblyVersion(DllMeadow.DllMeadow.MeadowVersionStr)]
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
namespace DllMeadow
{
    [BepInPlugin("danish.drmorecreaturesformeadow", "DllMeadow", MeadowVersionStr)]
    [BepInDependency("henpemaz.rainmeadow", BepInDependency.DependencyFlags.SoftDependency)]
    public partial class DllMeadow : BaseUnityPlugin
    {
        public const string MeadowVersionStr = "0.1.0";
        public static DllMeadow? instance;
        private bool init;
        private bool fullyInit;
        private bool addedMod = false;

        // =============================================================
        // LONG LEGS
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
            startingCoords = new WorldCoordinate("GW_C02", 16, 16, -1),
        });
        public static RainMeadow.MeadowProgression.Skin DaddyLongLegs_Blue = new("DaddyLongLegs_Blue", true, new()
        {
            character = DaddyLongLegs,
            displayName = "Blue (Daddy)",
            creatureType = CreatureTemplate.Type.DaddyLongLegs,
            randomSeed = 9817,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x460fdb),
        });
        public static RainMeadow.MeadowProgression.Skin DaddyLongLegs_Purple = new("DaddyLongLegs_Purple", true, new()
        {
            character = DaddyLongLegs,
            displayName = "Purple (Mother)",
            creatureType = CreatureTemplate.Type.DaddyLongLegs, //default, changed at runtime
            randomSeed = 9814,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x460fdb),
        });
        public static RainMeadow.MeadowProgression.Skin DaddyLongLegs_Brown = new("DaddyLongLegs_Brown", true, new()
        {
            character = DaddyLongLegs,
            displayName = "Brown (Brother)",
            creatureType = CreatureTemplate.Type.BrotherLongLegs,
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
        // =============================================================
        // CENTIPEDE
        internal static MenuScene.SceneID Slugcat_MeadowCentipede = new("Slugcat_MeadowCentipede", true);
        internal static SoundID RM_Centipede_Call = new("RM_Centipede_Call", true);
        public static RainMeadow.MeadowProgression.Character Centipede = new("Centipede", true, new()
        {
            displayName = "CENTIPEDE",
            emotePrefix = "centipede_",
            emoteAtlas = "emotes_centipede",
            emoteColor = RainMeadow.Extensions.ColorFromHex(0x2f2ac9),
            voiceId = RM_Centipede_Call,
            selectSpriteIndexes = new[] { 2 },
            startingCoords = new WorldCoordinate("SI_D02", 16, 16, -1),
        });
        public static RainMeadow.MeadowProgression.Skin Centipede_Small = new("Centipede_Small", true, new()
        {
            character = Centipede,
            displayName = "Small Centipede",
            creatureType = CreatureTemplate.Type.SmallCentipede,
            randomSeed = 9814,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x460fdb),
        });
        public static RainMeadow.MeadowProgression.Skin Centipede_Normal = new("Centipede_Normal", true, new()
        {
            character = Centipede,
            displayName = "Centipede",
            creatureType = CreatureTemplate.Type.Centipede,
            randomSeed = 9814,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x460fdb),
        });
        public static RainMeadow.MeadowProgression.Skin Centipede_Red = new("Centipede_Red", true, new()
        {
            character = Centipede,
            displayName = "Red Centipede",
            creatureType = CreatureTemplate.Type.RedCentipede,
            randomSeed = 9814,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x460fdb),
        });
        public static RainMeadow.MeadowProgression.Skin Centipede_Centiwing = new("Centipede_Centiwing", true, new()
        {
            character = Centipede,
            displayName = "Centiwing",
            creatureType = CreatureTemplate.Type.Centiwing,
            randomSeed = 9562,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x7d5f37),
        });
        public static RainMeadow.MeadowProgression.Skin Centipede_Aquacenti = new("Centipede_Aquacenti", true, new()
        {
            character = Centipede,
            displayName = "Aquacenti",
            creatureType = CreatureTemplate.Type.Centiwing, //default, changed at runtime
            randomSeed = 9236,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x2796c2),
        });
        // =============================================================
        // DROPBUG
        internal static MenuScene.SceneID Slugcat_MeadowDropBug = new("Slugcat_MeadowDropBug", true);
        internal static SoundID RM_DropBug_Call = new("RM_DropBug_Call", true);
        public static RainMeadow.MeadowProgression.Character DropBug = new("DropBug", true, new()
        {
            displayName = "DROPWIG",
            emotePrefix = "dropbug_",
            emoteAtlas = "emotes_dropbug",
            emoteColor = RainMeadow.Extensions.ColorFromHex(0x2f2ac9),
            voiceId = RM_DropBug_Call,
            selectSpriteIndexes = new[] { 2 },
            startingCoords = new WorldCoordinate("SU_B08", 16, 16, -1),
        });
        public static RainMeadow.MeadowProgression.Skin Dropbug_Normal = new("Dropbug_Normal", true, new()
        {
            character = DropBug,
            displayName = "Dropbug",
            creatureType = CreatureTemplate.Type.DropBug,
            randomSeed = 9211,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x808080),
        });
        // =============================================================
        // POLE PLANT
        internal static MenuScene.SceneID Slugcat_MeadowPoleMimic = new("Slugcat_MeadowPoleMimic", true);
        internal static SoundID RM_PoleMimic_Call = new("RM_PoleMimic_Call", true);
        public static RainMeadow.MeadowProgression.Character PoleMimic = new("PoleMimic", true, new()
        {
            displayName = "POLE MIMIC",
            emotePrefix = "polemimic_",
            emoteAtlas = "emotes_polemimic",
            emoteColor = RainMeadow.Extensions.ColorFromHex(0x2f2ac9),
            voiceId = RM_PoleMimic_Call,
            selectSpriteIndexes = new[] { 2 },
            startingCoords = new WorldCoordinate("SU_B01", -1, -1, 1),
        });
        public static RainMeadow.MeadowProgression.Skin PoleMimic_Normal = new("PoleMimic_Normal", true, new()
        {
            character = PoleMimic,
            displayName = "PoleMimic",
            creatureType = CreatureTemplate.Type.PoleMimic,
            randomSeed = 9211,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x808080),
        });
        // =============================================================
        // SMALL MOTH
        internal static MenuScene.SceneID Slugcat_MeadowBigMoth = new("Slugcat_MeadowBigMoth", true);
        internal static SoundID RM_BigMoth_Call = new("RM_BigMoth_Call", true);
        public static RainMeadow.MeadowProgression.Character BigMoth = new("BigMoth", true, new()
        {
            displayName = "MOTH",
            emotePrefix = "bigmoth_",
            emoteAtlas = "emotes_bigmoth",
            emoteColor = RainMeadow.Extensions.ColorFromHex(0x2f2ac9),
            voiceId = RM_BigMoth_Call,
            selectSpriteIndexes = new[] { 2 },
            startingCoords = new WorldCoordinate("SU_B01", 16, 16, -1),
        });
        public static RainMeadow.MeadowProgression.Skin BigMoth_Small = new("BigMoth_Small", true, new()
        {
            character = BigMoth,
            displayName = "BigMoth",
            creatureType = CreatureTemplate.Type.SmallNeedleWorm, //fallback
            randomSeed = 4211,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x808080),
        });
        // =============================================================
        // BARNACLE
        internal static MenuScene.SceneID Slugcat_MeadowBarnacle = new("Slugcat_MeadowBarnacle", true);
        internal static SoundID RM_Barnacle_Call = new("RM_Barnacle_Call", true);
        public static RainMeadow.MeadowProgression.Character Barnacle = new("Barnacle", true, new()
        {
            displayName = "BARNACLE",
            emotePrefix = "barnacle_",
            emoteAtlas = "emotes_barnacle",
            emoteColor = RainMeadow.Extensions.ColorFromHex(0x2f2ac9),
            voiceId = RM_Barnacle_Call,
            selectSpriteIndexes = new[] { 2 },
            startingCoords = new WorldCoordinate("SL_C09", 16, 16, -1),
        });
        public static RainMeadow.MeadowProgression.Skin Barnacle_Normal = new("Barnacle_Normal", true, new()
        {
            character = Barnacle,
            displayName = "Barnacle",
            creatureType = CreatureTemplate.Type.Snail, //fallback
            randomSeed = 4211,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x808080),
        });
        // =============================================================
        // EXTENSIONS TO LIZARDS
        public static RainMeadow.MeadowProgression.Skin Lizard_Strawberry = new("Lizard_Strawberry", true, new()
        {
            character = RainMeadow.MeadowProgression.Character.Lizard,
            displayName = "Strawberry Lizard",
            creatureType = CreatureTemplate.Type.PinkLizard, //fallback
            randomSeed = 6454,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x808080),
        });
        public static RainMeadow.MeadowProgression.Skin Lizard_Train = new("Lizard_Train", true, new()
        {
            character = RainMeadow.MeadowProgression.Character.Lizard,
            displayName = "Train Lizard",
            creatureType = CreatureTemplate.Type.PinkLizard, //fallback
            randomSeed = 34343,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x808080),
        });

        //
        internal static MenuScene.SceneID Slugcat_MeadowRedLizard = new("Slugcat_MeadowRedLizard", true);
        internal static SoundID RM_RedLizard_Call = new("RM_RedLizard_Call", true);
        public static RainMeadow.MeadowProgression.Character RedLizard = new("Red Lizard", true, new()
        {
            displayName = "RED LIZARD",
            emotePrefix = "lizard_",
            emoteAtlas = "emotes_lizard",
            emoteColor = RainMeadow.Extensions.ColorFromHex(0x2f2ac9),
            voiceId = RM_RedLizard_Call,
            selectSpriteIndexes = new[] { 2 },
            startingCoords = new WorldCoordinate("SL_C09", 16, 16, -1),
        });
        public static RainMeadow.MeadowProgression.Skin Lizard_RedRed = new("Lizard_RedRed", true, new()
        {
            character = RedLizard,
            displayName = "Red Lizard",
            creatureType = CreatureTemplate.Type.RedLizard, //fallback
            randomSeed = 34343,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x808080),
        });
        //
        public static RainMeadow.MeadowProgression.Skin Slugcat_Rivulet = new("Slugcat_Rivulet", true, new()
        {
            character = RainMeadow.MeadowProgression.Character.Slugcat,
            displayName = "Rivulet",
            creatureType = CreatureTemplate.Type.Slugcat,
            randomSeed = 6454,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x808080),
        });
        public static RainMeadow.MeadowProgression.Skin Slugcat_Spearmaster = new("Slugcat_Spearmaster", true, new()
        {
            character = RainMeadow.MeadowProgression.Character.Slugcat,
            displayName = "Spearmaster",
            creatureType = CreatureTemplate.Type.Slugcat,
            randomSeed = 6454,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x808080),
        });
        public static RainMeadow.MeadowProgression.Skin Slugcat_Saint = new("Slugcat_Saint", true, new()
        {
            character = RainMeadow.MeadowProgression.Character.Slugcat,
            displayName = "Saint",
            creatureType = CreatureTemplate.Type.Slugcat,
            randomSeed = 6454,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x808080),
        });
        public static RainMeadow.MeadowProgression.Skin Slugcat_Artificer = new("Slugcat_Artificer", true, new()
        {
            character = RainMeadow.MeadowProgression.Character.Slugcat,
            displayName = "Artificer",
            creatureType = CreatureTemplate.Type.Slugcat,
            randomSeed = 6454,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x808080),
        });
        public static RainMeadow.MeadowProgression.Skin Slugcat_Gourmand = new("Slugcat_Gourmand", true, new()
        {
            character = RainMeadow.MeadowProgression.Character.Slugcat,
            displayName = "Gourmand",
            creatureType = CreatureTemplate.Type.Slugcat,
            randomSeed = 6454,
            previewColor = RainMeadow.Extensions.ColorFromHex(0x808080),
        });

        public void OnEnable()
        {
            instance = this;
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.Menu.SlugcatSelectMenu.SlugcatPage.AddImage += SlugcatPage_AddImage;
            On.Menu.MenuScene.BuildScene += MenuScene_BuildScene;
            On.RainWorldGame.SpawnPlayers_bool_bool_bool_bool_WorldCoordinate += RainWorldGame_SpawnPlayers_bool_bool_bool_bool_WorldCoordinate; // Personas are set as non-transferable
            DeathContextualizer.CreateBindings();
        }

        // Avatars are set as non-transferable
        private AbstractCreature RainWorldGame_SpawnPlayers_bool_bool_bool_bool_WorldCoordinate(On.RainWorldGame.orig_SpawnPlayers_bool_bool_bool_bool_WorldCoordinate orig, RainWorldGame self, bool player1, bool player2, bool player3, bool player4, WorldCoordinate location)
        {
            // kludge for pole plants
            if (RainMeadow.OnlineManager.lobby != null)
            {
                if (RainMeadow.OnlineManager.lobby.gameMode is RainMeadow.MeadowGameMode mgm)
                {
                    var skinData = RainMeadow.MeadowProgression.skinData[mgm.avatarData.skin];
                    if (skinData.creatureType == CreatureTemplate.Type.PoleMimic)
                    {
                        var newLocation = location;
                        newLocation.x = -1;
                        newLocation.y = -1;
                        newLocation.abstractNode = 1;
                        RainMeadow.RainMeadow.sSpawningAvatar = true;
                        AbstractCreature abstractCreature = RainMeadow.OnlineManager.lobby.gameMode.SpawnAvatar(self, location);
                        if (abstractCreature == null) abstractCreature = orig(self, player1, player2, player3, player4, location);
                        // TODO: is this a good quick hack :(
                        self.world.GetAbstractRoom(abstractCreature.pos.room).MoveEntityToDen(abstractCreature);
                        RainMeadow.RainMeadow.sSpawningAvatar = false;
                        return abstractCreature;
                        //self.abstractRoom.MoveEntityToDen
                    }
                }
            }
            return orig(self, player1, player2, player3, player4, location);
        }

        public class DllHooks
        {
            private static SlugcatStats.Name? GetSlugcatBySkin(RainMeadow.MeadowProgression.Skin s)
            {
                if (s == Slugcat_Rivulet) return MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Rivulet;
                else if (s == Slugcat_Artificer) return MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Artificer;
                else if (s == Slugcat_Gourmand) return MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Gourmand;
                else if (s == Slugcat_Saint) return MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Saint;
                else if (s == Slugcat_Spearmaster) return MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear;
                return RainMeadow.RainMeadow.Ext_SlugcatStatsName.OnlineSessionPlayer;
            } 

            private delegate AbstractCreature orig_SpawnAvatar(RainMeadow.MeadowGameMode self, RainWorldGame game, WorldCoordinate location);
            private static AbstractCreature SpawnAvatar(orig_SpawnAvatar orig, RainMeadow.MeadowGameMode self, RainWorldGame game, WorldCoordinate location)
            {
                var skinData = RainMeadow.MeadowProgression.skinData[self.avatarData.skin];
                if (skinData.creatureType == CreatureTemplate.Type.Slugcat && GetSlugcatBySkin(self.avatarData.skin) is SlugcatStats.Name stat)
                {
                    var ac = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(skinData.creatureType), null, location, new EntityID(-1, skinData.randomSeed));
                    ac.state = new PlayerState(ac, 0, stat, false);
                    game.session.AddPlayer(ac);
                    game.world.GetAbstractRoom(ac.pos.room).AddEntity(ac);
                    RainMeadow.RainMeadow.Debug($"EXTRA SPAWNS AVATAR! {ac}");
                    return ac;
                }
                return orig(self, game, location);
            }

            private delegate void orig_BindAvatar(Creature creature, RainMeadow.OnlineCreature oc, RainMeadow.MeadowAvatarData customization);
            private static void BindAvatar(orig_BindAvatar orig, Creature creature, RainMeadow.OnlineCreature oc, RainMeadow.MeadowAvatarData customization)
            {
                if (creature is DaddyLongLegs p1)
                {
                    new LongLegsController(p1, oc, 0, customization);
                }
                else if (creature is Centipede p2)
                {
                    new CentipedeController(p2, oc, 0, customization);
                }
                else if (creature is DropBug p3)
                {
                    new DropBugController(p3, oc, 0, customization);
                }
                else if (creature is PoleMimic p4)
                {
                    new PoleMimicController(p4, oc, 0, customization);
                }
                else if (creature is Watcher.BigMoth p5)
                {
                    new BigMothController(p5, oc, 0, customization);
                }
                else if (creature is Watcher.Barnacle p6)
                {
                    new BarnacleController(p6, oc, 0, customization);
                }
                else
                {
                    orig(creature, oc, customization);
                }
            }
        }

        private static void BuildScene_Helper1(MenuScene self, string suffix)
        {
            var sep = Path.DirectorySeparatorChar.ToString();
            self.sceneFolder = $"Scenes{sep}meadow - {suffix}";
            if (self.flatMode)
            {
                self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "MeadowMouse - Flat", new Vector2(683f, 384f), false, true));
            }
            else
            {
                self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, $"rm{suffix} bg", new Vector2(0f, 0f), 3.5f, MenuDepthIllustration.MenuShader.Normal));
                self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, $"rm{suffix} lights", new Vector2(0f, 0f), 2.4f, MenuDepthIllustration.MenuShader.SoftLight));
                self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, $"rm{suffix} noot", new Vector2(0f, 0f), 2.2f, MenuDepthIllustration.MenuShader.Normal));
                self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, $"rm{suffix} fg", new Vector2(0f, 0f), 2.1f, MenuDepthIllustration.MenuShader.LightEdges));
                if (self is InteractiveMenuScene ims)
                {
                    ims.idleDepths.Add(3.2f);
                    ims.idleDepths.Add(2.2f);
                    ims.idleDepths.Add(2.1f);
                    ims.idleDepths.Add(2.0f);
                    ims.idleDepths.Add(1.5f);
                }
            }
        }

        private void MenuScene_BuildScene(On.Menu.MenuScene.orig_BuildScene orig, MenuScene self)
        {
            orig(self);
            if (!string.IsNullOrEmpty(self.sceneFolder)) return;

            if (self.sceneID == Slugcat_MeadowLongLegs) BuildScene_Helper1(self, "longlegs");
            else if (self.sceneID == Slugcat_MeadowCentipede) BuildScene_Helper1(self, "centipede");
            else if (self.sceneID == Slugcat_MeadowDropBug) BuildScene_Helper1(self, "dropbug");
            else if (self.sceneID == Slugcat_MeadowPoleMimic) BuildScene_Helper1(self, "polemimic");
            else if (self.sceneID == Slugcat_MeadowBigMoth) BuildScene_Helper1(self, "smallmoth");
            else if (self.sceneID == Slugcat_MeadowBarnacle) BuildScene_Helper1(self, "barnacle");
            // RED LIZARD
            else if (self.sceneID == Slugcat_MeadowRedLizard)
            {
                self.sceneFolder = "Scenes" + Path.DirectorySeparatorChar.ToString() + "meadow - lizard";
                if (self.flatMode)
                {
                    self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "MeadowLizard - Flat", new Vector2(683f, 384f), false, true));
                }
                else
                {
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "rmliz bg", new Vector2(0f, 0f), 3.5f, MenuDepthIllustration.MenuShader.Normal));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "rmliz liz1", new Vector2(0f, 0f), 2.4f, MenuDepthIllustration.MenuShader.Normal));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "rmliz liz2", new Vector2(0f, 0f), 2.2f, MenuDepthIllustration.MenuShader.Normal));
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "rmliz fgplants", new Vector2(0f, 0f), 2.1f, MenuDepthIllustration.MenuShader.LightEdges));
                    if (self is InteractiveMenuScene ims)
                    {
                        ims.idleDepths.Add(3.2f);
                        ims.idleDepths.Add(2.2f);
                        ims.idleDepths.Add(2.1f);
                        ims.idleDepths.Add(2.0f);
                        ims.idleDepths.Add(1.5f);
                    }
                }
            }
            else
            { //no scene valid folder
                return;
            }
            if (string.IsNullOrEmpty(self.sceneFolder)) return;
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

        private static MenuScene.SceneID? PageFromCharacter(RainMeadow.MeadowProgression.Character c)
        {
            if (c == DaddyLongLegs) return Slugcat_MeadowLongLegs;
            else if (c == Centipede) return Slugcat_MeadowCentipede;
            else if (c == DropBug) return Slugcat_MeadowDropBug;
            else if (c == PoleMimic) return Slugcat_MeadowPoleMimic;
            else if (c == BigMoth) return Slugcat_MeadowBigMoth;
            else if (c == Barnacle) return Slugcat_MeadowBarnacle;
            else if (c == RedLizard) return Slugcat_MeadowRedLizard;
            return null;
        }

        private void SlugcatPage_AddImage(On.Menu.SlugcatSelectMenu.SlugcatPage.orig_AddImage orig, SlugcatSelectMenu.SlugcatPage self, bool ascended)
        {
            if (self.slugcatNumber == RainMeadow.RainMeadow.Ext_SlugcatStatsName.OnlineSessionPlayer && self is RainMeadow.MeadowCharacterSelectPage mcsp && PageFromCharacter(mcsp.character) is MenuScene.SceneID sceneID)
            {
                self.sceneOffset = new Vector2(-10f, 100f);
                self.slugcatDepth = 3.1000001f;
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
            if (!init)
            {
                init = true;
                try
                {
                    LongLegsController.EnableLongLegs();
                    CentipedeController.EnableCentipede();
                    DropBugController.EnableDropBug();
                    PoleMimicController.EnablePoleMimic();
                    BigMothController.EnableBigMoth();
                    BarnacleController.EnableBarnacle();

                    // fixups
                    RainMeadow.MeadowProgression.skinData[Centipede_Aquacenti].creatureType ??= DLCSharedEnums.CreatureTemplateType.AquaCenti;
                    RainMeadow.MeadowProgression.skinData[DaddyLongLegs_Purple].creatureType ??= DLCSharedEnums.CreatureTemplateType.TerrorLongLegs;
                    RainMeadow.MeadowProgression.skinData[Lizard_Strawberry].creatureType ??= DLCSharedEnums.CreatureTemplateType.ZoopLizard;
                    RainMeadow.MeadowProgression.skinData[Lizard_Train].creatureType ??= MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.TrainLizard;
                    RainMeadow.MeadowProgression.skinData[BigMoth_Small].creatureType ??= Watcher.WatcherEnums.CreatureTemplateType.SmallMoth;
                    RainMeadow.MeadowProgression.skinData[Barnacle_Normal].creatureType ??= Watcher.WatcherEnums.CreatureTemplateType.Barnacle;

                    {
                        var methFrom = typeof(RainMeadow.CreatureController).GetMethod("BindAvatar", BindingFlags.NonPublic | BindingFlags.Static);
                        var methTo = typeof(DllHooks).GetMethod("BindAvatar", BindingFlags.NonPublic | BindingFlags.Static);
                        var d = new MonoMod.RuntimeDetour.Hook(methFrom, methTo);
                    }
                    {
                        var methFrom = typeof(RainMeadow.MeadowGameMode).GetMethod("SpawnAvatar", BindingFlags.Public | BindingFlags.Instance);
                        var methTo = typeof(DllHooks).GetMethod("SpawnAvatar", BindingFlags.NonPublic | BindingFlags.Static);
                        RainMeadow.RainMeadow.Debug($"{methFrom} -> {methTo}");
                        var d = new MonoMod.RuntimeDetour.Hook(methFrom, methTo);
                    }
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
}
