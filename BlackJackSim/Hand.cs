using System;
using System.Collections.Generic;
using System.Text;

namespace BlackJackSim
{
    /// <summary>
    /// Emulates a blackjack hand.
    /// </summary>
    public class Hand
    {
        private List<Card> _cards { get; set; }

        /// <summary>
        /// List of cards.
        /// </summary>
        public Card[] Cards
        {
            get
            {
                return _cards.ToArray();
            }
        }

        /// <summary>
        /// Property describing whether the player has already been doubled down on this hand.
        /// If this is true, the hand cannot be hit again.
        /// </summary>
        public bool DoubledDown { get; private set; }

        /// <summary>
        /// Set to true if this is a soft hand (there is an ace and its value is 11).
        /// </summary>
        public bool IsSoft { get; private set; }

        /// <summary>
        /// Point value of the hand.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Original point value of the hand, when only two cards existed in the hand.  Used to tally score
        /// after play is finished, but point value of the hand has changed.
        /// </summary>
        public int ValuePrePlay { get; private set; }

        /// <summary>
        /// Original hard/soft state of the hand, when only two cards existed in the hand.  Used to tally score
        /// after play is finished, but point value of the hand has changed.
        /// </summary>
        public bool IsSoftPrePlay { get; private set; }

        public ChipStack Bet { get; private set; }

        /// <summary>
        /// Set to true if there are two cards in the hand, and they share equal value.
        /// </summary>
        public bool IsSplittable
        {
            get
            {
                if (_cards.Count == 2 && _cards[0].Value == _cards[1].Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Creates an empty hand.
        /// </summary>
        public Hand(ChipStack Ante)
        {
            Bet = Ante;
            _cards = new List<Card>();
            DoubledDown = false;
            ComputeValue();
        }

        /// <summary>
        /// Creates a new hand with a set of cards.
        /// </summary>
        /// <param name="cards">Cards to add immediately to the hand</param>
        public Hand(ChipStack Ante, IEnumerable<Card> cards)
        {
            Bet = Ante;
            _cards = new List<Card>(cards);
            DoubledDown = false;
            ComputeValue();
        }

        /// <summary>
        /// Add a card to the hand.
        /// </summary>
        /// <param name="card">Card to add</param>
        public void Hit(Card card)
        {
            if (Value > 21)
            {
                throw new Exception("Cannot hit a busted hand.");
            }
            if (DoubledDown)
            {
                throw new Exception("Cannot hit or double down a hand already doubled.");
            }
            _cards.Add(card);
            ComputeValue();
        }

        /// <summary>
        /// Add only one more card to the hand.
        /// </summary>
        /// <param name="card">Card to add</param>
        public void DoubleDown(Card card, ChipStack DDBet)
        {
            Bet += DDBet;
            Hit(card);
            DoubledDown = true;
        }

        /// <summary>
        /// Split this hand.  Function returns a new hand with one of the cards.
        /// </summary>
        /// <returns>New hand</returns>
        public Hand Split(ChipStack Bet)
        {
            if (!this.IsSplittable)
            {
                throw new Exception("Hand cannot be split.");
            }
            Card SplitCard = this._cards[1];
            this._cards.RemoveAt(1);
            ComputeValue();
            Hand newHand = new Hand(Bet, new Card[] { SplitCard });
            newHand.ValuePrePlay = this.ValuePrePlay;
            newHand.IsSoftPrePlay = this.IsSoftPrePlay;
            return newHand;
        }

        /// <summary>
        /// Create an array with the cards in the current hand, to be handed back to a discard shoe.
        /// </summary>
        /// <returns>All of the cards in the hand</returns>
        public Card[] ReturnCards()
        {
            Card[] ReturnPile = _cards.ToArray();
            _cards.Clear();
            ComputeValue();
            return ReturnPile;
        }

        public ChipStack PayOut()
        {
            ChipStack retval = Bet;
            Bet = new ChipStack(0);
            return retval;
        }

        /// <summary>
        /// To be called anytime a card is added to the hand.
        /// </summary>
        private void ComputeValue()
        {
            Value = 0;
            IsSoft = false;
            int NumAces = 0;
            foreach (Card card in _cards)
            {
                Value += card.Value;
                if (card.Face == Face.Ace)
                {
                    NumAces++;
                }
            }
            while (Value < 12 && NumAces > 0)
            {
                IsSoft = true;
                NumAces--;
                Value += 10;
            }
            if (_cards.Count == 2 && ValuePrePlay == 0)
            {
                ValuePrePlay = Value;
                IsSoftPrePlay = IsSoft;
            }
        }
    }
}
