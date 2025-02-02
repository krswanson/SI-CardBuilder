﻿using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.AddEffect
{
    [LandEffect]
    [CustomEffect(1)]
    internal class VitalityAddEffect : AddEffect
    {
        public override string Name => "Add Vitality";
        public override List<Element> StronglyAssociatedElements { get { return new List<Element>() { Element.Plant, Element.Water }; } }
        public override List<Element> WeaklyAssociatedElements { get { return new List<Element>() { Element.Earth }; } }
        public override double BaseProbability { get { return .04; } }
        public override int Complexity { get { return 2; } }

        public override List<Type> IncompatibleEffects { get { return new List<Type>() { typeof(BlightAddEffect), typeof(BlightGatherEffect) }; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Vitality;

        protected override Dictionary<int, double> ExtraAmountMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0 },
            { 2, 1.5 },
        };

        public override double effectStrength => 1.1;

        public override IPowerLevel Duplicate()
        {
            VitalityAddEffect effect = new VitalityAddEffect();
            effect.Context = Context.Duplicate();
            effect.addAmount = addAmount;
            return effect;
        }

        protected override void InitializeEffect()
        {
            addAmount = 1;
        }

        public override bool IsValidGeneratorOption(Context context)
        {
            if (context.target.landConditions.Contains(TargetConditions.LandConditon.LandConditions.Blighted))
                return false;

            return true;
        }
    }
}
