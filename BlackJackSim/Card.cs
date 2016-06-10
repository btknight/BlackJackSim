using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    public class Card
    {
        /// <summary>
        /// Suit of the card
        /// </summary>
        public Suit Suit { get; private set; }
        
        /// <summary>
        /// Face of the card
        /// </summary>
        public Face Face { get; private set; }
        
        /// <summary>
        /// Blackjack value of the card.  K, Q, J are worth 10.
        /// </summary>
        public int Value
        {
            get
            {
                int retval = (int)Face;
                if (retval > 10) { retval = 10; }
                return retval;
            }
        }

        /// <summary>
        /// Creates a new card.
        /// </summary>
        /// <param name="suit">Suit of the card</param>
        /// <param name="face">Face of the card</param>
        public Card(Suit suit, Face face) { Suit = suit; Face = face; }

        /// <summary>
        /// Return a new object that is copy of this card.
        /// </summary>
        /// <returns>The new Card copy</returns>
        public Card Clone()
        {
            return new Card(Suit, Face);
        }

        public override string ToString()
        {
            return (Face.ToString() + " of " + Suit.ToString());
        }
    }

    /// <summary>
    /// Describes card suits.
    /// </summary>
    public enum Suit
    {
        Hearts,
        Diamonds,
        Spades,
        Clubs
    }

    /// <summary>
    /// Describes card face values.
    /// </summary>
    public enum Face
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }
}
