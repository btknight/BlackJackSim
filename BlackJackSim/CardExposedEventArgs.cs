using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    /// <summary>
    /// Event notification class for when a card is exposed to other players.
    /// </summary>
    public class CardExposedEventArgs : EventArgs
    {
        /// <summary>
        /// Card that was exposed on the table.
        /// </summary>
        public Card Card { get; private set; }

        /// <summary>
        /// Builds the event arguments, so we can notify watchers.
        /// </summary>
        /// <param name="card">Card that was exposed</param>
        public CardExposedEventArgs(Card card)
        {
            this.Card = card;
        }
    }
}
