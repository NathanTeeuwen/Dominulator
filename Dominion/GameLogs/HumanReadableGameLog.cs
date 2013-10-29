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
        bool is2Player;
        int roundNumber = 0;
        List<Card> playedTreasures;
        List<Card> boughtCards;
        List<Card> discardedCards;
        List<Card> drawnCards;
        IndentedTextWriter textWriter;        

        public HumanReadableGameLog(string filename)
            : this(new IndentedTextWriter(filename))
        {                     
        }

        public HumanReadableGameLog(IndentedTextWriter textWriter)
        {
            this.textWriter = textWriter;
            this.playedTreasures = new List<Card>();
            this.boughtCards = new List<Card>();
            this.discardedCards = new List<Card>();
            this.drawnCards = new List<Card>();
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

        public void BeginRound(PlayerState playerState)
        {
            ++this.roundNumber;            
        }

        public void BeginPhase(PlayerState playerState)
        {
        }

        public void EndPhase(PlayerState playerState)
        {
            if (playerState.PlayPhase == PlayPhase.PlayTreasure)
            {                
                this.textWriter.Write("Played ");
                WriteAllCards(this.playedTreasures);
                this.playedTreasures.Clear();
                this.PushScope();
                this.textWriter.WriteLine(" ... and has {0} coins and {1} buys available.", playerState.AvailableCoins, playerState.AvailableBuys);
                this.PopScope();
            }
            else if (playerState.PlayPhase == PlayPhase.Buy)
            {
                this.textWriter.Write("Bought ");                
                WriteAllCards(this.boughtCards);
                this.boughtCards.Clear();                
            }
            else if (playerState.PlayPhase == PlayPhase.Cleanup)
            {
                this.textWriter.Write("Discarded ");
                WriteAllCards(this.discardedCards);
                this.discardedCards.Clear();
            }
            else if (playerState.PlayPhase == PlayPhase.DrawCards)
            {
                WriteOutDrawnCardsIfNecessary();
            }
        }

        private void WriteOutPlayedTreasuresIfNecessary(bool unindent = false)
        {
            if (this.playedTreasures.Count > 0)
            {
                if (unindent)
                    this.PopScope();

                this.textWriter.Write("Played ");
                WriteAllCards(this.playedTreasures);
                if (unindent)
                    this.PushScope();
                this.playedTreasures.Clear();
            }
        }

        private void WriteOutDrawnCardsIfNecessary()
        {
            if (this.drawnCards.Count > 0)
            {
                this.textWriter.Write("Draws into hand ");
                WriteAllCards(this.drawnCards);
                this.drawnCards.Clear();
            }
        }


        private string GetPlayerName(PlayerState playerState)
        {
            if (playerState.PlayPhase == PlayPhase.NotMyTurn)
            {
                return this.is2Player ? "The other player" : playerState.actions.PlayerName;
            }
            return "... and";
        }
       
        public void EndRound(GameState gameState)
        {            
        }

        public void BeginTurn(PlayerState playerState)
        {           
            this.textWriter.WriteLine("== {0}'s turn {1} ===", playerState.actions.PlayerName, this.roundNumber);
            this.textWriter.Write("With hand: ");
            this.WriteAllCards(playerState.hand);                        
        }

        public void EndTurn(PlayerState playerState)
        {
            this.textWriter.Write("Ends turn owning: ", playerState.actions.PlayerName);            
            this.WriteAllOwnedCards(playerState);            
            this.textWriter.WriteLine();            
        }

        public void PlayerBoughtCard(PlayerState playerState, Card card)
        {
            this.boughtCards.Add(card);
        }        

        public void DrewCardIntoHand(PlayerState playerState, Card card)
        {
            if (this.roundNumber > 0)
            {
                if (playerState.PlayPhase == PlayPhase.DrawCards)
                {
                    this.drawnCards.Add(card);
                }
                else
                {
                    this.textWriter.WriteLine("{0} Drew {1} into hand.", GetPlayerName(playerState), card.name);                    
                }
            }
        }      

        public void PlayerGainedCard(PlayerState playerState, Card card)
        {
            if (this.roundNumber > 0)
            {
                WriteOutPlayedTreasuresIfNecessary(unindent:true);
                this.textWriter.WriteLine("{0} gains a {1}.", GetPlayerName(playerState), card.name);
            }
        }

        public void PlayerNamedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} named {1}.", GetPlayerName(playerState), card.name);
        }

        public void PlayerTrashedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} trashes {1}.", GetPlayerName(playerState), card.name);            
        }

        public void PlayerPutCardInHand(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} put {1} into his hand.", GetPlayerName(playerState), card.name);            
        }

        public void PlayerTopDeckedCard(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Placed {1} on top of his deck.", GetPlayerName(playerState), card.name);
        }

        public void ReceivedDurationEffectFrom(PlayerState playerState, Card card)
        {
            this.textWriter.WriteLine("{0} Finished Playing {1}.", GetPlayerName(playerState), card.name);
        }

        public void PlayedCard(PlayerState playerState, Card card)
        {
            if (playerState.PlayPhase == PlayPhase.PlayTreasure)
            {
                this.playedTreasures.Add(card);
            }
            else
            {
                this.textWriter.WriteLine("Played {0}.", card.name);
            }
        }

        public void DiscardedCard(PlayerState playerState, Card card)
        {
            if (playerState.PlayPhase == PlayPhase.Cleanup)
            {
                this.discardedCards.Add(card);
            }
            else
            {
                this.textWriter.WriteLine("{0} Discards {1}.", GetPlayerName(playerState), card.name);
            }
        }

        public void PlayerDiscardCard(PlayerState playerState, Card card)
        {
            if (playerState.PlayPhase == PlayPhase.Cleanup)
            {
                this.discardedCards.Add(card);
            }
            else
            {
                this.textWriter.WriteLine("{0} Discards {1}.", GetPlayerName(playerState), card.name);
            }
        }

        public void PlayerRevealedCard(PlayerState playerState, Card card, DeckPlacement source)
        {
            this.textWriter.WriteLine("{0} Reveals {1}.", GetPlayerName(playerState), card.name);
        }

        public void ReshuffledDiscardIntoDeck(PlayerState playerState)
        {
            if (this.roundNumber > 0)
            {
                if (playerState.PlayPhase != PlayPhase.NotMyTurn)
                {
                    WriteOutDrawnCardsIfNecessary();
                    
                }
                this.textWriter.WriteLine("{0} reshuffles", GetPlayerName(playerState));                
            }
        }

        public void StartGame(GameState gameState)
        {
            this.is2Player = gameState.players.PlayerCount == 2;
        }

        public void EndGame(GameState gameState)
        {
            this.textWriter.WriteLine("Game ended in {0} turns.", this.roundNumber);
            this.textWriter.WriteLine();

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
            this.textWriter.WriteLine();

            foreach (PlayerState player in gameState.players.AllPlayers)
            {
                this.textWriter.WriteLine("{0} Total Score is: {1}", player.actions.PlayerName, player.TotalScore());
                this.PushScope();
                this.WriteAllOwnedCards(player);
                this.PopScope();
                this.textWriter.WriteLine();           
            }

            this.textWriter.Write("Trash contains: ");
            this.WriteAllCards(gameState.trash);

            this.textWriter.WriteLine();           
        }

        public void PlayerGainedCoin(PlayerState playerState, int coinAmount)        
        {
            if (playerState.PlayPhase != PlayPhase.PlayTreasure)
            {
                var sign = coinAmount > 0 ? "+" : "";
                this.textWriter.WriteLine("{2}{0} Coin = {1} all together.", coinAmount, playerState.AvailableCoins, sign);
            }
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
            if (playerState.PlayPhase != PlayPhase.PlayTreasure)
            {
                this.textWriter.WriteLine("+{0} Buys = {1} all together.", buyAmount, playerState.AvailableBuys);
            }
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

        private void WriteAllOwnedCards(PlayerState playerState)
        {
            WriteAllCards(playerState.AllOwnedCards);
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
            this.textWriter.WriteLine("Player overpayed by {0} for {1}", overPayAmount, boughtCard.name);
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

        private void WriteAllCards(BagOfCards cards)
        {
            WriteAllCards((CollectionCards)cards);
        }

        private void WriteAllCards(CollectionCards cards)
        {
            bool needComma = false;
            foreach (Card currentCard in cards.AllTypes.OrderBy(card => card.name))
            {
                int cardCount = cards.CountOf(currentCard);
                if (!needComma)
                {
                    needComma = true;
                }
                else
                {
                    this.textWriter.Write(", ");
                }
                this.textWriter.Write("{0} {1}", cardCount, cardCount > 1 ? currentCard.pluralName : currentCard.name);
            }
            this.textWriter.WriteLine();
        }

        private void WriteAllCards(IEnumerable<Card> cards)
        {
            if (!cards.Any())
            {
                this.textWriter.WriteLine("nothing");
                return;
            }

            bool needComma = false;
            int cardCount = 0;
            Card lastCardType = null;
            foreach (Card currentCard in cards)
            {                
                if (currentCard != lastCardType)
                {
                    if (cardCount > 0)
                    {
                        if (!needComma)
                        {
                            needComma = true;
                        }
                        else
                        {
                            this.textWriter.Write(", ");
                        }
                        this.textWriter.Write("{0} {1}", cardCount, cardCount > 1 ? lastCardType.pluralName : lastCardType.name);
                        cardCount = 0;
                    }
                    lastCardType = currentCard;
                }                                
                cardCount++;                
            }

            if (cardCount > 0)
            {
                if (!needComma)
                {
                    needComma = true;
                }
                else
                {
                    this.textWriter.Write(", ");
                }
                this.textWriter.Write("{0} {1}", cardCount, cardCount > 1 ? lastCardType.pluralName : lastCardType.name);
                cardCount = 0;
            }

            this.textWriter.WriteLine();
        }
    }
}
