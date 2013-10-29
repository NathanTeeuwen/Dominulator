using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class GainSequenceGameLog
        : EmptyGameLog, IDisposable
    {
        IndentedTextWriter textWriter;
        List<Card>[] gainSequenceByPlayer;

        public GainSequenceGameLog(IndentedTextWriter textWriter = null)
        {
            this.textWriter = textWriter;
        }

        public override void PlayerBoughtCard(PlayerState playerState, Card card)
        {            
            List<Card> gainedList = this.gainSequenceByPlayer[playerState.PlayerIndex];
            gainedList.Add(card);
        }

        public override void PlayerGainedCard(PlayerState playerState, Card card)
        {            
            if (this.gainSequenceByPlayer != null)
            {
                List<Card> gainedList = this.gainSequenceByPlayer[playerState.PlayerIndex];
                gainedList.Add(card);
            }
        }

        public override void StartGame(GameState gameState)
        {
            this.gainSequenceByPlayer = new List<Card>[gameState.players.PlayerCount];
            for (int i = 0; i < gameState.players.PlayerCount; ++i)
            {
                this.gainSequenceByPlayer[i] = new List<Card>();
            }
        }

        public override void EndGame(GameState gameState)
        {
            if (this.textWriter != null)
            {
                Write(this.textWriter, gameState.players.OriginalPlayerOrder);
            }
        }

        public void Write(IndentedTextWriter textWriter, PlayerState[] players)
        {
            foreach (PlayerState player in players)
            {
                textWriter.WriteLine("{0} GainSequence was:", player.actions.PlayerName);
                textWriter.Indent();
                int count = 0;
                foreach (Card card in this.gainSequenceByPlayer[player.PlayerIndex])
                {
                    textWriter.Write("{0}, ", card.name);
                    if (++count == 5)
                    {
                        count = 0;
                        textWriter.WriteLine();
                    };
                }
                textWriter.Unindent();
                textWriter.WriteLine();
                textWriter.WriteLine();
            }
        }       
    }
}
