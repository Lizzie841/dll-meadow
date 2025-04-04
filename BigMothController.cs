using MonoMod.Cil;
using RWCustom;
using System;
using UnityEngine;

namespace DllMeadow
{
    class BigMothController : RainMeadow.CreatureController
    {
        private bool actLock; // act is hooked both at base and an override
        private bool forceMove;

        private readonly Watcher.BigMoth bigmoth;

        internal static void EnableBigMoth()
        {
            On.Watcher.BigMoth.Update += BigMoth_Update;
            On.Watcher.BigMoth.Act += BigMoth_Act;
            On.Watcher.BigMothAI.Update += BigMothAI_Update;
        }

        public override WorldCoordinate CurrentPathfindingPosition
        {
            get
            {
                if (bigmoth != null)
                {
                    if (!forceMove && Custom.DistLess(creature.coord, bigmoth.AI.pathFinder.destination, 3))
                    {
                        return bigmoth.AI.pathFinder.destination;
                    }
                }
                return base.CurrentPathfindingPosition;
            }
        }

        private static void BigMothAI_Update(On.Watcher.BigMothAI.orig_Update orig, Watcher.BigMothAI self)
        {
            if (creatureControllers.TryGetValue(self.creature.realizedCreature, out var p))
            {
                p.AIUpdate(self);
            }
            else
            {
                orig(self);
            }
        }

        private static void BigMoth_Update(On.Watcher.BigMoth.orig_Update orig, Watcher.BigMoth self, bool eu)
        {
            if (creatureControllers.TryGetValue(self, out var p))
            {
                p.Update(eu);
                var old = self.AI.bug.abstractCreature.controlled;
                self.AI.bug.abstractCreature.controlled = true;//глючное
                orig(self, eu);
                self.AI.bug.abstractCreature.controlled = old;
            }
            else
            {
                orig(self, eu);
            }
        }

        private static void BigMoth_Act(On.Watcher.BigMoth.orig_Act orig, Watcher.BigMoth self)
        {
            if (creatureControllers.TryGetValue(self, out var p) && !(p as BigMothController).actLock)
            {
                p.ConsciousUpdate();
                var old = self.AI.bug.abstractCreature.controlled;
                self.AI.bug.abstractCreature.controlled = true;//глючное
                orig(self);
                self.AI.bug.abstractCreature.controlled = old;
            }
            else
            {
                orig(self);
            }
        }

        internal void ModifyBodyColor(RainMeadow.MeadowAvatarData self, ref Color ogColor)
        {
            if (self.skinData.baseColor.HasValue)
            {
                ogColor = self.skinData.baseColor.Value;
            }
            if (self.effectiveTintAmount > 0f)
            {
                var hslTint = RainMeadow.Extensions.ToHSL(self.tint);
                var hslOgColor = RainMeadow.Extensions.ToHSL(ogColor);
                ogColor = Color.Lerp(HSLColor.Lerp(hslOgColor, hslTint, self.effectiveTintAmount).rgb, Color.Lerp(ogColor, self.tint, self.effectiveTintAmount), 0.5f); // lerp in average of hsl and rgb, neither is good on its own
            }
        }

        public BigMothController(Watcher.BigMoth creature, RainMeadow.OnlineCreature oc, int playerNumber, RainMeadow.MeadowAvatarData customization) : base(creature, oc, playerNumber, customization)
        {
            this.bigmoth = creature;
            //var c1 = this.bigmoth.effectColor;
            //this.ModifyBodyColor(customization, ref c1);
        }

        protected override void LookImpl(Vector2 pos)
        {
            //bigmoth.AI.reactTarget = Custom.MakeWorldCoordinate(new IntVector2((int)(pos.x / 20f), (int)(pos.y / 20f)), this.bigmoth.room.abstractRoom.index);
        }

        protected override void Moving(float magnitude)
        {
            if (bigmoth != null)
            {
                bigmoth.AI.behavior = Watcher.BigMothAI.Behavior.Idle;
                forceMove = true;
            }
        }

        protected override void Resting()
        {
            if (bigmoth != null)
            {
                bigmoth.AI.behavior = Watcher.BigMothAI.Behavior.Idle;
                forceMove = false;
            }
        }

        protected override void OnCall()
        {
            //truly
        }

        protected override void PointImpl(Vector2 dir)
        {
            //uh
        }
    }
}
