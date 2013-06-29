using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Ambassador :
        Card
    {
        public Ambassador()
            : base("Ambassador", coinCost: 3, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            //TODO
            throw new NotImplementedException();
        }
    }

    public class Bazaar : Card { public Bazaar() : base("Bazaar", coinCost: 5, isAction: true, plusCoins: 1, plusCards: 1, plusActions: 2) { } }

    public class Caravan :
        Card
    {
        public Caravan()
            : base("Caravan", coinCost: 4, isAction: true, isDuration: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.DrawAdditionalCardsIntoHand(1);
        }
    }

    public class Cutpurse :
       Card
    {
        public Cutpurse()
            : base("Cutpurse", coinCost: 4, isAction: true, plusCoins: 2, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            if (!otherPlayer.RequestPlayerDiscardCardFromHand(gameState, card => card.Is<Copper>(), isOptional: false))
            {
                otherPlayer.RevealHand();
            }
        }
    }

    public class Embargo :
        Card
    {
        public Embargo()
            : base("Embargo", coinCost: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.MoveCardFromPlayToTrash(gameState);
            PileOfCards cardPile = currentPlayer.RequestPlayerChooseCardPileFromSupply(gameState);
            gameState.AddEmbargoTokenToPile(cardPile);
        }
    }

    public class Explorer :
       Card
    {
        public Explorer()
            : base("Explorer", coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.RequestPlayerRevealCardFromHand(card => card.Is<Province>(), gameState) != null)
            {
                currentPlayer.GainCardFromSupply<Gold>(gameState);
            }
            else
            {
                currentPlayer.GainCardFromSupply<Silver>(gameState);
            }
        }
    }

    public class FishingVillage :
        Card
    {
        public FishingVillage()
            : base("Fishing Village", coinCost: 3, isAction: true, isDuration: true, plusCoins: 1, plusActions: 2)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddCoins(1);
            currentPlayer.AddActions(1);
        }
    }

    public class GhostShip :
        Card
    {
        public GhostShip()
            : base("Ghost Ship", coinCost: 5, isAction: true, isAttack: true, plusCards: 2)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            while (currentPlayer.Hand.Count() > 3)
            {
                currentPlayer.RequestPlayerTopDeckCardFromHand(gameState, acceptableCard => true, isOptional: false);
            }
        }
    }

    public class Haven :
        Card
    {
        public Haven()
            : base("Haven", coinCost: 2, isAction: true, isDuration: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerDeferCardFromHandtoNextTurn(gameState);
        }
    }

    public class Island :
        Card
    {
        public Island()
            : base("Island", coinCost: 4, isAction: true, victoryPoints: playerState => 2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
        }
    }

    public class Lighthouse :
        Card
    {
        public Lighthouse()
            : base("Lighthouse", coinCost: 2, isAction: true, isDuration: true, plusCoins: 1, plusActions: 1)
        {
        }

        public override bool DoReactionToAttackWhileInPlay(PlayerState currentPlayer, GameState gameState)
        {
            return false;
        }
    }

    public class Lookout :
        Card
    {
        public Lookout()
            : base("Lookout", coinCost: 3, isAction: true, plusActions: 1)
        {

        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealCardsFromDeck(3);
            gameState.gameLog.PushScope();
            currentPlayer.RequestPlayerTrashRevealedCard(gameState);
            currentPlayer.RequestPlayerDiscardRevealedCard(gameState);
            currentPlayer.MoveRevealedCardToTopOfDeck();
            gameState.gameLog.PopScope();
        }
    }

    public class MerchantShip :
        Card
    {
        public MerchantShip()
            : base("MerchantShip", coinCost: 5, isAction: true, isDuration: true, plusCoins: 2)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddCoins(2);
        }
    }

    public class NativeVillage :
        Card
    {
        public NativeVillage()
            : base("Native Village", coinCost: 2, isAction: true, plusActions: 2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
        }
    }

    public class Navigator :
        Card
    {
        public Navigator()
            : base("Navigator", coinCost: 4, isAction: true, plusCoins: 2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
        }
    }

    public class Outpost :
        Card
    {
        public Outpost()
            : base("Outpost", coinCost: 5, isAction: true, isDuration: true)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
        }
    }

    public class PearlDiver :
        Card
    {
        public PearlDiver()
            : base("Pearl Diver", coinCost: 2, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
        }
    }

    public class PirateShip :
        Card
    {
        public PirateShip()
            : base("Pirate Ship", coinCost: 4, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
        }
    }

    public class Salvager :
        Card
    {
        public Salvager()
            : base("Salvager", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card card = currentPlayer.RequestPlayerTrashCardFromHand(gameState, acceptableCard => true, isOptional: false);
            if (card != null)
            {
                currentPlayer.AddCoins(card.CurrentCoinCost(currentPlayer));
            }
        }
    }

    public class SeaHag :
        Card
    {
        public SeaHag()
            : base("Sea Hag", coinCost: 4, isAction: true, isAttack: true)
        {
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            otherPlayer.DiscardCardFromTopOfDeck();
            otherPlayer.GainCardFromSupply(gameState, typeof(Curse), DeckPlacement.TopOfDeck);
        }
    }

    public class Smugglers :
        Card
    {
        public Smugglers()
            : base("Smugglers", coinCost: 3, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
        }
    }   

    public class Tactician :
        Card
    {
        public Tactician()
            : base("Tactician", coinCost: 5, isAction: true, isDuration: true)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            // TODO
            throw new NotImplementedException();
            //currentPlayer.DiscardHand();
        }
    }

    public class TreasureMap :
        Card
    {
        public TreasureMap()
            : base("Treasure Map", coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.MoveCardFromPlayToTrash(gameState))
            {
                if (currentPlayer.TrashCardFromHandOfType(gameState, typeof(TreasureMap), guaranteeInHand: false) != null)
                {
                    currentPlayer.GainCardsFromSupply<Gold>(gameState, 4, DeckPlacement.TopOfDeck);
                }
            }
        }
    }

    public class Treasury :
        Card
    {
        public Treasury()
            : base("Treasury", coinCost: 5, isAction: true, plusActions: 1, plusCards: 1, plusCoins: 1)
        {
        }

        public override void DoSpecializedCleanupAtStartOfCleanup(PlayerState currentPlayer, GameState gameState)
        {
            //todo
            throw new NotImplementedException();
        }
    }


    public class Warehouse :
        Card
    {
        public Warehouse()
            : base("Warehouse", coinCost: 3, isAction: true, plusActions: 1, plusCards: 3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, 3, isOptional: false);
        }
    }

    public class Wharf :
        Card
    {
        public Wharf()
            : base("Wharf", coinCost: 5, isAction: true, isDuration: true, plusCards: 2, plusBuy: 1)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.DrawAdditionalCardsIntoHand(2);
            currentPlayer.AddBuys(1);
        }
    }
}