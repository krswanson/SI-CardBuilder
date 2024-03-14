﻿using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DestroyEffects
{
    [LandEffect]
    internal class DahanDestroyEffect : DestroyEffect
    {
        public override double BaseProbability { get { return .02; } }
        public override int Complexity { get { return 2; } }
        public override GamePieces.Piece Piece => GamePieces.Piece.Dahan;

        protected override Dictionary<int, double> ExtraPiecesMultiplier => new Dictionary<int, double>()
        {
            { 1, 1.0},
            { 2, 0.8}
        };

        protected override double PieceStrength => -0.2;

        public override IPowerLevel Duplicate()
        {
            DahanDestroyEffect effect = new DahanDestroyEffect();
            effect.Context = Context;
            effect.amount = amount;
            return effect;
        }

        protected override void InitializeEffect()
        {
            amount = 1;
        }

        public override bool IsValid(Context context)
        {
            if (context.target.landConditions.Contains(LandConditon.LandConditions.NoDahan))
                return false;
            return true;
        }
    }
}
