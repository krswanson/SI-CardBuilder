﻿using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions.CostConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.ConditionalEffects
{
    [ConditionalEffectAttribute]
    internal class ThisPowerMayBeFastEffect : Effect
    {
        public override double BaseProbability { get { return .05; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 2; } }

        public override Regex descriptionRegex
        {
            get
            {
                return new Regex(@"This power may be fast", RegexOptions.IgnoreCase);
            }
        }

        public override bool Standalone { get { return false; } }

        //Writes what goes on the card
        public override string Print()
        {
            return "This power may be {fast}.";
        }
        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            foreach(Condition c in context.conditions) { 
                //Powers don't target until the slow phase, so land targeting conditions don't make sense
                if (c.GetType() == typeof(LandTypeCondition) || c.GetType() == typeof(PlayPowerSlowCondition)) {
                    return false;
                }
            }
            if (context.card.ContainsSameEffectType(this) || context.card.Fast)
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
            return 0.15;
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
            ThisPowerMayBeFastEffect effect = new ThisPowerMayBeFastEffect();
            effect.Context = Context.Duplicate();
            return effect;
        }
    }
}
