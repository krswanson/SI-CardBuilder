﻿using OpenQA.Selenium.Internal;
using Spirit_Island_Card_Generator.Classes.CardGenerator;
using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spirit_Island_Card_Generator.Classes.Effects
{
    public abstract class Effect : IGeneratorOption, IPowerLevel
    {
        public abstract double BaseProbability { get; }
        public abstract double AdjustedProbability { get; set; }
        public abstract int Complexity { get; }

        public abstract Regex descriptionRegex { get; }

        protected Context? Context;
        //Whether the effect can be skipped. If it can, it should say "may"
        public bool optional = false;

        //Writes what goes on the card
        public abstract string Print();
        //Initializes effect from a card description.
        public abstract bool Scan(string description);
        //Checks if this should be an option for the card generator
        public abstract bool IsValid(Context context);
        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        protected abstract void InitializeEffect();
        //Estimates the effects own power level
        public abstract double CalculatePowerLevel();

        public abstract IPowerLevel Duplicate();
        //If false, the card must have another effect.
        public virtual bool Standalone { get { return true; } }

        public virtual bool TopLevelEffect()
        {
            return false;
        }

        //Set the context here so I don't have to worry about forgetting to do it in a million subclasses
        public void InitializeEffect(Context context)
        {
            Context = context;
            InitializeEffect();
        }
        /// <summary>
        /// Some conditional effects may want to do a stronger version of what an effect did already. Effects that support this can override this function to choose stronger versions of their effects
        /// So for example, a card may have a base effect of defend 1. A new effect being generated is trying to add a new effect with the condition: "if the target land is jungle/sands". The new condition wants to upgrade the defend instead of generating a different type of effect
        /// So it calls this function and if the effect can be upgraded it returns a new effect with a stronger effect, such as defend 4.
        /// </summary>
        /// <param name="card">The card so far</param>
        /// <param name="settings">Settings for the whole deck generation. This will mostly want the Target power level and the power level variance</param>
        /// <returns></returns>
        public abstract Effect? Strengthen();
        /// <summary>
        /// Similar to Strenthen, but makes the effect weaker instead
        /// </summary>
        /// <param name="card"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public abstract Effect? Weaken();
        /// <summary>
        /// Utility function to pick a random generatorOption from a list of possible options
        /// </summary>
        /// <param name="card"></param>
        /// <param name="settings"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        //public static IGeneratorOption? ChooseEffect(Context context, IEnumerable<IGeneratorOption> options)
        //{
        //    double totalWeight = 0;
        //    List<IGeneratorOption> validOptions = new List<IGeneratorOption>();
        //    foreach (IGeneratorOption option in options)
        //    {
        //        if (option.IsValid(card, settings))
        //        {
        //            totalWeight += option.AdjustedProbability;
        //            validOptions.Add(option);
        //        }
        //    }

        //    double choice = settings.rng.NextDouble() * totalWeight;
        //    double index = 0;
        //    foreach (IGeneratorOption option in validOptions)
        //    {
        //        if (choice < index + option.AdjustedProbability)
        //        {
        //            return option;
        //        }
        //        index += option.AdjustedProbability;
        //    }
        //    return default(IGeneratorOption);
        //}

        protected Context UpdateContext()
        {
            Context newContext = new Context();
            newContext.rng = Context.rng;
            newContext.card = Context.card;
            newContext.settings = Context.settings;
            newContext.effectGenerator = Context.effectGenerator;
            newContext.conditions = new List<Conditions.Condition>(Context.conditions);
            newContext.target = Context.target.CreateShallowCopy();

            newContext.chain = new List<IGeneratorOption>(Context.chain);
            newContext.chain.Add(this);
            return newContext;
        }
    }
}
