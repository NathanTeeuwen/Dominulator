using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public interface IGameLog
        : IDisposable
    {
        void BeginScope();
        void EndScope();
        void BeginRound();
        void BeginTurn(PlayerState playerState);
        void EndTurn(PlayerState playerState);
        void PlayerBoughtCard(PlayerState playerState, Card card);
        void GainedCard(PlayerState playerState, Card card);
        void PlayedCard(PlayerState playerState, Card card);
        void PlayerGainedCard(PlayerState playerState, Card card);
        void PlayerDiscardCard(PlayerState playerState, Card card);
        void PlayerTrashedCard(PlayerState playerState, Card card);
        void DrewCardIntoHand(PlayerState playerState, Card card);
        void DiscardedCard(PlayerState playerState, Card card);
        void ReshuffledDiscardIntoDeck(PlayerState playerState);
        void EndGame(GameState gameState);
        void PlayerGainedCoin(PlayerState playerState, int coinAmount);
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

    public class EmptyGameLog
        : IGameLog
    {
        public void Dispose()
        {         
        }

        public void BeginRound()
        {            
        }

        public void BeginScope()
        {

        }

        public void EndScope()
        {

        }

        public void BeginTurn(PlayerState playerState)
        {            
        }

        public void EndTurn(PlayerState playerState)
        {         
        }

        public void PlayerBoughtCard(PlayerState playerState, Card card)
        {         
        }

        public void GainedCard(PlayerState playerState, Card card)
        {         
        }

        public void DrewCardIntoHand(PlayerState playerState, Card card)
        {         
        }

        public void DiscardedCard(PlayerState playerState, Card card)
        {         
        }

        public void PlayerGainedCard(PlayerState playerState, Card card)
        {         
        }

        public void PlayerDiscardCard(PlayerState playerState, Card card)
        {
        }

        public void PlayerTrashedCard(PlayerState playerState, Card card)
        {         
        }

        public void PlayedCard(PlayerState playerState, Card card)
        {         
        }

        public void ReshuffledDiscardIntoDeck(PlayerState playerState)
        {         
        }

        public void EndGame(GameState gameState)
        {        
        }

        public void PlayerGainedCoin(PlayerState playerState, int coinAmount)
        {            
        }
    }

    internal class IndentedTextWriter
        : IDisposable
    {
        private bool isNewLine = true;
        int indentLevel = 0;
        System.IO.TextWriter textWriter;

        public IndentedTextWriter(string filename)
        {
            this.textWriter = new System.IO.StreamWriter(filename);
        }

        public void Indent()
        {
            this.indentLevel++;
        }

        public void Unindent()
        {
            this.indentLevel--;
        }

        public void Write(string format, params object[] args)
        {
            IndentIfNewLine();
            this.textWriter.Write(format, args);
        }

        public void WriteLine(string format, params object[] args)
        {
            IndentIfNewLine();
            this.textWriter.WriteLine(format, args);
            this.isNewLine = true;
        }

        public void WriteLine()
        {
            this.textWriter.WriteLine();
            this.isNewLine = true;
        }

        private void IndentIfNewLine()
        {
            if (this.isNewLine)
            {
                for (int i = 0; i < this.indentLevel; ++i)
                {
                    this.textWriter.Write("  ");
                }

                this.isNewLine = false;
            }
        }

        public void Dispose()
        {
            this.textWriter.Dispose();
        }
    }

    public class HumanReadableGameLog
        : IGameLog, IDisposable
    {
        int roundNumber = 0;
        IndentedTextWriter textWriter;

        public HumanReadableGameLog(string filename)
        {
            this.textWriter = new IndentedTextWriter(filename);
        }

        public void Dispose()
        {
            this.textWriter.Dispose();
        }

        public void BeginScope()
        {
            this.textWriter.Indent();
        }

        public void EndScope()
        {
            this.textWriter.Unindent();
        }

        public void BeginRound()
        {
            this.textWriter.WriteLine("ROUND {0}", ++this.roundNumber);
            this.textWriter.WriteLine("-------------");
        }

        public void BeginTurn(PlayerState playerState)
        {
            this.textWriter.WriteLine("{0} begins turn", playerState.actions.PlayerName);            
            this.textWriter.Write("With hand: ");

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

        public void PlayerBoughtCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} bought {1}.", playerState.actions.PlayerName, card.name);
        }

        public void GainedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Gained {1}.", playerState.actions.PlayerName, card.name);
        }

        public void DrewCardIntoHand(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Drew {1} into hand.", playerState.actions.PlayerName, card.name);
        }

        public void DiscardedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Discarded {1}.", playerState.actions.PlayerName, card.name);
        }

        public void PlayerGainedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} gained {1}.", playerState.actions.PlayerName, card.name);
        }

        public void PlayerTrashedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} trashed {1}.", playerState.actions.PlayerName, card.name);
        }

        public void PlayedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Played {1}.", playerState.actions.PlayerName, card.name);
        }

        public void PlayerDiscardCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Discarded {1}.", playerState.actions.PlayerName, card.name);
        }

        public void ReshuffledDiscardIntoDeck(PlayerState playerState)
        {
            this.textWriter.WriteLine("{0} reshuffled", playerState.actions.PlayerName);
        }

        public void EndGame(GameState gameState)
        {
            this.textWriter.WriteLine("Game ended in {0} turns.", this.roundNumber);

            PlayerState[] winners = gameState.WinningPlayers;

            if (winners.Length == 1)
            {
                this.textWriter.WriteLine("{0} Won!", winners[0].actions.PlayerName);
            }
            else
            {
                this.textWriter.Write("There was a tie between: ");
                foreach (PlayerState player in winners)
                {
                    this.textWriter.Write("{0}, ", player.actions.PlayerName);
                }
                this.textWriter.WriteLine();
            }         

            foreach (PlayerState player in gameState.players.AllPlayers)
            {
                this.textWriter.WriteLine("{0} total score: {1}", player.actions.PlayerName, player.TotalScore());
            }
        }

        public void PlayerGainedCoin(PlayerState playerState, int coinAmount)
        {            
            this.textWriter.WriteLine("+{0} Coin = {1} all together.", coinAmount, playerState.AvailableCoins);         
        }
    }
}
