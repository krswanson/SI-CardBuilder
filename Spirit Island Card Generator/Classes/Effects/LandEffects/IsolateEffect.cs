﻿using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects
{
    [LandEffect]
    public class IsolateEffect : Effect
    {
        public override double BaseProbability { get { return .04; } }
        public override double AdjustedProbability { get { return .04; } set { } }
        public override int Complexity { get { return 3; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"Isolate target land", RegexOptions.IgnoreCase);
            }
        }

        //Writes what goes on the card
        public override string Print()
        {
            return $"Isolate target land.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            if (context.card.ContainsSameEffectType(this) || context.target.SpiritTarget || !context.card.Fast)
                return false;
            else
                return true;
        }
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            
        }
        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            return 0.2;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);

            return match.Success;
        }

        public override IsolateEffect Duplicate()
        {
            IsolateEffect effect = new IsolateEffect();
            effect.Context = Context.Duplicate();
            return effect;
        }

        public override Effect? Strengthen()
        {
            return null;
        }

        public override Effect? Weaken()
        {
            return null;
        }
    }
}
