using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    /// <summary>
    /// Extremely cautious player.  Never hits past 12, never splits, never doubles down.  
    /// Voted for Reagan in 2012.
    /// </summary>
    public class NoBustPlayer : Player
    {
        public NoBustPlayer(ChipStack startingPurse) : base(startingPurse) { }

        protected override int DecideAnte()
        {
            return _Table.Rules.MinimumBet;
        }

        protected override PlayAction DecideAction(Card UpCard, Hand CurrentHand)
        {
            while (CurrentHand.Value < 12)
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
