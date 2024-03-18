﻿using Spirit_Island_Card_Generator.Classes.Attributes;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions;
using Spirit_Island_Card_Generator.Classes.Effects.Conditions.CostConditions;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects;
using Spirit_Island_Card_Generator.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects.GlobalEffects
{
    [LandEffect]
    [SpiritEffect]
    internal class CostConditionEffect : Effect, IParentEffect
    {
        public override double BaseProbability { get { return .15; } }
        public override double AdjustedProbability { get { return BaseProbability; } set { } }
        public override int Complexity { get { return 4; } }

        private Condition costCondition;
        public List<Effect> Effects = new List<Effect>();
        public double minPowerLevel = 0.2;
        public double maxPowerLevel = 0.5;

        public override bool Standalone { get { return false; } }

        public override Regex descriptionRegex
        {
            get
            {
                //This is unlikely to work since the Katalog has a different format from SI builder. The latter being the format the condition text is in currently.
                return new Regex(costCondition.ConditionText, RegexOptions.IgnoreCase);
            }
        }

        //Checks if this should be an option for the card generator
        public override bool IsValid(Context context)
        {
            if (context.chain.Count > 0)
                return false;
            return true;
        }

        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected override void InitializeEffect()
        {
            if (Context.target.SpiritTarget)
            {
                costCondition = (Condition?)Context.effectGenerator.ChooseGeneratorOption<Condition>(new List<Attribute>() { new CostConditionAttribute() }, new List<Attribute>() { new LandConditionAttribute() }, UpdateContext());
            } else
            {
                costCondition = (Condition?)Context.effectGenerator.ChooseGeneratorOption<Condition>(new List<Attribute>() { new CostConditionAttribute() }, new List<Attribute>() { new SpiritConditionAttribute() }, UpdateContext());
            }
            
            costCondition.Initialize(UpdateContext());
            Context.conditions.Add(costCondition);
            Effects.Add((Effect)Context.effectGenerator.ChooseEffect(UpdateContext()));
        }

        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            double powerLevel = 0;
            foreach (Effect effects in Effects)
            {
                powerLevel += effects.CalculatePowerLevel() * costCondition.DifficultyMultiplier;
            }
            return powerLevel;
        }

        public override string Print()
        {
            string conditionText = costCondition.ConditionText;
            string effectText = "";
            bool first = true;
            foreach (Effect effect in Effects)
            {
                if (!first)
                {
                    effectText += " and ";
                }
                else
                {
                    first = false;
                }
                effectText += effect.Print();
            }

            return conditionText + " " + effectText;
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
                //TODO: make the scan work
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            CostConditionEffect dupEffect = new CostConditionEffect();
            dupEffect.costCondition = (Condition?)costCondition?.Duplicate();
            dupEffect.Context = Context.Duplicate();
            foreach (Effect effect in Effects)
            {
                dupEffect.Effects.Add((Effect)effect.Duplicate());
            }
            return dupEffect;
        }

        public override Effect? Strengthen()
        {
            CostConditionEffect strongerThis = (CostConditionEffect)Duplicate();
            //Either make the condition easier to meet, or make the effect stronger
            double roll = Context.rng.NextDouble() * 100;
            if (roll < 50 && strongerThis.costCondition.ChooseEasierCondition(UpdateContext()))
            {
                if (strongerThis.AcceptablePowerLevel())
                    return strongerThis;
            }

            Effect? effectToStrengthen = Utils.ChooseRandomListElement(strongerThis.Effects, Context.rng);

            Effect? strongerEffect = effectToStrengthen?.Strengthen();
            if (strongerEffect != null && effectToStrengthen != null)
            {
                strongerThis.Effects.Remove(effectToStrengthen);
                strongerThis.Effects.Add(strongerEffect);
                if (strongerThis.AcceptablePowerLevel())
                    return strongerThis;
            }
            return null;
        }

        public override Effect? Weaken()
        {
            CostConditionEffect weakerThis = (CostConditionEffect)Duplicate();
            //Either make the condition easier to meet, or make the effect stronger
            double roll = Context.rng.NextDouble() * 100;
            if (roll < 50 && weakerThis.costCondition.ChooseHarderCondition(UpdateContext()))
            {
                if (weakerThis.AcceptablePowerLevel())
                    return weakerThis;
            }

            Effect? effectToWeaken = Utils.ChooseRandomListElement(weakerThis.Effects, Context.rng);

            Effect? weakerEffect = effectToWeaken?.Weaken();
            if (weakerEffect != null && effectToWeaken != null)
            {
                weakerThis.Effects.Remove(effectToWeaken);
                weakerThis.Effects.Add(weakerEffect);
                if (weakerThis.AcceptablePowerLevel())
                    return weakerThis;
            }

            return null;
        }

        public bool AcceptablePowerLevel()
        {
            double powerLevel = CalculatePowerLevel();
            return powerLevel >= minPowerLevel && powerLevel <= maxPowerLevel;
        }

        public List<Effect> GetChildren()
        {
            return Effects;
        }
    }
}
