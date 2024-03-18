﻿using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.SpiritEffects
{
    [SpiritEffect]
    internal class ExtraCardPlayEffect : Effect
    {
        public override double BaseProbability { get { return .07; } }
        public override double AdjustedProbability { get { return .07; } set { } }
        public override int Complexity { get { return 1; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"target spirit may (?:immediately)? play another Power Card by paying its cost", RegexOptions.IgnoreCase);
            }
        }

        //Writes what goes on the card
        public override string Print()
        {
            return $"Target Spirit may immediately play another Power Card by paying its cost.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            if (!context.target.SpiritTarget || !context.card.Fast)
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
            return 0.75;
        }

        /// <summary>
        /// Some conditional effects may want to do a stronger version of what an effect did already. Effects that support this can override this function to choose stronger versions of their effects
        /// So for example, a card may have a base effect of defend 1. A new effect being generated is trying to add a new effect with the condition: "if the target land is jungle/sands". The new condition wants to upgrade the defend instead of generating a different type of effect
        /// So it calls this function and if the effect can be upgraded it returns a new effect with a stronger effect, such as defend 4.
        /// </summary>
        /// <param name="card">The card so far</param>
        /// <param name="settings">Settings for the whole deck generation. This will mostly want the Target power level and the power level variance</param>
        /// <returns></returns>
        public override Effect? Strengthen()
        {
            return null;
        }

        public override Effect? Weaken()
        {
            return null;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);

            return match.Success;
        }

        public override Effect Duplicate()
        {
            ExtraCardPlayEffect effect = new ExtraCardPlayEffect();
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
