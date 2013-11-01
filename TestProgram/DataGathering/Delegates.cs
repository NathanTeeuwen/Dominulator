using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion;
using Dominion.Strategy;

namespace Program
{
    public delegate IGameLog CreateGameLog();
    public delegate IndentedTextWriter GetLogForGame(int gameIndex);

}
