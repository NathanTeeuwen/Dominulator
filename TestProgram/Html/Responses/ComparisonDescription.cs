using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Dominion;
using Dominion.Strategy;

namespace Program.WebService
{        
    [Serializable]
    public class ComparisonDescription
    {
        public string player1 { get; set; }
        public string player2 { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals((ComparisonDescription)obj);
        }

        public bool Equals(ComparisonDescription other)
        {
 	         return this.player1 == other.player1 &&
                    this.player2 == other.player2;
        }

        public override int GetHashCode()
        {
 	        return this.player1.GetHashCode() ^ 
                   this.player2.GetHashCode();
        }

        public PlayerAction Player1Action
        {
            get
            {
                return GetPlayerActionFromCode(this.player1);
            }
        }

        public PlayerAction Player2Action
        {
            get
            {
                return GetPlayerActionFromCode(this.player2);
            }
        }

        private PlayerAction GetPlayerActionFromCode(string code)
        {
            return Program.strategyLoader.GetPlayerActionFromCode(code);            
        }
    }
    
}
