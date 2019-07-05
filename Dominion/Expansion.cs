using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public enum Expansion
    {
        Alchemy,
        Adventures,
        Base,
        Cornucopia,
        DarkAges,
        Empires,
        Guilds,
        Hinterlands,
        Intrigue,
        Nocturne,
        Promo,
        Prosperity,
        Renaissance,
        Seaside,
        Unknown,
        Count = Unknown
    }

    public static class ExpansionExtensionMethods
    {
        public static string ToProgramaticName(this Dominion.Expansion expansion)
        {
            return ExpansionToString(expansion).Replace(" ", "_");
        }

        public static string ExpansionToString(this Dominion.Expansion expansion)
        {
            switch (expansion)
            {
                case Dominion.Expansion.Alchemy: return "Alchemy";
                case Dominion.Expansion.Base: return "Base";
                case Dominion.Expansion.Cornucopia: return "Cornucopia";
                case Dominion.Expansion.DarkAges: return "Dark Ages";
                case Dominion.Expansion.Guilds: return "Guilds";
                case Dominion.Expansion.Hinterlands: return "Hinterlands";
                case Dominion.Expansion.Intrigue: return "Intrigue";
                case Dominion.Expansion.Promo: return "Promo";
                case Dominion.Expansion.Prosperity: return "Prosperity";
                case Dominion.Expansion.Seaside: return "Seaside";
                case Dominion.Expansion.Adventures: return "Adventures";
                case Dominion.Expansion.Empires: return "Empires";
                case Dominion.Expansion.Nocturne: return "Nocturne";
                case Dominion.Expansion.Renaissance: return "Renaissance";
                default: throw new NotImplementedException();
            }
        }
    }

}
