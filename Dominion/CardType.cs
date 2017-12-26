using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public enum CardType
    {
        Action,
        Attack,
        Castle,
        Curse,
        Doom,
        Duration,
        Fate,
        Gathering,
        Heirloom,
        Prize,
        Night,
        Reaction,
        Reserve,
        Ruins,
        Shelter,
        Spirit,
        Treasure,
        Traveller,
        Victory,
        Zombie
    }

    public static class CardTypeExtensionMethods
    {
        public static string CardTypeToString(this Dominion.CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Action:    return "Action";
                case CardType.Attack:    return "Attack";
                case CardType.Curse:     return "Curse";
                case CardType.Duration:  return "Duration";
                case CardType.Gathering: return "Gathering";
                case CardType.Prize:     return "Prize";
                case CardType.Reaction:  return "Reaction";
                case CardType.Reserve:   return "Reserve";
                case CardType.Ruins:     return "Ruins";
                case CardType.Shelter:   return "Shelter";
                case CardType.Treasure:  return "Treasure";
                case CardType.Traveller: return "Traveller";
                default: throw new NotImplementedException();
            }
        }

        public static string CardTypeToStringPlural(this Dominion.CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Action: return "Actions";
                case CardType.Attack: return "Attacks";
                case CardType.Curse: return null;
                case CardType.Duration: return "Durations";
                case CardType.Gathering: return null;
                case CardType.Prize: return "Prizes";
                case CardType.Reaction: return "Reactions";
                case CardType.Reserve: return null;
                case CardType.Ruins: return "Ruins";
                case CardType.Shelter: return "Shelters";
                case CardType.Treasure: return "Treasures";
                case CardType.Traveller: return "Travellers";
                default: throw new NotImplementedException();
            }
        }
    }
}
