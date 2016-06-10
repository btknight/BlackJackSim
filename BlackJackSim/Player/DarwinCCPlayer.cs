using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    /// <summary>
    /// This class derives from a player with good basic strategy.
    /// This class also is smart enough to perform card counting.
    /// </summary>
    public class DarwinCCPlayer : DarwinPlayer
    {
        /// <summary>
        /// The current card count.
        /// </summary>
        private int CardCount { get; set; }

        /// <summary>
        /// Number of units to ante on the current hand.
        /// </summary>
        private int CurrentBetUnits { get; set; }
        
        /// <summary>
        /// Number of chips to ante on the current hand.
        /// </summary>
        private int CurrentBet
        {
            get
            {
                return CurrentBetUnits * BetIncrement;
            }
        }

        /// <summary>
        /// One betting unit will comprise this amount of chips.
        /// </summary>
        private int BetIncrement { get; set; }

        /// <summary>
        /// Number of pushes seen.
        /// </summary>
        private int Pushes { get; set; }

        public DarwinCCPlayer(ChipStack startingPurse) 
            : base(startingPurse) 
        {
            CardCount = 0;
            CurrentBetUnits = 0;
            BetIncrement = 0;
            Pushes = 0;
        }

        /// <summary>
        /// Seat this player at a Table.  Called by the Table to offer itself up to the player for interaction.
        /// This player watches the table closely, hence the subscription to table events.
        /// </summary>
        /// <param name="table">Table object calling the player</param>
        public override void SeatPlayer(Table table)
        {
            base.SeatPlayer(table);
            BetIncrement = _Table.Rules.MinimumBet;
            CurrentBetUnits = _Table.Rules.MinimumBet / BetIncrement;
            _Table.CardExposed += NoteCard;
            _Table.ShoeShuffled += table_ShoeShuffled;
        }

        protected override int DecideAnte()
        {
            /* "The ideal way to bet when you count at blackjack is to bet in exact proportion to the count.  Whenever
             * the count is negative, 0, or +1, you would not bet at all but instead sit out the hand.  However, if the
             * odds are higher than +1, the odds are in your favor.  In such a case, you would ideally bet a number of units
             * equal to the plus count.  If the count were +2, you would bet two units.  If the count were +5, you would
             * bet five units.  This would mean that the greater your advantage on the next hand, the bigger your bet
             * would be." pp80-81
             * 
             * "... There are four guidelines you should adopt in your betting.  First, 'No matter how much the count improves 
             * from one hand to the next, never bet more than double what you bet on the previous hand.' ... Second, 'No matter 
             * how much the count worsens from one hand to the next, never bet less than half of what you bet on the previous
             * hand.'
             * "The third rule is, 'Never change the size of your bet after a tie.'"
             * "The fourth rule, and one of the most important to keep in mind to disguise your betting strategy, is 'Never
             * hesitate when making a bet.'" ;)
             */
            int PriorBetUnits = CurrentBetUnits;

            // 'Never change the size of your bet after a tie.'
            if (Score.Push != Pushes)
            {
                Pushes = Score.Push;
                if (CurrentBet > Chips.Value) { CurrentBetUnits = Chips.Value / BetIncrement; }
                return CurrentBet;
            }

            // You would ideally bet a number of units equal to the plus count.
            int BetMultiplier = CardCount;
            CurrentBetUnits = BetMultiplier;

            // 'No matter how much the count worsens from one hand to the next, never bet less than half of 
            // what you bet on the previous hand.'
            if (CurrentBetUnits < PriorBetUnits / 2) { CurrentBetUnits = PriorBetUnits / 2; }
            if (CurrentBet < _Table.Rules.MinimumBet) { CurrentBetUnits = _Table.Rules.MinimumBet / BetIncrement; }

            // 'No matter how much the count improves from one hand to the next, never bet more than double 
            // what you bet on the previous hand.'
            if (CurrentBetUnits > PriorBetUnits * 2) { CurrentBetUnits = PriorBetUnits * 2; }
            if (_Table.Rules.MaximumBet > 0 && CurrentBet > _Table.Rules.MaximumBet / 4)
            {
                CurrentBetUnits = _Table.Rules.MaximumBet / (BetIncrement * 4);
            }

            if (CurrentBet > Chips.Value) { CurrentBetUnits = Chips.Value / BetIncrement; }
            return CurrentBet;
        }

        /// <summary>
        /// Called when the table reshuffles the dealer shoe.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event args (ignored)</param>
        void table_ShoeShuffled(object sender, EventArgs e)
        {
            CardCount = 0;
        }

        /// <summary>
        /// Describes how to progress the count.
        /// </summary>
        private static int[] CardCountMatrix = { -1, 1, 1, 1, 1, 1, 0, 0, 0, -1 };

        /// <summary>
        /// Called when the table has revealed a new card.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">A CardExposedEventArgs object containing a copy of the card revealed</param>
        private void NoteCard(object sender, CardExposedEventArgs e)
        {
            CardCount += CardCountMatrix[e.Card.Value - 1];
        }
    }
}
