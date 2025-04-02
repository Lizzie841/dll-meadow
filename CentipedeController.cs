using MonoMod.Cil;
using RWCustom;
using System;
using UnityEngine;

namespace DllMeadow
{
    class CentipedeController : RainMeadow.CreatureController
    {
        private bool actLock; // act is hooked both at base and an override
        private bool forceMove;

        private readonly Centipede centipede;

        internal static void EnableCentipede()
        {
            On.Centipede.Update += Centipede_Update;
            On.Centipede.Act += Centipede_Act;
            On.CentipedeAI.Update += CentipedeAI_Update;
        }

        public override WorldCoordinate CurrentPathfindingPosition
        {
            get
            {
                if (!forceMove && Custom.DistLess(creature.coord, centipede.AI.pathFinder.destination, 3))
                {
                    return centipede.AI.pathFinder.destination;
                }
                return base.CurrentPathfindingPosition;
            }
        }

        private static void CentipedeAI_Update(On.CentipedeAI.orig_Update orig, CentipedeAI self)
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

        private static void Centipede_Update(On.Centipede.orig_Update orig, Centipede self, bool eu)
        {
            if (creatureControllers.TryGetValue(self, out var p))
            {
                p.Update(eu);
                var old = self.AI.centipede.abstractCreature.controlled;
                self.AI.centipede.abstractCreature.controlled = true;//глючное
                orig(self, eu);
                self.AI.centipede.abstractCreature.controlled = old;
            }
            else
            {
                orig(self, eu);
            }
        }

        private static void Centipede_Act(On.Centipede.orig_Act orig, Centipede self)
        {
            if (creatureControllers.TryGetValue(self, out var p) && !(p as CentipedeController).actLock)
            {
                p.ConsciousUpdate();
                var old = self.AI.centipede.abstractCreature.controlled;
                self.AI.centipede.abstractCreature.controlled = true;//глючное
                orig(self);
                self.AI.centipede.abstractCreature.controlled = old;
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

        public CentipedeController(Centipede creature, RainMeadow.OnlineCreature oc, int playerNumber, RainMeadow.MeadowAvatarData customization) : base(creature, oc, playerNumber, customization)
        {
            this.centipede = creature;
            //var c1 = this.centipede.effectColor;
            //this.ModifyBodyColor(customization, ref c1);
        }

        protected override void LookImpl(Vector2 pos)
        {
            //centipede.AI.reactTarget = Custom.MakeWorldCoordinate(new IntVector2((int)(pos.x / 20f), (int)(pos.y / 20f)), this.centipede.room.abstractRoom.index);
        }

        protected override void Moving(float magnitude)
        {
            centipede.AI.behavior = CentipedeAI.Behavior.Hunt;
            forceMove = true;
        }

        protected override void Resting()
        {
            centipede.AI.behavior = CentipedeAI.Behavior.Idle;
            forceMove = false;
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
