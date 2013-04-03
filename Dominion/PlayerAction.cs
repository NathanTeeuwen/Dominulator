using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public interface IPlayerActionFactory
    {
        IPlayerAction NewPlayerAction(PlayerState playerState);
    }

    public interface IPlayerAction
    {        
        void BeginTurn();        
        void EndTurn();        
        Type BanCardForCurrentPlayerRevealedCards(GameState gameState);
        Type BanCardForCurrentPlayerPurchase(GameState gameState);
        Type GetActionFromHandToPlay(GameState gameState, bool isOptional);
        Type GetTreasureFromHandToPlay(GameState gameState);        
        Type GetCardFromSupplyToBuy(GameState gameState);        
        Type GuessCardTopOfDeck(GameState gameState);        
        Type GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional);        
        Type GetCardFromRevealedCarsToTopDeck(BagOfCards revealedCards);
        Type GetCardFromRevealedCardsToTrash(PlayerState player, BagOfCards revealedCards, CardPredicate acceptableCard);        
        Type GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard);
        Type GetCardFromHandToPassLeft(GameState gameState);
        Type GetCardFromHandToDiscard(GameState gameState, bool isOptional);
        Type GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard);
        bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card);
        bool ShouldPutCardInHand(GameState gameState, Card card);
        bool WantToResign(GameState gameState);
        bool ShouldRevealCard(GameState gameState, Card card);
        bool ShouldPutDeckInDiscard(GameState gameState);
        bool ShouldTrashCard(GameState gameState);
        bool ShouldGainCard(GameState gameState, Card card);        
        PlayerActionChoice ChooseAction(GameState gameState, IsValidChoice acceptableChoice);
        string PlayerName { get; }
    }

    public interface IGameLog
        : IDisposable
    {
        void BeginRound();
        void BeginTurn(PlayerState playerState);
        void EndTurn(PlayerState playerState);
        void BoughtCard(PlayerState playerState, Card card);
        void GainedCard(PlayerState playerState, Card card);
        void PlayedCard(PlayerState playerState, Card card);
        void DrewCardIntoHand(PlayerState playerState, Card card);
        void DiscardedCard(PlayerState playerState, Card card);
        void ReshuffledDiscardIntoDeck(PlayerState playerState);
        void EndGame(GameState gameState);
    }

    public class DefaultPlayerAction
        : IPlayerAction
    {        

        private Type NoCard()
        {
            return null;
        }

        private void NoDefaultAction()
        {

        }

        private Type PlayerMustMakeCardChoice()
        {
            throw new NotImplementedException();
        }

        private bool PlayerMustMakeChoice()
        {
            throw new NotImplementedException();
        }

        private PlayerActionChoice PlayerMustMakeActionChoice()
        {
            throw new NotImplementedException();
        }

        private Type NoCardIfOptional(bool isOptional)
        {
            if (isOptional)
            {
                return NoCard();
            }

            return PlayerMustMakeCardChoice();
        }

        virtual public string PlayerName 
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        virtual public void BeginTurn()
        {
            NoDefaultAction();
        }

        virtual public void EndTurn()
        {
            NoDefaultAction();
        }

        virtual public Type BanCardForCurrentPlayerRevealedCards(GameState gameState)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Type BanCardForCurrentPlayerPurchase(GameState gameState)
        {            
            return PlayerMustMakeCardChoice();
        }

        virtual public Type GetActionFromHandToPlay(GameState gameState, bool isOptional)
        {
            return NoCardIfOptional(isOptional);
        }

        virtual public Type GetTreasureFromHandToPlay(GameState gameState)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Type GetCardFromSupplyToBuy(GameState gameState)
        {
            return NoCard();
        }

        virtual public Type GuessCardTopOfDeck(GameState gameState)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Type GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return NoCardIfOptional(isOptional);   
        }

        virtual public Type GetCardFromRevealedCarsToTopDeck(BagOfCards revealedCards)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Type GetCardFromRevealedCardsToTrash(PlayerState player, BagOfCards revealedCards, CardPredicate acceptableCard)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Type GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Type GetCardFromHandToPassLeft(GameState gameState)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public Type GetCardFromHandToDiscard(GameState gameState, bool isOptional)
        {
            return NoCardIfOptional(isOptional);
        }

        virtual public Type GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard)
        {
            return PlayerMustMakeCardChoice();
        }

        virtual public bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card)
        {
            return PlayerMustMakeChoice();
        }

        virtual public bool ShouldPutCardInHand(GameState gameState, Card card)
        {
            return PlayerMustMakeChoice();
        }

        virtual public bool WantToResign(GameState gameState)
        {
            return false;
        }

        virtual public bool ShouldRevealCard(GameState gameState, Card card)
        {
            return PlayerMustMakeChoice();
        }

        virtual public bool ShouldPutDeckInDiscard(GameState gameState)
        {
            return PlayerMustMakeChoice();
        }

        virtual public bool ShouldTrashCard(GameState gameState)
        {
            return PlayerMustMakeChoice();
        }

        virtual public bool ShouldGainCard(GameState gameState, Card card)
        {
            return PlayerMustMakeChoice();
        }

        virtual public PlayerActionChoice ChooseAction(GameState gameState, IsValidChoice acceptableChoice)
        {
            return PlayerMustMakeActionChoice();
        }
    }

    public class DefaultLog
        : IGameLog, IDisposable
    {
        int roundNumber = 0;
        System.IO.TextWriter textWriter = new System.IO.StreamWriter("..\\..\\Results\\GameLog.txt");

        public void Dispose()
        {
            this.textWriter.Dispose();
        }

        public void BeginRound()
        {
            this.textWriter.WriteLine("ROUND {0}", ++this.roundNumber);
            this.textWriter.WriteLine("-------------");
        }

        public void BeginTurn(PlayerState playerState)
        {
            this.textWriter.WriteLine("{0} begins turn", playerState.actions.PlayerName);
            this.textWriter.Write("     With hand: ");

            foreach(Card card in playerState.Hand.OrderBy(card => card.name))
            {
                this.textWriter.Write(card.name + ",");
            }
            this.textWriter.WriteLine();
        }

        public void EndTurn(PlayerState playerState)
        {
            this.textWriter.WriteLine("{0} ends turn", playerState.actions.PlayerName);
            this.textWriter.WriteLine();
            this.textWriter.WriteLine();
        }

        public void BoughtCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} bought {1}", playerState.actions.PlayerName, card.name);
        }

        public void GainedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Gained {1}", playerState.actions.PlayerName, card.name);
        }

        public void DrewCardIntoHand(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Drew {1} into hand.", playerState.actions.PlayerName, card.name);
        }

        public void DiscardedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Discarded {1}", playerState.actions.PlayerName, card.name);
        }

        public void PlayedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Played {1}", playerState.actions.PlayerName, card.name);
        }

        public void ReshuffledDiscardIntoDeck(PlayerState playerState)
        {
            this.textWriter.WriteLine("{0} reshuffled", playerState.actions.PlayerName);
        }

        public void EndGame(GameState gameState)
        {
            this.textWriter.WriteLine("Game ended in {0} turns.", this.roundNumber);

            foreach (PlayerState player in gameState.players.AllPlayers.OrderByDescending(player => player.TotalScore()))
            {
                this.textWriter.WriteLine("{0} total score: {1}", player.actions.PlayerName, player.TotalScore());
            }
        }
    }
}
