using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    /// <summary>
    /// Blackjack scorekeeping.  Basic win / loss / push tracking.
    /// </summary>
    [Serializable]
    public class Score
    {
        /// <summary>
        /// Array tracking wins, losses, and pushes.  Uses GameResult for array position.
        /// </summary>
        protected int[] _results;

        /// <summary>
        /// Number of hands won.
        /// </summary>
        public int Won
        {
            get
            {
                return _results[(int)GameResult.Won];
            }
            set
            {
                int Difference = value - _results[(int)GameResult.Won];
                _results[(int)GameResult.Won] = value;
                if (Difference > 0)
                {
                    Streak.UpdateStreak(GameResult.Won, Difference);
                }
            }
        }

        /// <summary>
        /// Number of hands lost.
        /// </summary>
        public int Lost
        {
            get
            {
                return _results[(int)GameResult.Lost];
            }
            set
            {
                int Difference = value - _results[(int)GameResult.Lost];
                _results[(int)GameResult.Lost] = value;
                if (Difference > 0)
                {
                    Streak.UpdateStreak(GameResult.Lost, Difference);
                }
            }
        }

        /// <summary>
        /// Number of hands resulting in a tie with the dealer.
        /// </summary>
        public int Push
        {
            get
            {
                return _results[(int)GameResult.Pushed];
            }
            set
            {
                int Difference = value - _results[(int)GameResult.Pushed];
                _results[(int)GameResult.Pushed] = value;
                if (Difference > 0)
                {
                    Streak.UpdateStreak(GameResult.Pushed, Difference);
                }
            }
        }

        /// <summary>
        /// Total number of hands played.
        /// </summary>
        public int TotalPlayed { get { return Won + Lost + Push; } }

        /// <summary>
        /// Streak tracking object.
        /// </summary>
        protected ScoreStreak Streak { get; set; }

        /// <summary>
        /// Constructor for scoreboard
        /// </summary>
        public Score()
        {
            _results = new int[Enum.GetValues(typeof(GameResult)).Length];
            Streak = new ScoreStreak();
        }

        /// <summary>
        /// Special secret constructor for scoreboard.  Permits setting all elements directly.  Supports
        /// addition operator.
        /// </summary>
        /// <param name="results"><see cref="_results"/></param>
        /// <param name="streak"><see cref="Streak"/></param>
        protected Score(int[] results, ScoreStreak streak)
        {
            if (results.GetLength(0) != Enum.GetValues(typeof(GameResult)).Length)
            {
                throw new Exception("Score() was fed a results tracking array of unexpected size");
            }
            _results = new int[Enum.GetValues(typeof(GameResult)).Length];
            for (int i = 0; i < Enum.GetValues(typeof(GameResult)).Length; i++)
            {
                _results[i] = results[i];
            }
            Streak = streak;
        }

        /// <summary>
        /// Add one result to the tally.
        /// </summary>
        /// <param name="Result">GameResult describing the outcome of the game</param>
        private void IncrementScore(GameResult Result)
        {
            switch (Result)
            {
                case GameResult.Won:
                    Won++;
                    break;
                case GameResult.Lost:
                    Lost++;
                    break;
                case GameResult.Pushed:
                    Push++;
                    break;
            }
        }
        public virtual void IncrementScore(GameResult Result, Card UpCard, Hand PlayerHand)
        {
            this.IncrementScore(Result);
        }

        /// <summary>
        /// Add two scoreboards together.  Useful for tallying individual totals.
        /// </summary>
        /// <param name="score1">Score object 1</param>
        /// <param name="score2">Score object 2</param>
        /// <returns>Score object with all totals added together</returns>
        public static Score operator +(Score score1, Score score2)
        {
            int[] addResultsMatrix = new int[Enum.GetValues(typeof(GameResult)).Length];
            for (int i = 0; i < Enum.GetValues(typeof(GameResult)).Length; i++)
            {
                addResultsMatrix[i] = score1._results[i] + score2._results[i];
            }
            return new Score(addResultsMatrix, score1.Streak + score2.Streak);
        }

        /// <summary>
        /// Returns streak statistics from the streak tracking object.
        /// </summary>
        /// <returns>Printable string describing streak statistics</returns>
        public string StreakStats()
        {
            return Streak.StreakStats(this);
        }

        /// <summary>
        /// Generates a printable string suitable for console displays.
        /// </summary>
        /// <returns>Text summary of stats</returns>
        public virtual string Scoreboard()
        {
            return String.Format("[Total/W-L-P]= {0} / {1}-{2}-{3}\r\n",
                TotalPlayed, Won, Lost, Push);
        }

        /// <summary>
        /// Generates a text header suitable for dumping to a CSV file.
        /// </summary>
        /// <returns>Text header for CSV</returns>
        public virtual string CSVHeader()
        {
            return String.Join(",", "Hands Played", "Hands Won", "Hands Lost", "Hands Pushed");
        }

        /// <summary>
        /// Generates a CSV record summarizing this scoreboard.
        /// </summary>
        /// <returns>Text record for CSV</returns>
        public virtual string CSVString()
        {
            return String.Join(",", TotalPlayed, Won, Lost, Push);
        }

        public virtual string StreakCSVHeader()
        {
            return Streak.CSVHeader();
        }
        public virtual string StreakCSVString()
        {
            return Streak.CSVString();
        }
    }
}
