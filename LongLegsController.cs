using MonoMod.Cil;
using RWCustom;
using System;
using UnityEngine;

namespace DllMeadow
{
    class LongLegsController : RainMeadow.CreatureController
    {
        private bool actLock; // act is hooked both at base and an override
        private bool forceMove;

        private readonly DaddyLongLegs longlegs;

        internal static void EnableLongLegs()
        {
            On.DaddyLongLegs.Update += DaddyLongLegs_Update;
            On.DaddyLongLegs.Act += DaddyLongLegs_Act;
            On.DaddyAI.Update += DaddyAI_Update;
        }

        public override WorldCoordinate CurrentPathfindingPosition
        {
            get
            {
                if (!forceMove && Custom.DistLess(creature.coord, longlegs.AI.pathFinder.destination, 3))
                {
                    return longlegs.AI.pathFinder.destination;
                }
                return base.CurrentPathfindingPosition;
            }
        }

        private static void DaddyAI_Update(On.DaddyAI.orig_Update orig, DaddyAI self)
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

        private static void DaddyLongLegs_Update(On.DaddyLongLegs.orig_Update orig, DaddyLongLegs self, bool eu)
        {
            if (creatureControllers.TryGetValue(self, out var p))
            {
                p.Update(eu);
                var old = self.AI.daddy.abstractCreature.controlled;
                self.AI.daddy.abstractCreature.controlled = true;//глючное
                orig(self, eu);
                self.AI.daddy.abstractCreature.controlled = old;
            }
            else
            {
                orig(self, eu);
            }
        }

        private static void DaddyLongLegs_Act(On.DaddyLongLegs.orig_Act orig, DaddyLongLegs self, int legsGrabbing)
        {
            if (creatureControllers.TryGetValue(self, out var p) && !(p as LongLegsController).actLock)
            {
                p.ConsciousUpdate();
                var old = self.AI.daddy.abstractCreature.controlled;
                self.AI.daddy.abstractCreature.controlled = true;//глючное
                orig(self, legsGrabbing);
                self.AI.daddy.abstractCreature.controlled = old;
            }
            else
            {
                orig(self, legsGrabbing);
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

        public LongLegsController(DaddyLongLegs creature, RainMeadow.OnlineCreature oc, int playerNumber, RainMeadow.MeadowAvatarData customization) : base(creature, oc, playerNumber, customization)
        {
            this.longlegs = creature;
            var c1 = this.longlegs.effectColor;
            this.ModifyBodyColor(customization, ref c1);
            this.longlegs.effectColor = c1;
            var c2 = this.longlegs.eyeColor;
            this.ModifyBodyColor(customization, ref c2);
            this.longlegs.eyeColor = c2;
        }

        protected override void LookImpl(Vector2 pos)
        {
            longlegs.AI.reactTarget = Custom.MakeWorldCoordinate(new IntVector2((int)(pos.x / 20f), (int)(pos.y / 20f)), this.longlegs.room.abstractRoom.index);
            //longlegs.abstractAI.SetDestination(this.reactTarget);
			//longlegs.AI.pathFinder.ForceNextDestination();
            //longlegs.lookDir = (pos - creature.DangerPos).normalized;
            //longlegs.getToLookDir = longlegs.lookDir;
        }

        protected override void Moving(float magnitude)
        {
            longlegs.AI.behavior = DaddyAI.Behavior.Hunt;
            //longlegs.AI.flySpeed = Custom.LerpAndTick(longlegs.AI.flySpeed, magnitude, 0.2f, 0.05f);
            forceMove = true;
        }

        protected override void Resting()
        {
            longlegs.AI.behavior = DaddyAI.Behavior.Idle;
            //longlegs.AI.flySpeed = Custom.LerpAndTick(longlegs.AI.flySpeed, 0, 0.4f, 0.1f);
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
