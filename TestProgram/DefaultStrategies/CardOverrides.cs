using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion;

namespace Program.DefaultStrategies
{
    static class DefaultResponses
    {
        public static MapOfCardsFor<IPlayerAction> GetCardResponses(PlayerAction playerAction)
        {
            var result = new MapOfCardsFor<IPlayerAction>();

            result[Cards.Ambassador]       = new Ambassador(playerAction);
            result[Cards.BandOfMisfits] = new BandOfMisfits(playerAction);
            result[Cards.Cartographer]     = new Cartographer(playerAction);            
            result[Cards.Golem]            = new Golem(playerAction);
            result[Cards.HorseTraders]     = new HorseTraders(playerAction);
            result[Cards.IllGottenGains]   = new IllGottenGainsAlwaysGainCopper(playerAction);
            result[Cards.Library]          = new Library(playerAction);
            result[Cards.MarketSquare]     = new MarketSquare(playerAction);
            result[Cards.Trader]           = new Trader(playerAction);
            result[Cards.Watchtower]       = new Watchtower(playerAction);

            return result;
        }

        public static MapOfCardsFor<GameStatePlayerActionPredicate> GetCardShouldPlayDefaults(PlayerAction playerAction)
        {
            var result = new MapOfCardsFor<GameStatePlayerActionPredicate>();

            result[Cards.Salvager] = Strategies.HasCardToTrashInHand;
            result[Cards.Lookout] = Lookout.ShouldPlay;            

            return result;
        }
    }

    internal class Ambassador
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Ambassador(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override int GetCountToReturnToSupply(Card card, GameState gameState)
        {
            return 2;
        }
    }

    internal class BandOfMisfits
       : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public BandOfMisfits(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromSupplyToPlay(GameState gameState, CardPredicate acceptableCard)
        {
            throw new NotImplementedException();
        }
    }

    internal class Cartographer
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Cartographer(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromRevealedCardsToTopDeck(GameState gameState)
        {
            // good for cartographer, not sure about anyone else.
            foreach (Card card in gameState.Self.CardsBeingRevealed)
            {
                bool shouldDiscard = card.isVictory || card == Cards.Copper;
                if (!shouldDiscard)
                {
                    return card;
                }
            }

            return null;
        }
    }

    internal class Golem
       : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Golem(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card ChooseCardToPlayFirst(GameState gameState, Card card1, Card card2)
        {
            Card result = playerAction.actionOrder.GetPreferredCard(
                gameState,
                card => card == card1 || card == card2);

            // choose a reasonable default
            if (result == null)
            {
                result = playerAction.defaultActionOrder.GetPreferredCard(
                    gameState,
                    card => card == card1 || card == card2);
            }

            return result;
        }
    }

    internal class HorseTraders
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public HorseTraders(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return playerAction.DefaultGetCardFromHandToDiscard(gameState, acceptableCard, isOptional);
        }

        public override bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            return true;
        }
    }

    internal class IllGottenGainsAlwaysGainCopper
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public IllGottenGainsAlwaysGainCopper(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldGainCard(GameState gameState, Card card)
        {
            return true;
        }
    }

    internal class IllGottenGains
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public IllGottenGains(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldGainCard(GameState gameState, Card card)
        {
            Card result = playerAction.gainOrder.GetPreferredCard(
                gameState,
                c => c == card);

            // do a reasonable default for gaining copper on ill gotten gain
            if (result == null)
            {
                return ShouldGainCopper(gameState, playerAction.purchaseOrder);                
            }

            return false;
        }

        private static bool ShouldGainCopper(GameState gameState, ICardPicker gainOrder)
        {
            PlayerState self = gameState.Self;

            int minValue = self.ExpectedCoinValueAtEndOfTurn;
            int maxValue = minValue + Strategies.CountInHand(Cards.IllGottenGains, gameState);

            if (maxValue == minValue)
                return false;

            CardPredicate shouldGainCard = delegate(Card card)
            {
                int currentCardCost = card.CurrentCoinCost(self);

                return currentCardCost >= minValue &&
                        currentCardCost <= maxValue;
            };

            Card cardType = gainOrder.GetPreferredCard(gameState, shouldGainCard);
            if (cardType == null)
                return false;

            int coppersToGain = PlayerAction.CostOfCard(cardType, gameState) - minValue;

            return (coppersToGain > 0);
        }
    }

    internal class Library
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Library(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldPutCardInHand(GameState gameState, Card card)
        {
            return playerAction.discardOrder.DoesCardPickerMatch(gameState, card);
        }
    }

    internal class Lookout
    {
        public static bool ShouldPlay(GameState gameState, PlayerAction playerAction)
        {            
            int cardCountToTrash = Strategies.CountInDeck(Cards.Copper, gameState);

            if (!playerAction.purchaseOrder.DoesCardPickerMatch(gameState, Cards.Estate))
            {
                cardCountToTrash += Strategies.CountInDeck(Cards.Estate, gameState);
            }

            cardCountToTrash += Strategies.CountInDeck(Cards.Curse, gameState);
            cardCountToTrash += Strategies.CountInDeck(Cards.Hovel, gameState);
            cardCountToTrash += Strategies.CountInDeck(Cards.Necropolis, gameState);
            cardCountToTrash += Strategies.CountInDeck(Cards.OvergrownEstate, gameState);

            if (!playerAction.purchaseOrder.DoesCardPickerMatch(gameState, Cards.Lookout))
            {
                cardCountToTrash += Strategies.CountInDeck(Cards.Lookout, gameState);
            }           

            int totalCardsOwned = gameState.Self.CardsInDeck.Count;

            return ((double)cardCountToTrash) / totalCardsOwned > 0.4;
        }
    }

    internal class MarketSquare
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public MarketSquare(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldPlayerDiscardCardFromHand(GameState gameState, Card card)
        {
            return true;
        }
    }    

    internal class Trader
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Trader(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return this.playerAction.DefaultGetCardFromHandToTrash(gameState, acceptableCard, isOptional);
        }

        public override bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor)
        {
            return playerAction.trashOrder.DoesCardPickerMatch(gameState, cardFor) &&
                   !playerAction.purchaseOrder.DoesCardPickerMatch(gameState, cardFor);
        }
    }

    internal class Watchtower
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Watchtower(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            return true;
        }
    }   
}
