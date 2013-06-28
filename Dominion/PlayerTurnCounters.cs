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
        private int availableCoins;
        internal HashSet<Type> cardsBannedFromPurchase = new HashSet<Type>();
        internal int copperAdditionalValue = 0;

        internal void InitializeTurn()
        {
            this.availableActionCount = 1;
            this.availableBuys = 1;
            this.availableCoins = 0;
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

        internal int AvailableBuys
        {
            get
            {
                return this.availableBuys;
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
        }

        public void RemoveAction()
        {
            this.availableActionCount--;
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

        internal void RemoveCoins(int count)
        {
            availableCoins -= count;
            if (availableCoins < 0)
            {
                availableCoins = 0;
            }
        }
    }
}