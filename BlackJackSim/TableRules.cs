using System;

namespace BlackJackSim
{
    /// <summary>
    /// Defines rules for the table.  This is used by the table to enforce rules, and used by players to abide by
    /// those rules.
    /// </summary>
    public class TableRules
    {
        /// <summary>
        /// Minimum bet permitted to play.
        /// </summary>
        public int MinimumBet { get; private set; }

        /// <summary>
        /// Maximum bet permitted on the table.  Set to -1 if there is no limit.
        /// </summary>
        public int MaximumBet { get; private set; }

        /// <summary>
        /// Number of 52-card decks to put into the dealer shoe.
        /// </summary>
        public int CardDecks { get; private set; }

        /// <summary>
        /// Number of times to shuffle the deck the first time it is generated.
        /// </summary>
        public int InitialShuffle { get; private set; }

        /// <summary>
        /// Number of times to reshuffle the deck.
        /// </summary>
        public int SubsequentShuffle { get; private set; }

        /// <summary>
        /// Minimum percentage of cards remaining before dealer will reshuffle the stack.  For example, if 
        /// ReshuffleMinimum were set to 0.25, 75% of the cards in the shoe may be dealt before a reshuffle must occur.
        /// </summary>
        public double ReshuffleMinimum { get; private set; }

        /// <summary>
        /// Maximum percentage of cards remaining before dealer may reshuffle the stack.  For example, if 
        /// ReshuffleMaximum were set to 0.30, 70% of the cards in the shoe must be dealt before a reshuffle may take place.
        /// </summary>
        public double ReshuffleMaximum { get; private set; }

        public TableRules(
            int minimumBet,
            int maximumBet,
            int cardDecks,
            int initialShuffle,
            int subsequentShuffle,
            double reshuffleMinimum,
            double reshuffleMaximum
            )
        {
            if (minimumBet < 1)
            {
                throw new Exception("Minimum bet cannot be less than 1");
            }
            MinimumBet = minimumBet;

            if (maximumBet < minimumBet && maximumBet != -1)
            {
                throw new Exception("Maximum bet cannot be less than minimum");
            }
            MaximumBet = maximumBet;

            if (cardDecks < 1 || cardDecks > 19231)
            {
                throw new Exception("CardDecks must be greater than 1 and less than 19231 (~1 million cards)");
            }
            CardDecks = cardDecks;

            if (initialShuffle < 1 || initialShuffle > 32)
            {
                throw new Exception("Initial number of shuffles must be between 1 and 32");
            }
            InitialShuffle = initialShuffle;

            if (subsequentShuffle < 1 || subsequentShuffle > 32)
            {
                throw new Exception("Initial number of shuffles must be between 1 and 32");
            }
            SubsequentShuffle = subsequentShuffle;

            if (reshuffleMinimum < 0.0 || reshuffleMinimum > 32)
            {
                throw new Exception("Initial number of shuffles must be between 1 and 32");
            }
            ReshuffleMinimum = reshuffleMinimum;

            if (subsequentShuffle < 1 || subsequentShuffle > 32)
            {
                throw new Exception("Initial number of shuffles must be between 1 and 32");
            }
            ReshuffleMaximum = reshuffleMaximum;
        }

        public bool CheckBet(int Bet)
        {
            if (Bet < MinimumBet) { return false; }
            if (MaximumBet != -1 && Bet > MaximumBet) { return false; }
            return true;
        }
    }
}
