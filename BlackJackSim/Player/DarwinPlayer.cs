using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    /// <summary>
    /// This class implements play rules laid out in the 1986 book 
    /// "Darwin Ortiz on Casino Gambling", pp 63-64.
    /// </summary>
    public class DarwinPlayer : BasicStrategyPlayer
    {
        /// <summary>
        /// Static constructor used to initialize the decision tables.
        /// </summary>
        static DarwinPlayer()
        {
            ShouldSplit = new bool[,] {
                { true,true,true,true,true,true,true,true,true,true },
                { false,true,true,true,true,true,true,false,false,false },
                { false,true,true,true,true,true,true,false,false,false },
                { false,false,false,false,true,true,false,false,false,false },
                { false,false,false,false,false,false,false,false,false,false },
                { false,true,true,true,true,true,true,false,false,false },
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
        
        public DarwinPlayer(ChipStack startingPurse) : base(startingPurse) { }
    }
}
