using System;
using System.Collections.Generic;
using System.Text;

namespace BlackJackSim
{
    /// <summary>
    /// Emulates a dealer deck or a dealer shoe.
    /// </summary>
    public class Deck
    {
        private static Random random = new Random();
        private Queue<Card> _deck;

        /// <summary>
        /// A count of cards left in the shoe.
        /// </summary>
        public int CardsLeft { get { return _deck.Count; } }

        /// <summary>
        /// Create new deck.
        /// </summary>
        /// <param name="NumDecks">Number of 52-card decks the shoe should hold</param>
        public Deck(int NumDecks)
        {
            _deck = new Queue<Card>(NumDecks * Enum.GetValues(typeof(Face)).Length * Enum.GetValues(typeof(Suit)).Length);
        }

        /// <summary>
        /// Fills the shoe with new cards.
        /// </summary>
        /// <param name="NumDecks">Number of 52-cards to use to fill the shoe</param>
        public void FillWithNewCards(int NumDecks)
        {
            // Break the wrapper and smell the freshly-made plastic.
            for (int i = 0; i < NumDecks; i++)
            {
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    foreach (Face kind in Enum.GetValues(typeof(Face)))
                    {
                        _deck.Enqueue(new Card(suit, kind));
                    }
                }
            }
        }

        /// <summary>
        /// Draw a card from the shoe.
        /// </summary>
        /// <returns>Card that was drawn</returns>
        public Card DrawCard()
        {
            return _deck.Dequeue();
        }

        /// <summary>
        /// Add a card to the shoe.
        /// </summary>
        /// <param name="card">Card to add</param>
        public void AddCard(Card card)
        {
            _deck.Enqueue(card);
        }

        /// <summary>
        /// Return multiple cards to the shoe.
        /// </summary>
        /// <param name="cards">Group of cards to return</param>
        public void ReturnCards(IEnumerable<Card> cards)
        {
            foreach (Card card in cards) { _deck.Enqueue(card); }
        }

        /// <summary>
        /// Shuffle the deck once.
        /// </summary>
        public void Shuffle() { Shuffle(1); }
        
        /// <summary>
        /// Shuffle the deck multiple times.
        /// </summary>
        /// <param name="Times">Number of times to shuffle</param>
        public void Shuffle(int Times)
        {
            Card[] Pile = _deck.ToArray();
            _deck.Clear();
            for (int i = 0; i < Times; i++)
            {
                int n = Pile.Length;
                while (n > 1)
                {
                    n--;
                    int k = random.Next(n + 1);
                    Card Tmp = Pile[k];
                    Pile[k] = Pile[n];
                    Pile[n] = Tmp;
                }
            }
            _deck = new Queue<Card>(Pile);
        }
    }
}
