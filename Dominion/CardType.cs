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
                case CardType.Castle:    return "Castle";
                case CardType.Curse:     return "Curse";
                case CardType.Doom:      return "Doom";
                case CardType.Duration:  return "Duration";
                case CardType.Fate:      return "Fate";
                case CardType.Gathering: return "Gathering";
                case CardType.Heirloom:  return "Heirloom";
                case CardType.Prize:     return "Prize";
                case CardType.Night:     return "Night";
                case CardType.Reaction:  return "Reaction";
                case CardType.Reserve:   return "Reserve";
                case CardType.Ruins:     return "Ruins";
                case CardType.Shelter:   return "Shelter";
                case CardType.Spirit:    return "Spirit";
                case CardType.Traveller: return "Traveller";
                case CardType.Treasure:  return "Treasure";
                case CardType.Victory:  return "Victory";
                case CardType.Zombie: return "Zombie";
                default: throw new NotImplementedException();
            }
        }

        public static string CardTypeToStringPlural(this Dominion.CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Action: return "Actions";
                case CardType.Attack: return "Attacks";
                case CardType.Castle: return "Castles";
                case CardType.Curse: return null;
                case CardType.Doom: return null;
                case CardType.Duration: return "Durations";
                case CardType.Fate: return null;
                case CardType.Gathering: return null;
                case CardType.Night: return null;
                case CardType.Heirloom: return "Heirlooms";
                case CardType.Prize: return "Prizes";
                case CardType.Reaction: return "Reactions";
                case CardType.Reserve: return null;
                case CardType.Ruins: return "Ruins";
                case CardType.Spirit: return "Spirits";
                case CardType.Shelter: return "Shelters";
                case CardType.Treasure: return "Treasures";
                case CardType.Traveller: return "Travellers";
                case CardType.Victory: return null;
                case CardType.Zombie: return "Zombies";
                default: throw new NotImplementedException();
            }
        }
    }
}
