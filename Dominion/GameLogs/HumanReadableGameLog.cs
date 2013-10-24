using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class HumanReadableGameLog
        : IGameLog, IDisposable
    {
        int roundNumber = 0;
        IndentedTextWriter textWriter;
        List<Card>[] gainSequenceByPlayer;

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

        public void EndRound(GameState gameState)
        {            
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

            List<Card> gainedList = this.gainSequenceByPlayer[playerState.PlayerIndex];
            gainedList.Add(card);
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

            if (this.gainSequenceByPlayer != null)
            {
                List<Card> gainedList = this.gainSequenceByPlayer[playerState.PlayerIndex];
                gainedList.Add(card);
            }
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

        public void StartGame(GameState gameState)
        {
            this.gainSequenceByPlayer = new List<Card>[gameState.players.PlayerCount];
            for (int i = 0; i < gameState.players.PlayerCount; ++i)
            {
                this.gainSequenceByPlayer[i] = new List<Card>();
            }
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

            this.textWriter.WriteLine();

            foreach (PlayerState player in gameState.players.AllPlayers)
            {
                this.textWriter.WriteLine("{0} GainSequence is:", player.actions.PlayerName);
                this.PushScope();
                int count = 0;
                foreach(Card card in this.gainSequenceByPlayer[player.PlayerIndex])
                {
                    this.textWriter.Write("{0}, ", card.name);
                    if (++count == 5)
                    {
                        count = 0;
                        this.textWriter.WriteLine();
                    };                     
                }
                this.PopScope();
                this.textWriter.WriteLine();
            }
        }

        public void PlayerGainedCoin(PlayerState playerState, int coinAmount)        
        {
            var sign = coinAmount > 0 ? "+" : "";
            this.textWriter.WriteLine("{2}{0} Coin = {1} all together.", coinAmount, playerState.AvailableCoins, sign);
        }

        public void PlayerGainedPotion(PlayerState playerState, int count)
        {
            var sign = count > 0 ? "+" : "";
            this.textWriter.WriteLine("{2}{0} Potions = {1} all together.", count, playerState.AvailablePotions, sign);
        }

        public void PlayerGainedActions(PlayerState playerState, int actionAmount)
        {
            this.textWriter.WriteLine("+{0} Actions = {1} all together.", actionAmount, playerState.AvailableActions);
        }

        public void PlayerGainedBuys(PlayerState playerState, int buyAmount)
        {
            this.textWriter.WriteLine("+{0} Buys = {1} all together.", buyAmount, playerState.AvailableBuys);
        }

        public void PlayerGainedCoinToken(PlayerState playerState, int coinAmount)
        {               
            if (coinAmount > 0)
            {
                this.textWriter.WriteLine("+{0} coin tokens = {1} all together.", coinAmount, playerState.AvailableCoinTokens);
            }
            else if (coinAmount < 0)
            {
                this.textWriter.WriteLine("{0} spent {1} coin tokens.  {2} remaining", playerState.actions.PlayerName, -coinAmount, playerState.AvailableCoinTokens);
            }
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

            for (int index = 0; index < allCards.Length; )
            {
                Card currentCard = allCards[index];
                int cardCount = 0;
                do
                {
                    cardCount++;
                    index++;
                } while (index < allCards.Length && cardComparer.Equals(currentCard, allCards[index]));

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

        public void PlayerReturnedCardToHand(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} returned {1} to hand", playerState.actions.PlayerName, card.name);
        }

        public void PlayerSetAsideCardFromHandForNextTurn(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} set aside {1} for next turn", playerState.actions.PlayerName, card.name);
        }    

        public void PlayerOverpaidForCard(Card boughtCard, int overPayAmount)
        {
            this.textWriter.WriteLine("Player overpayed by {2} for {2}", overPayAmount, boughtCard.name);
        }

        public void CardWentToLocation(DeckPlacement deckPlacement)
        {
            switch (deckPlacement)
            {
                case DeckPlacement.TopOfDeck: this.textWriter.WriteLine("... and placed card on top of deck"); break;
                case DeckPlacement.Hand: this.textWriter.WriteLine("... and placed card in hand"); break;                
            }
        }

        public void PlayerReturnedCardToPile(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} returned {1} to its pile", playerState.actions.PlayerName, card.name);
        }
    }
}
