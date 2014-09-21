using Dominion;
using Dominion.Strategy.Description;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.Strategy
{
    public delegate bool GameStatePlayerActionPredicate(GameState gameState, DefaultPlayerAction playerAction);

    public class PlayerAction
        : PlayerActionFromCardResponses
    {
        public PlayerAction(
            string name,            
            ICardPicker purchaseOrder,
            ICardPicker actionOrder = null,
            bool chooseDefaultActionOnNone = true,
            ICardPicker treasurePlayOrder = null,
            ICardPicker discardOrder = null,
            ICardPicker trashOrder = null,
            ICardPicker gainOrder = null)
            : base(new DefaultPlayerAction(name, purchaseOrder, actionOrder, chooseDefaultActionOnNone, treasurePlayOrder, discardOrder, trashOrder, gainOrder))
        {

        }

        public ICardPicker purchaseOrder { get { return this.playerAction.purchaseOrder;  } }
        public ICardPicker actionOrder { get { return this.playerAction.actionOrder; } set { this.playerAction.actionOrder = value; } }
        public bool chooseDefaultActionOnNone { get { return this.playerAction.chooseDefaultActionOnNone; } }
        public ICardPicker treasurePlayOrder { get { return this.playerAction.treasurePlayOrder; } }
        public ICardPicker discardOrder { get { return this.playerAction.discardOrder; } }
        public ICardPicker trashOrder { get { return this.playerAction.trashOrder; } }
        public ICardPicker gainOrder { get { return this.playerAction.gainOrder; } }

        public static void SetKingdomCards(GameConfigBuilder builder, params PlayerAction[] players)
        {
            var allCards = new List<Card>();
            foreach (PlayerAction player in players)
            {
                AddCards(allCards, player.actionOrder);
                AddCards(allCards, player.purchaseOrder);
                AddCards(allCards, player.gainOrder);
            }

            builder.SetKingdomPiles(allCards);
        }

        private static void AddCards(List<Card> cardSet, ICardPicker matchingCards)
        {
            foreach (Card card in matchingCards.GetNeededCards())
            {
                cardSet.Add(card);
            }
        }
    }
}
