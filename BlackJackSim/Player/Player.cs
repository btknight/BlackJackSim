using System;
using System.Collections.Generic;
using System.Text;

namespace BlackJackSim
{
    /// <summary>
    /// A blackjack player.  Implements logic to act on decisions, but 
    /// does not implement any decision making.  This class must be derived, and the relevant Decide...() methods
    /// must be implemented.
    /// Player does not come with martini.
    /// </summary>
    public abstract class Player
    {
        protected Table _Table { get; set; }
        
        /// <summary>
        /// List of active blackjack hands.  Empty when there is no game going on.
        /// </summary>
        public List<Hand> Hands { get; private set; }
        
        /// <summary>
        /// Statistics tracking.
        /// </summary>
        public ScorePerHand Score { get; private set; }

        protected PlayTrack Track { get; set; }
        
        /// <summary>
        /// Player's money.
        /// </summary>
        protected ChipStack Chips { get; set; }
        public int CashOnHand
        {
            get
            {
                return Chips.Value;
            }
        }

        /// <summary>
        /// Create new player object.  Every new player starts with 100,000 chips.
        /// </summary>
        public Player(ChipStack startingPurse)
        {
            Hands = new List<Hand>();
            Score = new ScorePerHand();
            Track = new PlayTrack();
            Chips = new ChipStack(0);
            
            if (startingPurse != null)
            {
                AcceptChips(startingPurse);
            }
        }

        /// <summary>
        /// Seat this player at a Table.  Called by the Table to offer itself up to the player for interaction.
        /// </summary>
        /// <param name="table">Table object calling the player</param>
        public virtual void SeatPlayer(Table table)
        {
            if (_Table != null)
            {
                throw new Exception("Player is already seated at a table");
            }
            _Table = table;
        }

        /// <summary>
        /// Used by the table object to elicit an ante from the player.  
        /// The player will return cash if the player has enough.
        /// Default action is to bid only the table minimum.  This can be overridden to send more money.
        /// </summary>
        /// <param name="sender">Calling object</param>
        /// <param name="Minimum">Minimum buy-in for the table</param>
        /// <returns></returns>
        public virtual ChipStack RequestAnte()
        {
            if (Chips.Value < _Table.Rules.MinimumBet)
            {
                return null;
            }
            int Amount = DecideAnte();
            if (Amount > Chips.Value)
            {
                throw new Exception("Player object decided on an ante too large for available funds");
            }
            if (!_Table.Rules.CheckBet(Amount))
            {
                throw new Exception("Player decided on an ante too large or small for the table");
            }
            if (Amount == 0)
            {
                return null;
            }
            return Chips.RemoveChips(Amount);
        }

        /// <summary>
        /// Accepts chips from the table, and adds them to the player's chip stack.
        /// </summary>
        /// <param name="stack">Stack of chips offered by the table</param>
        public void AcceptChips(ChipStack stack)
        {
            Chips += stack;
        }

        /// <summary>
        /// Plays a single hand.  This may be called multiple times if a hand is split.
        /// </summary>
        /// <param name="table">The Table object to use to request cards</param>
        /// <param name="CurrentHand">Hand to play</param>
        public void PlayHand(Hand CurrentHand)
        {
            while (CurrentHand.Value <= 21 && !CurrentHand.DoubledDown)
            {
                PlayAction Action = this.DecideAction(_Table.UpCard, CurrentHand);
                switch (Action)
                {
                    case PlayAction.Split:
                        _Table.Split(CurrentHand, this, Chips.RemoveChips(CurrentHand.Bet.Value));
                        Track.Splits++;
                        break;
                    case PlayAction.DoubleDown:
                        _Table.DoubleDown(CurrentHand, Chips.RemoveChips(CurrentHand.Bet.Value));
                        Track.Doubles++;
                        break;
                    case PlayAction.Hit:
                        _Table.Hit(CurrentHand);
                        Track.Hits++;
                        break;
                    case PlayAction.Stand:
                        Track.Stands++;
                        return;
                }
            }
            if (CurrentHand.Value > 21) { Track.Busted++; }     // Wah wah waaaah.
        }

        /// <summary>
        /// Called by the table to play insurance bets.  This calls DecideInsurance(), implemented in child classes.
        /// </summary>
        /// <returns>Returns a ChipStack with an insurance bet, otherwise null.</returns>
        public ChipStack PlayInsurance()
        {
            if (DecideInsurance())
            {
                ChipStack insurance = Chips.RemoveChips((int)(Hands[0].Bet.Value * 0.5));
                return insurance;
            }
            return null;
        }

        /// <summary>
        /// Decision function for amount of ante.
        /// </summary>
        /// <returns>Returns ante amount.  If amount is 0, player will not submit an ante and will not join the game.</returns>
        abstract protected int DecideAnte();

        /// <summary>
        /// AI decision function.
        /// </summary>
        /// <param name="UpCard">Dealer's up card</param>
        /// <param name="CurrentHand">Hand in play</param>
        /// <returns>Instructions for PlayHand() to execute: hit, stand, double down, or split.</returns>
        abstract protected PlayAction DecideAction(Card UpCard, Hand CurrentHand);

        /// <summary>
        /// Decision function to take insurance or not.  Triggered only at the start of a game, when the dealer has an ace showing.
        /// </summary>
        /// <returns>True if the player takes the bet, false if the player does not</returns>
        abstract protected bool DecideInsurance();

        /// <summary>
        /// Used by DecideAction to communicate to PlayHand() what action to take.
        /// </summary>
        protected enum PlayAction
        {
            Hit,
            Stand,
            DoubleDown,
            Split
        }

        public string Scoreboard()
        {
            return String.Format("[{0}]: Cash={1}\r\n  {2}",
                this.GetType().ToString(), CashOnHand, Score.Scoreboard());
        }

        public string CSVHeader()
        {
            return String.Join(",", "Cash", Score.CSVHeader());
        }

        public string CSVString()
        {
            return String.Join(",", Chips.Value, Score.CSVString());
        }
    }
}