using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    class PlayerTurnCounters
    {
        private int availableActionCount;
        private int availableBuys;
        private int buysUsed;
        private int availableCoins;
        private int availablePotions;
        private int availableCoinTokens;
        internal SetOfCards cardsBannedFromPurchase;
        internal int copperAdditionalValue = 0;

        internal PlayerTurnCounters(CardGameSubset gameSubset)
        {
            cardsBannedFromPurchase = new SetOfCards(gameSubset);
        }

        internal void InitializeTurn()
        {
            this.availableActionCount = 1;
            this.availableBuys = 1;
            this.buysUsed = 0;
            this.availableCoins = 0;
            this.availablePotions = 0;
            this.copperAdditionalValue = 0;
            this.cardsBannedFromPurchase.Clear();
        }

        internal int AvailableActions
        {
            get
            {
                return this.availableActionCount;
            }

        }

        internal int AvailableCoins
        {
            get
            {
                return this.availableCoins;
            }
        }

        internal int AvailablePotions
        {
            get
            {
                return this.availablePotions;
            }
        }

        internal int AvailableBuys
        {
            get
            {
                return this.availableBuys;
            }
        }

        internal int AvailableCoinTokens
        {
            get
            {
                return this.availableCoinTokens;
            }
        }

        internal int BuysUsed
        {
            get
            {
                return this.buysUsed;
            }
        }

        public void AddBuys(PlayerState playerState, int count)
        {
            if (count > 0)
            {
                this.availableBuys += count;
                playerState.gameLog.PlayerGainedBuys(playerState, count);
            }
        }

        public void AddActions(PlayerState playerState, int count)
        {
            if (count > 0)
            {
                this.availableActionCount += count;
                playerState.gameLog.PlayerGainedActions(playerState, count);
            }
        }

        public void RemoveBuy()
        {
            this.availableBuys--;
            this.buysUsed++;
        }

        public void RemoveAction()
        {
            this.availableActionCount--;
        }

        public void AddPotions(PlayerState playerState, int count)        
        {
            if (count != 0)
            {
                this.availablePotions += count;
                if (availablePotions < 0)
                {
                    availablePotions = 0;
                }
                playerState.gameLog.PlayerGainedPotion(playerState, count);
            }
        }

        public void AddCoins(PlayerState playerState, int count)
        {
            if (count != 0)
            {
                this.availableCoins += count;
                if (availableCoins < 0)
                {
                    availableCoins = 0;
                }
                playerState.gameLog.PlayerGainedCoin(playerState, count);
            }
        }

        public void AddCoinTokens(PlayerState playerState, int count)
        {
            if (count != 0)
            {
                this.availableCoinTokens += count;
                if (availableCoinTokens < 0)
                {
                    availableCoinTokens = 0;
                }
                playerState.gameLog.PlayerGainedCoinToken(playerState, count);
            }
        }

        internal void RemoveCoinTokens(int count)
        {
            this.availableCoinTokens -= count;
            if (this.availableCoinTokens < 0)
            {
                this.availableCoinTokens = 0;
            }
        }

        internal void RemoveCoins(int count)
        {
            this.availableCoins -= count;
            if (this.availableCoins < 0)
            {
                this.availableCoins = 0;
            }
        }

        internal void RemovePotions(int count)
        {
            this.availablePotions -= count;
            if (this.availablePotions < 0)
            {
                this.availablePotions = 0;
            }
        }
    }
}