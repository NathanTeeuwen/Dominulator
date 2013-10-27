using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    class MutateBigMoneyWithCardDescription
           : ISpecidesMutator<BigMoneyWithCardDescription>    
    {
        private delegate bool ApplyMutation(BigMoneyWithCardDescription descripton);
        private readonly Random random;
        private readonly ApplyMutation[] mutators;

        public MutateBigMoneyWithCardDescription(Random random)
        {
            this.random = random;
            mutators = CreateMutators();
        }

        static ApplyMutation[] CreateMutators()
        {
            return new ApplyMutation[]
                {
                    delegate(BigMoneyWithCardDescription descr) { descr.cardCount++; return true;},
                    delegate(BigMoneyWithCardDescription descr) { descr.cardCount--; return descr.cardCount > 0;},
                    delegate(BigMoneyWithCardDescription descr) { descr.countGoldBeforeProvince++; return true;},
                    delegate(BigMoneyWithCardDescription descr) { descr.countGoldBeforeProvince--; return descr.countGoldBeforeProvince >= 0;},
                    delegate(BigMoneyWithCardDescription descr) { descr.countRemainingProvinceBeforeDuchy++; return true;},
                    delegate(BigMoneyWithCardDescription descr) { descr.countRemainingProvinceBeforeDuchy--; return descr.countRemainingProvinceBeforeDuchy >= 0;},
                    delegate(BigMoneyWithCardDescription descr) { descr.countRemainingProvinceBeforeEstateOverGold++; return true;},
                    delegate(BigMoneyWithCardDescription descr) { descr.countRemainingProvinceBeforeEstateOverGold--; return descr.countRemainingProvinceBeforeEstateOverGold >= 0;},
                    delegate(BigMoneyWithCardDescription descr) { descr.countRemainingProvinceBeforeEstateOverSilver++; return true;},
                    delegate(BigMoneyWithCardDescription descr) { descr.countRemainingProvinceBeforeEstateOverSilver--; return descr.countRemainingProvinceBeforeEstateOverSilver >= 0;},
                };
        }

        public BigMoneyWithCardDescription Mutate(BigMoneyWithCardDescription descr)
        {
            BigMoneyWithCardDescription result = null;

            bool applied = false;
            while (!applied)
            {
                result = descr.Clone();
                var mutator = this.mutators[this.random.Next(this.mutators.Length)];
                applied = mutator(result);
            }

            return result;
        }
    }
}
