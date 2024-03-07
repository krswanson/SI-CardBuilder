﻿using Spirit_Island_Card_Generator.Classes.Effects.LandEffects.GatherEffects;
using Spirit_Island_Card_Generator.Classes.GameConcepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Spirit_Island_Card_Generator.Classes.ElementSet;
using static Spirit_Island_Card_Generator.Classes.GameConcepts.GamePieces;

namespace Spirit_Island_Card_Generator.Classes.Effects.LandEffects.PushEffects
{
    public class PushEffect : LandEffect
    {
        public override double BaseProbability { get { return .26; } }
        public override double AdjustedProbability { get { return .26; } set { } }
        public override int Complexity { get { return 2; } }

        public List<PiecePush> pushOptions = new List<PiecePush>();
        //public virtual Piece piece { get { throw new NotImplementedException(); } }
        //Must VS. May
        public bool mandatory = false;
        public int amount = 1;

        protected virtual bool Valid(Card card, Settings settings)
        {
            throw new NotImplementedException();
        }

        public override Regex descriptionRegex
        {
            get
            {
                List<string> pieceNames = new List<string>();
                foreach (Piece piece in Enum.GetValues(typeof(Piece)))
                {
                    pieceNames.Add(piece.ToString());
                }
                string allPiecesRegex = "(?:"+String.Join("|", pieceNames)+")";
                return new Regex(@"Push (up to )?(\d) ((?:" + allPiecesRegex + "\\/?)+)", RegexOptions.IgnoreCase);
            }
        }

        //Checks if this should be an option for the card generator
        public override bool IsValid(Card card, Settings settings)
        {
            foreach(PiecePush pushEffects in pushOptions)
            {
                if (!pushEffects.IsValid(card, settings))
                {
                    return false;
                }
            }
            return true;
        }

        //Chooses what exactly the effect should be (how much damage/fear/defense/etc...)
        public override void InitializeEffect(Card card, Settings settings)
        {
            //TODO: Care about power level
            if (card.CardType == Card.CardTypes.Minor)
            {
                int numberOfPieces = settings.rng.Next(1, 4);
                for(int i = 0; i < numberOfPieces; i++)
                {
                    pushOptions.Add(ChooseRandomPushEffect(card, settings));
                }
                amount = settings.rng.Next(1, 4);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool HasPushType(Piece piece)
        {
            foreach (PiecePush pushEffect in pushOptions)
            {
                if (pushEffect.piece == piece)
                {
                    return true;
                }
            }
            return false;
        }

        private PiecePush ChooseRandomPushEffect(Card card, Settings settings)
        {
            double totalWeight = 0;
            List<PiecePush> pushOptions = ReflectionManager.GetInstanciatedSubClasses<PiecePush>();
            List<PiecePush> validOptions = new List<PiecePush>();
            foreach (PiecePush effect in pushOptions)
            {
                if (!HasPushType(effect.piece) && effect.IsValid(card, settings))
                {
                    totalWeight += effect.AdjustedProbability;
                    validOptions.Add(effect);
                }
            }

            double choice = settings.rng.NextDouble() * totalWeight;
            double index = 0;
            foreach(PiecePush effect in validOptions)
            {
                if (choice < index + effect.AdjustedProbability)
                {
                    return effect;
                }
                index += effect.AdjustedProbability;
            }
            throw new Exception("No choice made");
        }

        //Estimates the effects own power level
        public override double CalculatePowerLevel()
        {
            //If there's more than one options, the strongest option will be most of the power level since players will usually do that.
            //For now, we'll just use a flat power level for each other option.
            //TODO: If they are close in power, the increase from having more than one option becomes larger.
            double powerLevel = 0;
            double strongestOptionPower = 0;
            foreach (PiecePush pushEffect in pushOptions)
            {
                if (pushEffect.CalculatePowerLevel() > strongestOptionPower)
                {
                    strongestOptionPower = pushEffect.CalculatePowerLevel() * amount;
                }
            }
            powerLevel += strongestOptionPower + (0.1 * pushOptions.Count);
            //Decrease the power for redundent pieces a bit since there may not be enough pieces to get the full value
            if (amount == 2)
            {
                powerLevel *= 0.85;
            } else if (amount >= 3)
            {
                powerLevel *= 0.8;
            }
            return powerLevel;
        }

        public override string Print()
        {
            string mayText = mandatory ? "" : "up to ";
            List<string> convertedPieceTexts = new List<string>();
            foreach (PiecePush pushEffect in pushOptions)
            {
                convertedPieceTexts.Add(GamePieces.ToSIBuilderString(pushEffect.piece));
            }
            string piecesText = String.Join("/",convertedPieceTexts);
            return $"Push {mayText}{amount} {piecesText}";
        }

        public override bool Scan(string description)
        {
            Match match = descriptionRegex.Match(description);
            if (match.Success)
            {
                if (match.Groups[1].Value.Equals(""))
                {
                    mandatory = true;
                }
                amount = Int32.Parse(match.Groups[2].Value);
                string pieceString = match.Groups[3].Value;
                foreach (string piece in pieceString.Split("/"))
                {
                    List<PiecePush> PushEffects = ReflectionManager.GetInstanciatedSubClasses<PiecePush>();
                    foreach(PiecePush pushType in PushEffects)
                    {
                        if (pushType.piece.ToString().Equals(piece,StringComparison.InvariantCultureIgnoreCase))
                        {
                            pushOptions.Add(pushType);
                        }
                    }
                }
            }
            return match.Success;
        }

        public override Effect Duplicate()
        {
            PushEffect effect = new PushEffect();
            effect.amount = amount;
            foreach (PiecePush piece in pushOptions)
            {
                effect.pushOptions.Add(piece);
            }
            return effect;
        }

        public override Effect? Strengthen(Card card, Settings settings)
        {
            if (card.CardType == Card.CardTypes.Minor)
            {
                PushEffect newEffect = (PushEffect)Duplicate();
                newEffect.amount += 1;
                if (newEffect.CalculatePowerLevel() > settings.TargetPowerLevel + settings.PowerLevelVariance)
                {
                    //Add another piece
                    newEffect = (PushEffect)Duplicate();
                    newEffect.pushOptions.Add(ChooseRandomPushEffect(card, settings));
                    //This is not guaranteed to be better, but the card selector will confirm that
                    return newEffect;

                }
                else
                {
                    return newEffect;
                }

            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override Effect? Weaken(Card card, Settings settings)
        {
            if (card.CardType == Card.CardTypes.Minor)
            {
                PushEffect newEffect = (PushEffect)Duplicate();
                newEffect.amount -= 1;
                if (newEffect.amount <= 0 || newEffect.CalculatePowerLevel() > settings.TargetPowerLevel + settings.PowerLevelVariance)
                {
                    if (pushOptions.Count > 1)
                    {
                        //Remove an option
                        newEffect = (PushEffect)Duplicate();
                        PiecePush piece = Utils.ChooseRandomListElement(pushOptions, settings.rng);
                        newEffect.pushOptions.Remove(piece);
                        return newEffect;
                    }
                    else
                    {
                        //Add another piece
                        newEffect = (PushEffect)Duplicate();
                        newEffect.pushOptions.Clear();
                        newEffect.pushOptions.Add(ChooseRandomPushEffect(card, settings));
                        //This is not guaranteed to be better, but the card selector will confirm that
                        return newEffect;
                    }
                }
                else
                {
                    return newEffect;
                }

            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
