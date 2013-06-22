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
        void PushScope();
        void PopScope();
        void BeginRound();
        void BeginTurn(PlayerState playerState);
        void EndTurn(PlayerState playerState);
        void PlayerNamedCard(PlayerState playerState, Card card);
        void PlayerRevealedCard(PlayerState playerState, Card card, DeckPlacement source);
        void PlayerBoughtCard(PlayerState playerState, Card card);
        void GainedCard(PlayerState playerState, Card card);
        void PlayedCard(PlayerState playerState, Card card);
        void ReceivedDurationEffectFrom(PlayerState playerState, Card card);
        void PlayerGainedCard(PlayerState playerState, Card card);
        void PlayerDiscardCard(PlayerState playerState, Card card);
        void PlayerTrashedCard(PlayerState playerState, Card card);
        void PlayerPutCardInHand(PlayerState playerState, Card card);
        void PlayerTopDeckedCard(PlayerState playerState, Card card);
        void DrewCardIntoHand(PlayerState playerState, Card card);
        void DiscardedCard(PlayerState playerState, Card card);
        void ReshuffledDiscardIntoDeck(PlayerState playerState);
        void EndGame(GameState gameState);
        void PlayerGainedCoin(PlayerState playerState, int coinAmount);
        void PlayerGainedActions(PlayerState playerState, int actionAmount);
        void PlayerGainedBuys(PlayerState playerState, int actionAmount);
        void LogDeck(PlayerState playerState);
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

        public void PushScope()
        {

        }

        public void PopScope()
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

        public void PlayerRevealedCard(PlayerState playerState, Card card, DeckPlacement source)
        {

        }

        public void PlayerNamedCard(PlayerState playerState, Card card)
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

        public void PlayerPutCardInHand(PlayerState playerState, Card card)
        {

        }

        public void PlayerTopDeckedCard(PlayerState playerState, Card card)
        {
        }

        public void PlayedCard(PlayerState playerState, Card card)
        {
        }

        public void ReceivedDurationEffectFrom(PlayerState playerState, Card card)
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

        public void PlayerGainedActions(PlayerState playerState, int coinAmount)
        {

        }

        public void PlayerGainedBuys(PlayerState playerState, int coinAmount)
        {

        }

        public void LogDeck(PlayerState playerState)
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

        public void PushScope()
        {
            this.textWriter.Indent();
        }

        public void PopScope()
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

            foreach (Card card in playerState.Hand.OrderBy(card => card.name))
            {
                this.textWriter.Write(card.name + ",");
            }
            this.textWriter.WriteLine();
        }

        public void EndTurn(PlayerState playerState)
        {
            this.textWriter.Write("{0} ends turn with deck: ", playerState.actions.PlayerName);
            //this.PushScope();
            this.WriteAllCards(playerState);
            //this.PopScope();
            //this.textWriter.WriteLine();
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

        public void PlayerNamedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} named {1}.", playerState.actions.PlayerName, card.name);
        }

        public void PlayerTrashedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} trashed {1}.", playerState.actions.PlayerName, card.name);
        }

        public void PlayerPutCardInHand(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} put {1} into his hand.", playerState.actions.PlayerName, card.name);
        }

        public void PlayerTopDeckedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Placed {1} on top of his deck.", playerState.actions.PlayerName, card.name);
        }

        public void ReceivedDurationEffectFrom(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Finished Playing {1}.", playerState.actions.PlayerName, card.name);
        }

        public void PlayedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Played {1}.", playerState.actions.PlayerName, card.name);
        }

        public void PlayerDiscardCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Discarded {1}.", playerState.actions.PlayerName, card.name);
        }

        public void PlayerRevealedCard(PlayerState playerState, Card card, DeckPlacement source)
        {
            this.textWriter.WriteLine("{0} Revealed {1}.", playerState.actions.PlayerName, card.name);
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
                this.PushScope();
                this.WriteAllCards(player);
                this.PopScope();
            }

            this.textWriter.Write("Trash contains: ");
            this.WriteAllCards(gameState.trash);            
        }

        public void PlayerGainedCoin(PlayerState playerState, int coinAmount)
        {
            this.textWriter.WriteLine("+{0} Coin = {1} all together.", coinAmount, playerState.AvailableCoins);
        }

        public void PlayerGainedActions(PlayerState playerState, int actionAmount)
        {
            this.textWriter.WriteLine("+{0} Actions = {1} all together.", actionAmount, playerState.AvailableActions);
        }

        public void PlayerGainedBuys(PlayerState playerState, int buyAmount)
        {
            this.textWriter.WriteLine("+{0} Buys = {1} all together.", buyAmount, playerState.AvailableBuys);
        }        

        private void WriteAllCards(PlayerState playerState)
        {
            WriteAllCards(playerState.AllOwnedCards);
        }

        private void WriteAllCards(IEnumerable<Card> enumerable)
        {
            Card[] allCards = enumerable.ToArray<Card>();

            var cardComparer = new CompareCardByType();
            Array.Sort(allCards, cardComparer);

            for (int index = 0; index < allCards.Length;)
            {
                Card currentCard = allCards[index];
                int cardCount  = 0;                
                do
                {
                    cardCount++;
                    index++;
                }while (index < allCards.Length && cardComparer.Equals(currentCard, allCards[index]));

                this.textWriter.Write("{0}({1}), ", currentCard.name, cardCount);
            }
            this.textWriter.WriteLine();
        }

        public void LogDeck(PlayerState playerState)
        {
            this.textWriter.Write("{0} Deck (Player knows {1} cards):", playerState.actions.PlayerName, playerState.deck.KnownCards.Count());            
            foreach (Card card in playerState.deck)
            {
                this.textWriter.Write("{0}, ", card.name);
            }
            this.textWriter.WriteLine();
        }
    }
}
