using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class BlackMarket
       : Card
    {
        public static BlackMarket card = new BlackMarket();

        private BlackMarket()
            : base("Black Market", Expansion.Promo, coinCost: 3, plusCoins: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            gameState.DoPlayTreasures(currentPlayer);
            PileOfCards pile = gameState.GetSpecialPile(typeof(BlackMarket));            
            throw new NotImplementedException();
        }
    }

    public class Envoy
       : Card
    {
        public static Envoy card = new Envoy();

        private Envoy()
            : base("Envoy", Expansion.Promo, coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(5, gameState);
            Card cardType = gameState.players.PlayerLeft.actions.BanCardToDrawnIntoHandFromRevealedCards(gameState);
            if (!currentPlayer.cardsBeingRevealed.HasCard(cardType))
            {
                throw new Exception("Must ban a card currently being revealed");
            }
            currentPlayer.MoveRevealedCardToDiscard(cardType, gameState);
            currentPlayer.MoveAllRevealedCardsToHand();
        }
    }

    public class Governor
       : Card
    {
        public static Governor card = new Governor();

        private Governor()
            : base("Governor", Expansion.Promo, coinCost: 5, isAction: true, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            PlayerActionChoice playerChoice = currentPlayer.RequestPlayerChooseBetween(gameState, acceptableChoice =>
                acceptableChoice == PlayerActionChoice.GainCard ||
                acceptableChoice == PlayerActionChoice.PlusCard ||
                acceptableChoice == PlayerActionChoice.Trash);

            switch (playerChoice)
            {
                case PlayerActionChoice.PlusCard:
                    {
                        currentPlayer.DrawAdditionalCardsIntoHand(3, gameState);
                        foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
                        {
                            otherPlayer.DrawAdditionalCardsIntoHand(1, gameState);
                        }
                        break;
                    }
                case PlayerActionChoice.GainCard:
                    {
                        currentPlayer.GainCardFromSupply(gameState, Gold.card);
                        foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
                        {
                            otherPlayer.GainCardFromSupply(gameState, Silver.card);
                        }
                        break;
                    }
                case PlayerActionChoice.Trash:
                    {
                        currentPlayer.RequestPlayerTrashCardFromHandAndGainCard(
                            gameState,
                            acceptableCardsToTrash => true,
                            CostConstraint.Exactly,
                            2,
                            CardRelativeCost.RelativeCost,
                            isOptionalToTrash: true,
                            isOptionalToGain: false);

                        foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
                        {
                            otherPlayer.RequestPlayerTrashCardFromHandAndGainCard(
                                gameState,
                                acceptableCardsToTrash => true,
                                CostConstraint.Exactly,
                                1,
                                CardRelativeCost.RelativeCost,
                                isOptionalToTrash: true,
                                isOptionalToGain: false);
                        }
                        break;
                    }
            }
        }
    }

    public class Prince
       : Card
    {
        public static Prince card = new Prince();

        private Prince()
            : base("Prince", Expansion.Promo, coinCost: 8, isAction:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Sauna
       : Card
    {
        public static Sauna card = new Sauna();

        private Sauna()
            : base("Sauna", Expansion.Promo, coinCost: 4, isAction:true, plusCards:1, plusActions:1, isKingdomCard:false)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class SaunaAvanto
       : Card
    {
        public static SaunaAvanto card = new SaunaAvanto();

        private SaunaAvanto()
            : base("Sauna/Avanto", Expansion.Promo, coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Avanto
       : Card
    {
        public static Avanto card = new Avanto();

        private Avanto()
            : base("Avanto", Expansion.Promo, coinCost: 5, isAction: true, plusCards: 3, isKingdomCard: false)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Dismantle
       : Card
    {
        public static Dismantle card = new Dismantle();

        private Dismantle()
            : base("Dismantle", Expansion.Promo, coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Summon
       : Event
    {
        public static Summon card = new Summon();

        private Summon()
            : base("Summon", Expansion.Promo, coinCost: 4)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Stash
       : Card
    {
        public static Stash card = new Stash();

        private Stash()
            : base("Stash", Expansion.Promo, coinCost: 5, plusCoins:2, isTreasure: true, pluralName:"Stashes")
        {
        }
   
        public static int[] GetStashPlacementBeginningOfDeck(int stashCount)
        {
            var result = new int[stashCount];
            for(int i = 0; i < stashCount; ++i)
            {
                result[i] = i;
            }
            return result;
        }

        public static void VerifyStashPlacementInDeck(int[] placements, int deckCount)
        {
            System.Array.Sort(placements);
            for (int i = 0; i < placements.Length; ++i)
            {
                if (placements[i] >= deckCount)
                {
                    throw new Exception("Placement is out of deck range");
                }

                if (i > 0 && placements[i] == placements[i - 1] )
                    throw new Exception("Two stashes have chosen to be in the same place");                
            }            
        }
    }

    public class WalledVillage
       : Card
    {
        public static WalledVillage card = new WalledVillage();

        private WalledVillage()
            : base("Walled Village", Expansion.Promo, coinCost: 4, plusCards: 1, plusActions: 2, isAction: true)
        {
            this.doSpecializedCleanupAtStartOfCleanup = DoSpecializedCleanupAtStartOfCleanup;
        }

        private new void DoSpecializedCleanupAtStartOfCleanup(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.cardsPlayed.CountWhere(card => card.isAction == true) <= 2)
            {
                currentPlayer.RequestPlayerTopDeckCardFromCleanup(this, gameState);                
            }
        }
    }
}