using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    /// <summary>
    /// Tracks win / loss / push streaks.
    /// </summary>
    [Serializable]
    public class ScoreStreak
    {
        /// <summary>
        /// Maximum streak length recordable.
        /// </summary>
        private static readonly int _MAXSTREAKLEN = 48;     // not even the Cubs would lose this much in a row. I hope.

        /// <summary>
        /// Array describing the frequency of streaks. X dimension is a GameResult enum describing the result 
        /// of the hand; whether the hand won or lost or pushed.  Y dimension is the length of the streak subtracted by 1.  The 
        /// value in the array is the frequency a streak of that length and result occurred.
        /// </summary>
        public int[,] Streaks
        {
            get
            {
                // Creates a copy of the array.
                int[,] retval = new int[_streaks.GetLength(0), _streaks.GetLength(1)];
                for (int i = 0; i < _streaks.GetLength(0); i++)
                {
                    for (int j = 0; j < _streaks.GetLength(1); j++)
                    {
                        retval[i, j] = _streaks[i, j];
                    }
                }
                WriteStreak(retval);        // be sure to include the current streak in this array
                return retval;
            }
        }
        /// <summary>
        /// Internal storage for the streak tracking array
        /// </summary>
        private int[,] _streaks;

        /// <summary>
        /// Result of the current streak; whether it is a winning, losing or tying streak
        /// </summary>
        private GameResult _curStreakDir;
        /// <summary>
        /// Length of the current streak.  This value is NOT subtracted by 1 until it is put in the matrix.
        /// </summary>
        private int _curStreakLen;
        /// <summary>
        /// Text string describing the current streak.
        /// </summary>
        public string CurrentStreak
        {
            get
            {
                return String.Format("{0} {1}", _curStreakDir, _curStreakLen);
            }
        }

        /// <summary>
        /// Constructor for scoreboard
        /// </summary>
        public ScoreStreak()
        {
            _streaks = new int[Enum.GetValues(typeof(GameResult)).Length, _MAXSTREAKLEN];
        }

        /// <summary>
        /// Special secret constructor for scorestreak tracker.  Permits setting all elements directly.  Supports
        /// addition operator.
        /// </summary>
        /// <param name="streaks"><see cref="Streaks"/></param>
        private ScoreStreak(int[,] streaks)
        {
            if (streaks.GetLength(0) != Enum.GetValues(typeof(GameResult)).Length ||
                streaks.GetLength(1) != _MAXSTREAKLEN)
            {
                throw new Exception("Score() was fed a streak tracking array of unexpected size");
            }
            _streaks = new int[Enum.GetValues(typeof(GameResult)).Length, _MAXSTREAKLEN];
            for (int i = 0; i < Enum.GetValues(typeof(GameResult)).Length; i++)
            {
                for (int j = 0; j < _MAXSTREAKLEN; j++)
                {
                    _streaks[i, j] = streaks[i, j];
                }
            }
        }

        /// <summary>
        /// Add two scoreboards together.  Useful for tallying individual totals.
        /// </summary>
        /// <param name="score1">Score object 1</param>
        /// <param name="score2">Score object 2</param>
        /// <returns>Score object with all totals added together</returns>
        public static ScoreStreak operator +(ScoreStreak score1, ScoreStreak score2)
        {
            int[,] addStreakMatrix = new int[Enum.GetValues(typeof(GameResult)).Length, _MAXSTREAKLEN];
            int[,] streaks1 = score1.Streaks;
            int[,] streaks2 = score2.Streaks;
            for (int i = 0; i < Enum.GetValues(typeof(GameResult)).Length; i++)
            {
                for (int j = 0; j < _MAXSTREAKLEN; j++)
                {
                    addStreakMatrix[i, j] = streaks1[i, j] + streaks2[i,j];
                }
            }
            return new ScoreStreak(addStreakMatrix);
        }

        public void IncrementScore(GameResult Result, Card UpCard, Hand PlayerHand)
        {
            UpdateStreak(Result, 1);
        }

        /// <summary>
        /// Internal method called by the Won, Lost, and Push properties.  Updates internal streak tracking, and
        /// the array if applicable.
        /// </summary>
        /// <param name="direction">Directionality of the streak; whether it is a winning, losing or tying streak</param>
        /// <param name="Length">Length of streak to add</param>
        public void UpdateStreak(GameResult direction, int Length)
        {
            if (direction != _curStreakDir)
            {
                WriteStreak(_streaks);
                _curStreakDir = direction;
                _curStreakLen = 0;
            }
            _curStreakLen += Length;
        }

        /// <summary>
        /// Adds the current streak to a streak tracking array.
        /// </summary>
        /// <param name="streakArray">Array to which the current streak should be added</param>
        private void WriteStreak(int[,] streakArray)
        {
            if (_curStreakLen > 0)
            {
                if (_curStreakLen < _MAXSTREAKLEN)
                {
                    streakArray[(int)_curStreakDir, _curStreakLen - 1]++;
                }
                else
                {
                    streakArray[(int)_curStreakDir, _MAXSTREAKLEN - 1]++;
                }
            }
        }

        /// <summary>
        /// Outputs a summary of statistics for streaks.
        /// </summary>
        /// <param name="score">Score object containing aggregate win/loss/push info</param>
        /// <returns>Printable string describing streak statistics</returns>
        public string StreakStats(Score score)
        {
            StringBuilder sb = new StringBuilder();
            int[,] streaks = this.Streaks;
            foreach (GameResult direction in Enum.GetValues(typeof(GameResult)))
            {
                sb.AppendFormat("\r\nStreak frequency where player {0}:\r\n", direction);
                for (int i = 0; i < _MAXSTREAKLEN; i++)
                {
                    if (streaks[(int)direction, i] > 0)
                    {
                        double Tally = 0.0;
                        switch(direction){
                            case GameResult.Lost:
                                Tally = score.Lost;
                                break;
                            case GameResult.Pushed:
                                Tally = score.Push;
                                break;
                            case GameResult.Won:
                                Tally = score.Won;
                                break;
                            default:
                                break;
                        }

                        sb.AppendFormat("  Length {0}: {1} ({2:P})\r\n", 
                            i + 1, 
                            streaks[(int)direction, i],
                            (double)streaks[(int)direction, i] * (double)(i + 1) / Tally
                            );
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generates a text header suitable for dumping to a CSV file.
        /// </summary>
        /// <returns>Text header for CSV</returns>
        public virtual string CSVHeader()
        {
            int longestStreak = LongestStreak();
            string[] LengthHeader = new string[longestStreak];
            for (int i = 0; i < longestStreak; i++)
            {
                LengthHeader[i] = String.Format("Length {0}", i + 1);
            }
            return String.Join(",", "Result", String.Join(",", LengthHeader));
        }

        /// <summary>
        /// Generates a CSV record summarizing this scoreboard.
        /// </summary>
        /// <returns>Text record for CSV</returns>
        public virtual string CSVString()
        {
            int longestStreak = LongestStreak();
            string CSVOutput = "";
            for (int i = 0; i < Enum.GetValues(typeof(GameResult)).Length; i++)
            {
                string[] CSVLine = new string[longestStreak];
                for (int j = 0; j < longestStreak; j++)
                {
                    CSVLine[j] = _streaks[i, j].ToString();
                }
                CSVOutput += String.Join(",", Enum.GetName(typeof(GameResult), i),
                    String.Join(",", CSVLine)) + "\r\n";
            }
            return CSVOutput;
        }
        
        /// <summary>
        /// Returns the longest streak listed in the table.  Needed to generate CSV reports.
        /// </summary>
        /// <returns></returns>
        protected int LongestStreak()
        {
            int longestStreak = 0;
            for (int j = _MAXSTREAKLEN - 1; j >= 0 && longestStreak == 0; j--)
            {
                for (int i = 0; i < Enum.GetValues(typeof(GameResult)).Length; i++)
                {
                    if (_streaks[i, j] > 0) { longestStreak = j + 1; }
                }
            }
            return longestStreak;
        }
    }
}
