using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    /// <summary>
    /// This class implements decision logic useful for basic strategy.  
    /// Class defines the names of the matrices used for decision-making, but does not
    /// implement these.  Derived classes will implement the matrices.
    /// </summary>
    public abstract class BasicStrategyPlayer : Player
    {
        public BasicStrategyPlayer(ChipStack startingPurse) : base(startingPurse) { }
        
        /// <summary>
        /// Describes what action should be taken if the hand is splittable.
        /// If this matrix returns true, then split the hand.  If false, do not, and 
        /// play normally.
        /// The X dimension is the card value of ONE of the splittable pair.
        /// The Y dimension is the dealer up card.
        /// </summary>
        protected static bool[,] ShouldSplit;

        /// <summary>
        /// Matrix describing whether a soft hand should be doubled down.
        /// The X dimension is the value of the player's hand.
        /// The Y dimension is the dealer up card.
        /// </summary>
        protected static bool[,] SoftDouble;
        
        /// <summary>
        /// Matrix describing whether to stand on a soft hand.
        /// The X dimension is the value of the player's hand.
        /// The Y dimension is the dealer up card.
        /// </summary>
        protected static bool[,] SoftStand;
        
        /// <summary>
        /// Matrix describing whether a hard hand should be doubled down.
        /// The X dimension is the value of the player's hand.
        /// The Y dimension is the dealer up card.
        /// </summary>
        protected static bool[,] HardDouble;
        
        /// <summary>
        /// Matrix describing whether to stand on a hard hand.
        /// The X dimension is the value of the player's hand.
        /// The Y dimension is the dealer up card.
        /// </summary>
        protected static bool[,] HardStand;

        protected override int DecideAnte()
        {
            return _Table.Rules.MinimumBet;
        }

        protected override PlayAction DecideAction(Card UpCard, Hand CurrentHand)
        {
            int UpCardIdx = UpCard.Value - 1;

            // First, see if we need to split.
            if (CurrentHand.IsSplittable && Chips.Value > CurrentHand.Bet.Value)
            {
                int SplitCardIdx = CurrentHand.Cards[0].Value - 1;
                if (ShouldSplit[SplitCardIdx, UpCardIdx])
                {
                    return PlayAction.Split;
                }
            }

            // If there's an ace in my hand, follow the soft play rules.
            if (CurrentHand.IsSoft)
            {
                if (SoftDouble[CurrentHand.Value, UpCardIdx] && Chips.Value > CurrentHand.Bet.Value)
                {
                    return PlayAction.DoubleDown;
                }
                if (SoftStand[CurrentHand.Value, UpCardIdx])
                {
                    return PlayAction.Stand;
                }
                return PlayAction.Hit;
            }

            // Otherwise follow hard hand rules.
            if (HardDouble[CurrentHand.Value, UpCardIdx] && Chips.Value > CurrentHand.Bet.Value)
            {
                return PlayAction.DoubleDown;
            }
            if (HardStand[CurrentHand.Value, UpCardIdx])
            {
                return PlayAction.Stand;
            }
            return PlayAction.Hit;
        }

        protected override bool DecideInsurance()
        {
            return false;
        }
    }
}
