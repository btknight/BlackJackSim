using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    /// <summary>
    /// A Darwin basic strategy player, with a positive progression betting system.
    /// </summary>
    public class PositiveProgDarwinPlayer : DarwinPlayer
    {
        private int WinCount { get; set; }
        private int CurrentBetUnit { get; set; }
        private int BetIncrement { get; set; }
        private int MaxBet { get; set; }

        public PositiveProgDarwinPlayer(ChipStack startingPurse)
            : base(startingPurse)
        {
        }

        public override void SeatPlayer(Table table)
        {
            base.SeatPlayer(table);
            BetIncrement = _Table.Rules.MinimumBet;
            CurrentBetUnit = 1;
            if (_Table.Rules.MaximumBet > 0)
            {
                MaxBet = _Table.Rules.MaximumBet;
            }
            else
            {
                MaxBet = BetIncrement * 5;
            }
        }

        protected override int DecideAnte()
        {
            if (WinCount < Score.Won)
            {
                WinCount = Score.Won;
                if (CurrentBetUnit * BetIncrement < MaxBet)
                {
                    CurrentBetUnit++;
                }
            }
            else
            {
                CurrentBetUnit = 1;
            }
            return CurrentBetUnit * BetIncrement;
        }
    }
}
