using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    public class PlayTrack
    {
        /// <summary>
        /// Number of splits done by this player.
        /// </summary>
        public int Splits { get; set; }
        /// <summary>
        /// Number of times this player doubled down.
        /// </summary>
        public int Doubles { get; set; }
        /// <summary>
        /// Number of times this player hit a hand.
        /// </summary>
        public int Hits { get; set; }
        /// <summary>
        /// Number of times this player stood pat on a hand.
        /// </summary>
        public int Stands { get; set; }
        /// <summary>
        /// Number of times this player busted a hand.
        /// </summary>
        public int Busted { get; set; }

        public PlayTrack()
        {
            Splits = Doubles = Hits = Stands = Busted = 0;
        }

    }
}
