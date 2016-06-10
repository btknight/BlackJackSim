using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    /// <summary>
    /// Implements play logic for a dealer.  Stand on 17 or higher, hard or soft.  
    /// Hit on 16 or less.
    /// </summary>
    public class Dealer : Player
    {
        public Dealer(ChipStack startingPurse) : base(startingPurse) { }

        protected override int DecideAnte()
        {
            return _Table.Rules.MinimumBet;
        }

        protected override PlayAction DecideAction(Card UpCard, Hand CurrentHand)
        {
            while (CurrentHand.Value < 17)
            {
                return PlayAction.Hit;
            }
            return PlayAction.Stand;
        }

        protected override bool DecideInsurance()
        {
            return false;
        }
    }
}
