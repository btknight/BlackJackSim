using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    /// <summary>
    /// This class implements play rules laid out at 
    /// http://en.wikipedia.org/wiki/Blackjack#Basic_strategy.
    /// Only one change from DarwinPlayer - this player will not split 6's vs. a 7 up-card.
    /// </summary>
    public class WikiPlayer : BasicStrategyPlayer
    {
        /// <summary>
        /// Static constructor used to initialize the decision tables.
        /// </summary>
        static WikiPlayer()
        {
            ShouldSplit = new bool[,] {
                { true,true,true,true,true,true,true,true,true,true },
                { false,true,true,true,true,true,true,false,false,false },
                { false,true,true,true,true,true,true,false,false,false },
                { false,false,false,false,true,true,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,true,true,true,true,true,false,false,false,false },
                { false,true,true,true,true,true,true,false,false,false },
                { true,true,true,true,true,true,true,true,true,true },
                { false,true,true,true,true,true,false,true,true,false },
                { false,false,false,false,false,false,false,false,false,false }
            };

            SoftDouble = new bool[,] {
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,true,true,true,true,false,false,false,false },
                { false,true,true,true,true,true,true,true,true,false },
                { false,true,true,true,true,true,true,true,true,true },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,true,true,false,false,false,false },
                { false,false,false,false,true,true,false,false,false,false },
                { false,false,false,true,true,true,false,false,false,false },
                { false,false,false,true,true,true,false,false,false,false },
                { false,false,true,true,true,true,false,false,false,false },
                { false,false,true,true,true,true,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false }
            };

            SoftStand = new bool[,] {
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,true,false,false,false,false,true,true,false,false },
                { true,true,true,true,true,true,true,true,true,true },
                { true,true,true,true,true,true,true,true,true,true }
            };

            HardDouble = new bool[,] {
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,true,true,true,true,false,false,false,false },
                { false,true,true,true,true,true,true,true,true,false },
                { false,true,true,true,true,true,true,true,true,true },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false }
            };

             HardStand = new bool[,] {
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,false,false,true,true,true,false,false,false,false },
                { false,true,true,true,true,true,false,false,false,false },
                { false,true,true,true,true,true,false,false,false,false },
                { false,true,true,true,true,true,false,false,false,false },
                { false,true,true,true,true,true,false,false,false,false },
                { true,true,true,true,true,true,true,true,true,true },
                { true,true,true,true,true,true,true,true,true,true },
                { true,true,true,true,true,true,true,true,true,true },
                { true,true,true,true,true,true,true,true,true,true },
                { true,true,true,true,true,true,true,true,true,true }
            };
        }
        
        public WikiPlayer(ChipStack startingPurse) : base(startingPurse) { }
    }
}
