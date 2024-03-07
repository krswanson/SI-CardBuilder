﻿using OpenQA.Selenium.Internal;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.TargetConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.TargetConditions.LandConditon;

namespace Spirit_Island_Card_Generator.Classes.Effects.Conditions
{
    internal class LandTypeCondition : Condition
    {
        public override double BaseProbability => 0.2;

        public override double AdjustedProbability { get => BaseProbability; set { } }

        public override int Complexity => 3;

        struct WeightAndMultiplier
        {
            public double multiplier;
            public int weight;

            public WeightAndMultiplier(double m, int w)
            {
                multiplier = m;
                weight = w;
            }
        }

        static Dictionary<LandConditions, WeightAndMultiplier> conditions = new Dictionary<LandConditions, WeightAndMultiplier>()
        {
            { LandConditions.Inland, new WeightAndMultiplier(0.9, 2)},
            { LandConditions.Coastal, new WeightAndMultiplier(0.7, 2)},

            { LandConditions.Mountain, new WeightAndMultiplier(0.6, 2)},
            { LandConditions.Jungle, new WeightAndMultiplier(0.6, 2)},
            { LandConditions.Sands, new WeightAndMultiplier(0.6, 2)},
            { LandConditions.Wetlands, new WeightAndMultiplier(0.6, 2)},

            { LandConditions.NoMountain, new WeightAndMultiplier(0.9, 1)},
            { LandConditions.NoJungle, new WeightAndMultiplier(0.9, 1)},
            { LandConditions.NoSands, new WeightAndMultiplier(0.9, 1)},
            { LandConditions.NoWetlands, new WeightAndMultiplier(0.9, 1)},

            { LandConditions.MountainOrJungle, new WeightAndMultiplier(0.8, 4)},
            { LandConditions.MountainOrSands, new WeightAndMultiplier(0.8, 4)},
            { LandConditions.MountainOrWetlands, new WeightAndMultiplier(0.8, 4)},
            { LandConditions.JungleOrSands, new WeightAndMultiplier(0.8, 4)},
            { LandConditions.JungleOrWetlands, new WeightAndMultiplier(0.8, 4)},
            { LandConditions.SandsOrWetlands, new WeightAndMultiplier(0.8, 4)},

            { LandConditions.Blighted, new WeightAndMultiplier(0.55, 2)},
            { LandConditions.Noblight, new WeightAndMultiplier(0.85, 3)},

            { LandConditions.Dahan, new WeightAndMultiplier(0.7, 3)},
            { LandConditions.NoDahan, new WeightAndMultiplier(0.75, 1)},

            { LandConditions.Invaders, new WeightAndMultiplier(0.8, 2)},
            { LandConditions.NoInvaders, new WeightAndMultiplier(0.65, 2)},
            { LandConditions.Buildings, new WeightAndMultiplier(0.7, 2)},
            { LandConditions.NoBuildings, new WeightAndMultiplier(0.7, 2)},
            { LandConditions.City, new WeightAndMultiplier(0.5, 2)},
            { LandConditions.NoCity, new WeightAndMultiplier(0.9, 2)},
        };

        public override double DifficultyMultiplier {
            get {
                return conditions[landCondition].multiplier;
            }
        }

        public override string ConditionText { 
            get {
                switch (landCondition)
                {
                    case LandConditon.LandConditions.Noblight:
                    case LandConditon.LandConditions.NoBuildings:
                    case LandConditon.LandConditions.NoDahan:
                    case LandConditon.LandConditions.Dahan:
                    case LandConditon.LandConditions.Blighted:
                    case LandConditon.LandConditions.City:
                    case LandConditon.LandConditions.Buildings:
                    case LandConditon.LandConditions.Invaders:
                        return "If target land has " + LandConditon.Print(landCondition) + ",";
                    default:
                        return "If target land is " + LandConditon.Print(landCondition) + ",";
                }
            } 
        }

        public LandConditon.LandConditions landCondition;

        public override IPowerPowerLevel Duplicate()
        {
            LandTypeCondition condition = new LandTypeCondition();
            condition.landCondition = landCondition;
            return condition;
        }

        public override bool IsValid(Card card, Settings settings)
        {
            return true;
        }

        public override void Initialize(Card card, Settings settings)
        {
            Dictionary<LandConditions, int> weights = new Dictionary<LandConditions, int>();
            List<LandConditions> compatibleConditions = LandConditon.GetCompatibleLandConditions(card.Target.landConditions);
            foreach (LandConditions condition in conditions.Keys)
            {
                if (compatibleConditions.Contains(condition))
                    weights.Add(condition, conditions[condition].weight);
            }

            landCondition = Utils.ChooseWeightedOption(weights, settings.rng);
        }

        public override bool ChooseHarderCondition(Card card, Settings settings)
        {
            Dictionary<LandConditions, int> weights = new Dictionary<LandConditions, int>();
            List<LandConditions> compatibleConditions = LandConditon.GetCompatibleLandConditions(card.Target.landConditions);

            foreach (LandConditions condition in conditions.Keys)
            {
                if (compatibleConditions.Contains(condition) && conditions[condition].multiplier < DifficultyMultiplier)
                    weights.Add(condition, conditions[condition].weight);
            }

            LandConditions? newCondition =  Utils.ChooseWeightedOption(weights, settings.rng);
            if (newCondition.HasValue)
            {
                landCondition = newCondition.Value;
                return true;
            } else
            {
                return false;
            }
        }
        public override bool ChooseEasierCondition(Card card, Settings settings)
        {
            Dictionary<LandConditions, int> weights = new Dictionary<LandConditions, int>();
            List<LandConditions> compatibleConditions = LandConditon.GetCompatibleLandConditions(card.Target.landConditions);

            foreach (LandConditions condition in conditions.Keys)
            {
                if (compatibleConditions.Contains(condition) && conditions[condition].multiplier > DifficultyMultiplier)
                    weights.Add(condition, conditions[condition].weight);
            }

            LandConditions? newCondition = Utils.ChooseWeightedOption(weights, settings.rng);
            if (newCondition.HasValue)
            {
                landCondition = newCondition.Value;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
