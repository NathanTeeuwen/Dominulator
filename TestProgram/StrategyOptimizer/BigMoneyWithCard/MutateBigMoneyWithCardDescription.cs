using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program.GeneticAlgorithm;

namespace Program
{
    class MutateBigMoneyWithCardDescription
           : ISpecidesMutator<BigMoneyWithCardDescription>    
    {        
        private readonly Random random;        

        public MutateBigMoneyWithCardDescription(Random random)
        {
            this.random = random;            
        }    

        public BigMoneyWithCardDescription Mutate(BigMoneyWithCardDescription descr)
        {
            BigMoneyWithCardDescription result = null;

            bool applied = false;
            while (!applied)
            {
                result = descr.Clone();
                int indexToModify = this.random.Next(descr.parameters.Length);
                int op = this.random.Next(2);
                if (op == 0)
                {
                    result.parameters[indexToModify].Value++;
                }
                else
                {
                    result.parameters[indexToModify].Value--;
                }
                
                applied = result.parameters[indexToModify].IsInRange();
            }

            return result;
        }
    }
}
