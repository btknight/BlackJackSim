using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BlackJackSim
{
    /// <summary>
    /// Emulates a blackjack table.  The table is fully automated.  The table object manages
    /// the game.
    /// </summary>
    public class Table
    {
        private Random random { get; set; }
        
        /// <summary>
        /// The dealer player object.  Contains the AI for playing a dealer hand (hit 16, stand on 17 or higher)
        /// </summary>
        private Dealer Dealer { get; set; }

        /// <summary>
        /// Table rules.
        /// </summary>
        public TableRules Rules { get; private set; }

        /// <summary>
        /// The dealer's card that is exposed to everyone seated at the table.
        /// </summary>
        public Card UpCard
        {
            get
            {
                return Dealer.Hands[0].Cards[1];
            }
        }

        /// <summary>
        /// The dealer shoe contains the cards that will be dealt during play.
        /// </summary>
        private Deck DealerShoe { get; set; }

        /// <summary>
        /// The discard shoe contains the cards used during play.
        /// </summary>
        private Deck DiscardShoe { get; set; }

        /// <summary>
        /// If the DealerShoe drops below this many cards in the shoe, reshuffle after the turn is complete.
        /// </summary>
        private int ReshuffleLimit { get; set; }
        
        /// <summary>
        /// Array containing the players at the table.  Players have Hands, code to take actions 
        /// (hitting, doubling down on hands, etc) and decision-making code.
        /// </summary>
        public Player[] Players { get; private set; }

        /// <summary>
        /// Number of players seated at the table
        /// </summary>
        public int NumPlayers
        {
            get
            {
                return Players.Length;
            }
        }

        /// <summary>
        /// Called when a card is turned face up.
        /// </summary>
        public event EventHandler<CardExposedEventArgs> CardExposed;
        
        /// <summary>
        /// Called when the dealer shoe is reshuffled.
        /// </summary>
        public event EventHandler ShoeShuffled;

        /// <summary>
        /// Initializes a new table.
        /// </summary>
        /// <param name="rules">TableRules object describing the rules of the table</param>
        /// <param name="players">Array of players to be seated at the table</param>
        public Table(TableRules rules, Player[] players)
        {
            if (rules == null || players == null)
            {
                throw new Exception("Error: either TableRules or Player[]s were missing");
            }
            Rules = rules;

            // Validate admission of new players.
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == null)
                {
                    throw new Exception("Error: player spot is open, but no player exists");
                }
                else
                {
                    players[i].SeatPlayer(this);
                }
            }
            Players = players;

            random = new Random();

            Debug.WriteLine("table> Initializing decks");
            DealerShoe = new Deck(Rules.CardDecks);
            DealerShoe.FillWithNewCards(Rules.CardDecks);
            DealerShoe.Shuffle(Rules.InitialShuffle);               // Hey Pinky, scrumble em up good
            DiscardShoe = new Deck(Rules.CardDecks);
            ReshuffleLimit = (int)(Rules.ReshuffleMinimum * DealerShoe.CardsLeft) +
                random.Next((int)((Rules.ReshuffleMaximum - Rules.ReshuffleMinimum) * DealerShoe.CardsLeft));

            Debug.WriteLine("table> Initializing dealer");
            Dealer = new Dealer(null);
            Dealer.SeatPlayer(this);
        }

        /// <summary>
        /// Draws a card from the dealer shoe.
        /// </summary>
        /// <param name="FaceDown">Set this to true if the card is dealt face down.  Default is false</param>
        /// <returns>Card drawn from dealer shoe</returns>
        private Card DrawCard(bool FaceDown = false)
        {
            if(DealerShoe.CardsLeft == 0)
            {
                Debug.WriteLine("table> No cards left to deal.  Reshuffling discards");
                Reshuffle();
            }
            Card newCard = DealerShoe.DrawCard();
            if (!FaceDown)
            {
                OnCardExposure(new CardExposedEventArgs(newCard.Clone()));
            }
            return newCard;
        }

        /// <summary>
        /// Empty the DealerShoe, swap the Discard with Dealer shoes, and reshuffle.
        /// </summary>
        private void Reshuffle()
        {
            while (DealerShoe.CardsLeft > 0)
            {
                DiscardShoe.AddCard(DealerShoe.DrawCard());
            }
            Deck tmp = DiscardShoe;
            DiscardShoe = DealerShoe;
            DealerShoe = tmp;
            DealerShoe.Shuffle(Rules.SubsequentShuffle);
            OnShoeShuffle();
            ReshuffleLimit = (int)(Rules.ReshuffleMinimum * DealerShoe.CardsLeft) +
                random.Next((int)((Rules.ReshuffleMaximum - Rules.ReshuffleMinimum) * DealerShoe.CardsLeft));
        }

        /// <summary>
        /// Called when a card is dealt face-up on the table.  Card counters watch this!
        /// </summary>
        /// <param name="e">CardExposedEventArgs containing a copy of the card dealt</param>
        private void OnCardExposure(CardExposedEventArgs e)
        {
            if (CardExposed != null)
            {
                CardExposed(this, e);
            }
        }
        
        /// <summary>
        /// Called when the DealerShoe is reshuffled.  Card counters watch this!
        /// </summary>
        private void OnShoeShuffle()
        {
            if (ShoeShuffled != null)
            {
                ShoeShuffled(this, new EventArgs());
            }
        }

        /// <summary>
        /// Used by players to split a hand.
        /// </summary>
        /// <param name="hand">Hand to be split</param>
        /// <param name="player">Player object calling this method.  Used to add the split hand to the player's list of hands.</param>
        /// <param name="NewBet">New bet to be applied to the split hand</param>
        public void Split(Hand hand, Player player, ChipStack NewBet)
        {
            if (NewBet.Value != hand.Bet.Value)
            {
                throw new Exception("Error: Player tried to double down with a bet unequal to the hand bet");
            }
            if (!Rules.CheckBet(hand.Bet.Value + NewBet.Value))
            {
                throw new Exception("Error: Bet would exceed table maximum");
            }
            Hand newHand = hand.Split(NewBet);
            Hit(hand);
            Hit(newHand);
            player.Hands.Add(newHand);
        }

        /// <summary>
        /// Used by players to hit a hand.
        /// </summary>
        /// <param name="hand">Hand to hit</param>
        public void Hit(Hand hand)
        {
            if (hand.Value > 20)
            {
                throw new Exception("The dealer glares at the player.  Why would you want to hit a 21?");
            }
            hand.Hit(DrawCard());
        }

        /// <summary>
        /// Used by players to double down on a hand.
        /// </summary>
        /// <param name="hand">Hand to double down</param>
        public void DoubleDown(Hand hand, ChipStack NewBet)
        {
            if (NewBet.Value != hand.Bet.Value)
            {
                throw new Exception("Error: Player tried to double down with a bet unequal to the hand bet");
            }
            if (!Rules.CheckBet(hand.Bet.Value + NewBet.Value))
            {
                throw new Exception("Error: Bet would exceed table maximum");
            }
            hand.DoubleDown(DrawCard(), NewBet);
        }

        /// <summary>
        /// Used to return cards to the discard pile, after a hand is over.
        /// </summary>
        /// <param name="cards">Grouping of cards to add to the pile</param>
        public void DiscardCards(IEnumerable<Card> cards)
        {
            DiscardShoe.ReturnCards(cards);
        }

        /// <summary>
        /// Play one round of blackjack.
        /// </summary>
        public void Play()
        {
            Debug.WriteLine("table> Starting round");

            // Deal everyone in.
            Play_Deal();

#if DEBUG
            {
                Card _upcard = this.UpCard;
                Hand[] Hands = new Hand[NumPlayers];
                for (int i = 0; i < NumPlayers; i++)
                {
                    if (Players[i].Hands.Count > 0)
                    {
                        Hand playerHand = Players[i].Hands[0];
                        Hands[i] = playerHand;
                        if (playerHand.ValuePrePlay == 12 && playerHand.IsSoftPrePlay)
                        {
                            Debug.WriteLine("% Found a very interesting combo");
                        }
                        if (playerHand.ValuePrePlay == 4 && !playerHand.IsSoftPrePlay)
                        {
                            Debug.WriteLine("% Found an interesting combo");
                        }
                    }
                }
            }
#endif

            ChipStack[] InsuranceBets = new ChipStack[Players.Length];

            // If dealer has an ace card face up, offer insurance.
            if (UpCard.Face == Face.Ace)
            {
                for (int i = 0; i < Players.Length; i++)
                {
                    if (Players[i].Hands.Count == 1)
                    {
                        ChipStack Bet = Players[i].PlayInsurance();
                        InsuranceBets[i] = Bet;
                    }
                }
            }

            // Check for dealer natural.
            if (Dealer.Hands[0].Value == 21)
            {
                // Reveal dealer hole card.
                OnCardExposure(new CardExposedEventArgs(Dealer.Hands[0].Cards[0].Clone()));

                Debug.WriteLine("table> Dealer has a natural.  Ending play");

                // Pay out insurance bets.
                for (int i = 0; i < Players.Length; i++)
                {
                    if (InsuranceBets[i] != null)
                    {
                        ChipStack Winnings = InsuranceBets[i] + new ChipStack(InsuranceBets[i].Value);
                        InsuranceBets[i] = null;
                        Players[i].AcceptChips(Winnings);
                    }
                }
                Play_Finish();
                return;
            }

            // Dealer does not have a natural.  House pockets insurance.
            InsuranceBets.Initialize();

            // Pass control to players.
            foreach (Player player in Players)
            {
                Play_StartPlayerTurn(player);
            }

            // Reveal dealer hole card.
            OnCardExposure(new CardExposedEventArgs(Dealer.Hands[0].Cards[0].Clone()));

            // Play dealer hand.
            Dealer.PlayHand(Dealer.Hands[0]);

            // Tally score and collect player cards.
            Play_Finish();

            // If we need to reshuffle, do so now.
            if (DealerShoe.CardsLeft < ReshuffleLimit)
            {
                Reshuffle();
            }
        }

        /// <summary>
        /// Procedure to deal the cards.
        /// </summary>
        private void Play_Deal()
        {
            // First, obtain ante from all players, and create Hands.
            Dealer.Hands.Add(new Hand(new ChipStack(0)));
            for (int i = 0; i < NumPlayers; i++)
            {
                ChipStack ante = Players[i].RequestAnte();
                if (ante != null)
                {
                    if (!Rules.CheckBet(ante.Value))
                    {
                        Debug.WriteLine("Player tried to bet less than the table minimum, or bet more than maximum.");
                        // Refuse the ante.
                        Players[i].AcceptChips(ante);
                    }
                    else
                    {
                        Players[i].Hands.Add(new Hand(ante));
                    }
                }
            }

            // Deal em out Joliet-style.
            Dealer.Hands[0].Hit(DrawCard(true));
            for (int i = 0; i < NumPlayers; i++)
            {
                if (Players[i].Hands.Count > 0)
                {
                    Players[i].Hands[0].Hit(DrawCard());
                }
            }

            Dealer.Hands[0].Hit(DrawCard());
            for (int i = 0; i < NumPlayers; i++)
            {
                if (Players[i].Hands.Count > 0)
                {
                    Players[i].Hands[0].Hit(DrawCard());
                }
            }
        }

        /// <summary>
        /// Pass control to players to make decisions.
        /// </summary>
        /// <param name="player">Player to be handed control</param>
        private void Play_StartPlayerTurn(Player player)
        {
            // If player has a natural, mark as a win, and remove hand from play.
            if (player.Hands.Count > 0 && player.Hands[0].Value == 21)
            {
                Debug.WriteLine("table> Player has a natural");
                player.Score.IncrementScore(GameResult.Won, UpCard, player.Hands[0]);
                ChipStack Winnings = player.Hands[0].PayOut();
                Winnings += new ChipStack( (int)(Winnings.Value * 1.5) );
                player.AcceptChips(Winnings);
                DiscardCards(player.Hands[0].ReturnCards());
                player.Hands.RemoveAt(0);
                return;
            }

            // Hand control to the player to make decisions.
            for (int i = 0; i < player.Hands.Count; i++)
            {
                Hand CurrentHand = player.Hands[i];
                player.PlayHand(CurrentHand);
            }

            // Player is done.  Player may now have multiple hands due to splits.
            // Build a boolean table to mark busted hands to remove from the list.
            bool[] HandsToRemove = new bool[player.Hands.Count];

            // Mark busted hands in table.
            for (int i = 0; i < player.Hands.Count; i++)
            {
                Hand playerHand = player.Hands[i];
#if DEBUG
                if (playerHand.ValuePrePlay == 12 && playerHand.IsSoftPrePlay)
                {
                    Debug.WriteLine("% Found a very interesting combo");
                }
                if(playerHand.ValuePrePlay == 4 && !playerHand.IsSoftPrePlay)
                {
                    Debug.WriteLine("% Found an interesting combo");
                }
#endif
                if (playerHand.Value > 21)
                {
                    Debug.WriteLine("Player " + i.ToString() + " loses - player busted");
                    player.Score.IncrementScore(GameResult.Lost, UpCard, playerHand);
                    DiscardCards(playerHand.ReturnCards());
                    HandsToRemove[i] = true;
                }
                else
                {
                    HandsToRemove[i] = false;
                }
            }
            
            // Remove busted hands from the list.
            for (int i = HandsToRemove.Length - 1; i >= 0; i--)
            {
                if (HandsToRemove[i])
                {
                    player.Hands.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Finish the round after players are done.  Compare hands, dole out winnings, collect cards, and clear Hands.
        /// </summary>
        private void Play_Finish()
        {
            // Compare player hands to the dealer.
            for (int i = 0; i < NumPlayers; i++)
            {
                foreach (Hand playerHand in Players[i].Hands)
                {
                    // If dealer busted, all hands still standing win.
                    if (Dealer.Hands[0].Value > 21)
                    {
                        Debug.WriteLine("Player " + i.ToString() + " wins - dealer busted");
                        Players[i].Score.IncrementScore(GameResult.Won, UpCard, playerHand);
                        ChipStack Winnings = playerHand.PayOut();
                        Winnings += new ChipStack(Winnings.Value);
                        Players[i].AcceptChips(Winnings);
                    }
                    else
                    {
                        if (playerHand.Value < Dealer.Hands[0].Value)
                        {
                            Debug.WriteLine("Player " + i.ToString() + " loses - " + playerHand.Value.ToString() + " vs " + Dealer.Hands[0].Value.ToString());
                            Players[i].Score.IncrementScore(GameResult.Lost, UpCard, playerHand);
                        }
                        if (playerHand.Value > Dealer.Hands[0].Value)
                        {
                            Debug.WriteLine("Player " + i.ToString() + " wins - " + playerHand.Value.ToString() + " vs " + Dealer.Hands[0].Value.ToString());
                            Players[i].Score.IncrementScore(GameResult.Won, UpCard, playerHand);
                            ChipStack Winnings = playerHand.PayOut();
                            Winnings += new ChipStack(Winnings.Value);
                            Players[i].AcceptChips(Winnings);
                        }
                        if (playerHand.Value == Dealer.Hands[0].Value)
                        {
                            Debug.WriteLine("Player " + i.ToString() + " pushes - " + playerHand.Value.ToString() + " vs " + Dealer.Hands[0].Value.ToString());
                            Players[i].Score.IncrementScore(GameResult.Pushed, UpCard, playerHand);
                            ChipStack Winnings = playerHand.PayOut();
                            Players[i].AcceptChips(Winnings);
                        }
                    }
                    DiscardCards(playerHand.ReturnCards());
                }
                Players[i].Hands.Clear();
            }
            DiscardCards(Dealer.Hands[0].ReturnCards());
            Dealer.Hands.Clear();
        }

        public string CSVHeader()
        {
            string[] Headers = new string[NumPlayers];
            for (int i = 0; i < NumPlayers; i++)
            {
                Headers[i] = Players[i].CSVHeader();
            }
            return String.Join(",", Headers);
        }

        public string CSVString()
        {
            string[] Data = new string[NumPlayers];
            for (int i = 0; i < NumPlayers; i++)
            {
                Data[i] = Players[i].CSVString();
            }
            return String.Join(",", Data);
        }


        /// <summary>
        /// Get a string summarizing player wins and losses.
        /// </summary>
        /// <returns>Printable list of wins and losses</returns>
        public string Scoreboard()
        {
            StringBuilder sb = new StringBuilder();
            ScorePerHand Totals = new ScorePerHand();
            for (int i = 0; i < NumPlayers; i++)
            {
                sb.AppendFormat("Player {0} {1}", i, Players[i].Scoreboard());
                Totals += Players[i].Score;
            }
            sb.AppendFormat("\r\nTotals on hands played: {0}", Totals.Scoreboard());
            sb.Append(Totals.StreakStats());
            return sb.ToString();
        }
    }
}
