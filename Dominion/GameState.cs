using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{    
    public enum DeckPlacement
    {
        Hand,
        Discard,
        Play,
        Trash
    }

    public struct CardPlacementPair
    {
        public readonly Card card;
        public readonly DeckPlacement placement;

        public CardPlacementPair(Card card, DeckPlacement placement)
        {
            this.card = card;
            this.placement = placement;
        }
    }

    public delegate int VictoryPointCounter(PlayerState player);
    public delegate DeckPlacement MapCardToPlacement(Card card);
    public delegate CardPlacementPair PlaceCardsFromList(BagOfCards cards);
    public delegate bool CardPredicate(Card card);    
    public delegate bool IsValidChoice(PlayerActionChoice availableChoice);    

    public enum CostConstraint
    {
        Exactly,
        UpTo
    }

    public enum CardRelativeCost
    {
        RelativeCost,
        AbsoluteCost
    }

    
    public class CardPile
        : ListOfCards
    {
        Card protoType;

        internal int embargoTokenCount;
        internal bool tradeRouteTokenCount;
        internal readonly bool isInSupply;

        public CardPile(Card protoType, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                this.AddCardToTop(protoType);
            }            

            this.protoType = protoType;            
        }

        public bool IsType(Type card)
        {            
            return this.protoType.Is(card);            
        }

        public bool IsType<T>()
        {
            return IsType(typeof(T));
        }        
    }

    public class GameState
    {
        public IGameLog gameLog;
        public PlayerCircle players;
        public CardPile[] supplyPiles;
        public BagOfCards trash;

        // special piles not in the supply - not available in every game
        private CardPile blackMarketDeck;

        public GameState(             
            IGameLog gameLog,
            IPlayerActionFactory playerActionFactory,
            Card[] InitialCardTypes,
            int playerCount, 
            bool usePlatinumAndColony)
        {
            this.gameLog = gameLog;
            this.players = new PlayerCircle(playerCount, playerActionFactory, this.gameLog);
 
            var cardPiles = new List<CardPile>(capacity:20);

            int curseCount = (playerCount - 1) * 10;
            int victoryCount = (playerCount == 2) ? 8 : 12;

            Add<CardTypes.Copper>(cardPiles, 60);
            Add<CardTypes.Silver>(cardPiles, 40);
            Add<CardTypes.Gold>(cardPiles, 20);
            Add<CardTypes.Curse>(cardPiles, curseCount);
            Add<CardTypes.Estate>(cardPiles, victoryCount + playerCount * 3);
            Add<CardTypes.Duchy>(cardPiles, victoryCount);
            Add<CardTypes.Province>(cardPiles, victoryCount);

            foreach (Card card in InitialCardTypes)
            {
                if (card.isVictory)
                {
                    Add(cardPiles, victoryCount, card);
                }
                else
                {
                    Add(cardPiles, card.defualtSupplyCount, card);
                }
            }

            this.supplyPiles = cardPiles.ToArray();
                       
            foreach (PlayerState player in this.players.AllPlayers)
            {
                player.GainCardsFromSupply(this, typeof(CardTypes.Estate), 3);
                player.GainCardsFromSupply(this, typeof(CardTypes.Copper), 7);                
            }

            this.players.AllPlayersDrawInitialCards();         
        }

        private static void Add<CardType>(List<CardPile> cardPiles, int initialCount)
            where CardType : Card, new()
        {
            
            Add(cardPiles, initialCount, new CardType());
        }

        private static void Add(List<CardPile> cardPiles, int initialCount, Card protoType)           
        {                        
            cardPiles.Add( new CardPile(protoType, initialCount));
        }


        public bool IsVictoryConditionReached()
        {
            int countEmptyPiles = 0;
            foreach (CardPile pile in this.supplyPiles)
            {
                if (pile.IsEmpty)
                {
                    if (pile.IsType<CardTypes.Province>() ||
                        pile.IsType<CardTypes.Colony>())
                    {
                        return true;
                    }

                    ++countEmptyPiles;
                }                
            }

            if (countEmptyPiles >= 3)
            {
                return true;
            }

            return false;
        }

        public void PlayGameToEnd()
        {
            while (!IsVictoryConditionReached())
            {
                if (this.players.BeginningOfRound)
                {
                    this.gameLog.BeginRound();
                }

                PlayerState currentPlayerState = this.players.CurrentPlayer;
                PlayTurn(currentPlayerState);
                if (this.players.CurrentPlayer.actions.WantToResign(this))
                {
                    break;
                }
                this.players.PassTurnLeft();                
            }

            this.gameLog.EndGame(this);
        }

        public void PlayTurn(PlayerState currentPlayerState)
        {            
            IPlayerAction currentPlayer = currentPlayerState.actions;

            this.gameLog.BeginTurn(currentPlayerState);
            currentPlayerState.InitializeTurn();
            currentPlayer.BeginTurn();            

            DoActionPhase(currentPlayerState);
            DoPlayTreasures(currentPlayerState);
            DoBuyPhase(currentPlayerState);            
            currentPlayerState.CleanupPhase();
            currentPlayerState.DrawUntilCountInHand(5);
            currentPlayer.EndTurn();
            this.gameLog.EndTurn(currentPlayerState);
        }

        private void DoActionPhase(PlayerState currentPlayer)
        {            
            while (currentPlayer.availableActionCount > 0)
            {
                currentPlayer.availableActionCount--;

                if (!currentPlayer.RequestPlayerPlayActionFromHand(this, isOptional: true))
                {
                    break;
                }                
            }
        }

        private void DoBuyPhase(PlayerState currentPlayerState)
        {
            while (currentPlayerState.availableBuys > 0)
            {
                Type cardType = currentPlayerState.actions.GetCardFromSupplyToBuy(this);
                if (cardType == null)
                {
                    return;
                }

                Card gainedCard = this.PlayerGainCardFromSupply(cardType, currentPlayerState);
                if (gainedCard == null)
                {
                    return;
                }
                this.gameLog.BoughtCard(currentPlayerState, gainedCard);
                currentPlayerState.availableBuys-=1;
            }
        }

        internal void DoPlayTreasures(PlayerState currentPlayer)
        {
            while (true)
            {
                Type cardTypeToPlay = currentPlayer.actions.GetTreasureFromHandToPlay(this);
                if (cardTypeToPlay == null)
                {
                    break;
                }

                Card currentCard = currentPlayer.RemoveCardFromHand(cardTypeToPlay);
                if (currentCard == null)
                {
                    throw new Exception("Player tried to remove a card that wasn't available in hand");
                }
                
                currentPlayer.DoPlayTreasure(currentCard, this);
            }
        }

        internal CardPile GetSpecialPile(Type cardType)
        {
            if (cardType.Equals(typeof(CardTypes.BlackMarket)))
            {
                return this.blackMarketDeck;
            }
            else
            {
                throw new Exception("Card type does not have a special pile");
            }
        }

        private CardPile GetPile(Type cardType)
        {
            for (int i = 0; i < this.supplyPiles.Length; ++i)
            {
                CardPile cardPile = this.supplyPiles[i];
                if (cardPile.IsType(cardType))
                {
                    return cardPile;
                }
            }

            return null;
        }

        public CardPile GetPile<cardType>()
        {
            for (int i = 0; i < this.supplyPiles.Length; ++i)
            {
                CardPile cardPile = this.supplyPiles[i];
                if (cardPile.IsType<cardType>())
                {
                    return cardPile;
                }
            }

            return null;
        }

        public Card PlayerGainCardFromSupply(Type cardType, PlayerState playerState, DeckPlacement defaultLocation = DeckPlacement.Discard)
        {
            CardPile pile = this.GetPile(cardType);
            if (pile == null)
            {
                return null;
            }

            Card card = pile.DrawCardFromTop();
            if (card == null)
            {
                return null;
            }

            playerState.GainCard(card, defaultLocation);

            return card;
        }

        public void PlayerGainCardFromSupply<cardType>(PlayerState playerState)
        {
            PlayerGainCardFromSupply(typeof(cardType), playerState);            
        }
    }
        
    public enum PlayerActionChoice
    {
        PlusCoin,
        Discard,
        PlusCard,
        PlusAction,
        PlusBuy,
        GainCard,
        Trash
    }      
}
