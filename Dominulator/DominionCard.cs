using System;
using System.Linq;

namespace Dominulator
{
    public class DominionCard
    {

        public DominionCard(string name)
        {
            this.Name = name;
        }

        public DominionCard(Windows.Data.Json.JsonValue jsonDescription)
        {
            var dictionary = jsonDescription.GetObject();
            this.Id = dictionary.GetNamedString("id");
            this.Name = dictionary.GetNamedString("name");
            this.Coin = (int)dictionary.GetNamedNumber("coin");
            this.Potion = (int)dictionary.GetNamedNumber("potion");
            this.Expansion = GetExpansionIndex(dictionary.GetNamedString("expansion"));
            this.IsAction = dictionary.GetNamedBoolean("isAction");
            this.IsAttack = dictionary.GetNamedBoolean("isAttack");
            this.IsReaction = dictionary.GetNamedBoolean("isReaction");
            this.IsDuration = dictionary.GetNamedBoolean("isDuration");
            this.isWebCard = true;
            this.dominionCard = null;
        }

        private DominionCard(Dominion.Card card)
        {
            this.Id = card.ProgrammaticName;
            this.Name = card.name;
            this.Coin = card.DefaultCoinCost;
            this.Potion = card.potionCost;
            this.Expansion = GetExpansionIndex(card.expansion);
            this.IsAttack = card.isAttack;
            this.IsAction = card.isAction;
            this.IsReaction = card.isReaction;
            this.IsDuration = card.isDuration;
            this.isWebCard = false;
            this.dominionCard = card;
        }

        static System.Collections.Generic.Dictionary<string, DominionCard> mpCardNameToCard = BuildNameMap();

        static System.Collections.Generic.Dictionary<string, DominionCard> BuildNameMap()
        {
            var result = new System.Collections.Generic.Dictionary<string, DominionCard>();
            
            foreach(Dominion.Card card in Dominion.Cards.AllCards())
            {
                result[card.name] = new DominionCard(card);
            }

            return result;
        }

        public static DominionCard Create(Dominion.Card card)
        {
            if (card == null)
                return null;
            return mpCardNameToCard[card.name];
        }

        public static DominionCard Create(string name)
        {
            return mpCardNameToCard[name];
        }

        public readonly Dominion.Card dominionCard;
        public string Name { get; private set; }
        public string Id { get; private set; }
        public int Coin { get; private set; }
        public int Potion { get; private set; }
        public ExpansionIndex Expansion { get; private set; }
        public bool IsAction { get; private set; }
        public bool IsAttack { get; private set; }
        public bool IsReaction { get; private set; }
        public bool IsDuration { get; private set; }
        private bool isWebCard;

        public bool CanSimulate
        {
            get
            {
                return !Dominion.Strategy.MissingDefaults.UnImplementedKingdomCards().Contains(this.dominionCard);
            }
        }

        public string ImageUrl
        {
            get
            {
                string uri = "ms-appx://Win8Client/Resources/" + this.Id + ".jpg";                
                return isWebCard ? "http://localhost:8081/dominion" + "/resources/cards/" + this.Id + ".jpg"
                    : uri;
            }
        }

        public static Dominion.Expansion GetExpansionForIndex(ExpansionIndex index)
        {
            switch (index)
            {
                case ExpansionIndex.Alchemy: return Dominion.Expansion.Alchemy;
                case ExpansionIndex.Base: return Dominion.Expansion.Base;
                case ExpansionIndex.Cornucopia: return Dominion.Expansion.Cornucopia;
                case ExpansionIndex.DarkAges: return Dominion.Expansion.DarkAges;
                case ExpansionIndex.Guilds: return Dominion.Expansion.Guilds;
                case ExpansionIndex.Hinterlands: return Dominion.Expansion.Hinterlands;
                case ExpansionIndex.Intrigue: return Dominion.Expansion.Intrigue;
                case ExpansionIndex.Promo: return Dominion.Expansion.Promo;
                case ExpansionIndex.Prosperity: return Dominion.Expansion.Prosperity;
                case ExpansionIndex.Seaside: return Dominion.Expansion.Seaside;
                case ExpansionIndex.Adventures: return Dominion.Expansion.Adventures;
            }
            throw new Exception("Expansion not found");
        }

        private static ExpansionIndex GetExpansionIndex(Dominion.Expansion expansion)
        {
            switch (expansion)
            {
                case Dominion.Expansion.Alchemy: return ExpansionIndex.Alchemy;
                case Dominion.Expansion.Base: return ExpansionIndex.Base;
                case Dominion.Expansion.Cornucopia: return ExpansionIndex.Cornucopia;
                case Dominion.Expansion.DarkAges: return ExpansionIndex.DarkAges;
                case Dominion.Expansion.Guilds: return ExpansionIndex.Guilds;
                case Dominion.Expansion.Hinterlands: return ExpansionIndex.Hinterlands;
                case Dominion.Expansion.Intrigue: return ExpansionIndex.Intrigue;
                case Dominion.Expansion.Promo: return ExpansionIndex.Promo;
                case Dominion.Expansion.Prosperity: return ExpansionIndex.Prosperity;
                case Dominion.Expansion.Seaside: return ExpansionIndex.Seaside;
                case Dominion.Expansion.Adventures: return ExpansionIndex.Adventures;
                case Dominion.Expansion.Empires: return ExpansionIndex.Empires;
                case Dominion.Expansion.Nocturne: return ExpansionIndex.Nocturne;
                case Dominion.Expansion.Unknown: return ExpansionIndex._Unknown;
            }
            throw new Exception("Expansion not found");
        }

        private static ExpansionIndex GetExpansionIndex(string value)
        {
            switch (value)
            {
                case "Alchemy": return ExpansionIndex.Alchemy;
                case "Base": return ExpansionIndex.Base;
                case "Cornucopia": return ExpansionIndex.Cornucopia;
                case "DarkAges": return ExpansionIndex.DarkAges;
                case "Guilds": return ExpansionIndex.Guilds;
                case "Hinterlands": return ExpansionIndex.Hinterlands;
                case "Intrigue": return ExpansionIndex.Intrigue;
                case "Promo": return ExpansionIndex.Promo;
                case "Prosperity": return ExpansionIndex.Prosperity;
                case "Seaside": return ExpansionIndex.Seaside;
                case "Empires": return ExpansionIndex.Empires;
            }

            return ExpansionIndex._Unknown;
        }
    }
}