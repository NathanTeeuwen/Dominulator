using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public enum Edition
    {
        First,
        Second
    }

    public static class EditionExtensionMethods
    {
        public static string EditionToString(this Dominion.Edition edition)
        {
            switch (edition)
            {
                case Dominion.Edition.First: return "First";
                case Dominion.Edition.Second: return "Second";
                default: throw new NotImplementedException();
            }
        }
    }

}
