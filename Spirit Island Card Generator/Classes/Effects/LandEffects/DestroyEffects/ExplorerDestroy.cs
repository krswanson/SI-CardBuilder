﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.DestroyEffects
{
    internal class ExplorerDestroy : PieceDestroyed
    {
        public override double BaseProbability { get { return .06; } }
        public override double AdjustedProbability { get { return .06; } set { } }
        public override int Complexity { get { return 1; } }
        public override Piece piece { get { return Piece.Explorer; } }

        public override bool IsValid(Card card, Settings settings)
        {
            return true;
        }

        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            //TODO: work with the calculated power levels
            return 0.85;
        }
    }
}
