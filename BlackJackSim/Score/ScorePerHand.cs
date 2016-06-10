using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    /// <summary>
    /// Keeps win / loss / push scores for individual hands.  Uses Score class to track.
    /// </summary>
    [Serializable]
    public class ScorePerHand : Score
    {
        protected Score[,] _hardHandScore;
        protected Score[,] _softHandScore;

        public ScorePerHand() : base()
        {
            // Hard hands range in value from 4 (pair of two's) to 20.
            // The matrix is therefore length 16.  Any hand value must have 4 subtracted before
            // looking up its score table in the matrix.
            _hardHandScore = new Score[10, 17];
            // Soft hands range in value from 12 (pair of aces) to 21.
            // The matrix is length 10.  Any hand value must have 12 subtracted before lookup.
            _softHandScore = new Score[10, 10];

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    _hardHandScore[i, j] = new Score();
                    if (j < 10) { _softHandScore[i, j] = new Score(); }
                }
            }
        }

        /// <summary>
        /// Special secret constructor for scoreboard.  Permits setting all elements directly.  Supports
        /// addition operator.
        /// </summary>
        /// <param name="results"><see cref="_results"/></param>
        /// <param name="streak"><see cref="Streak"/></param>
        protected ScorePerHand(Score[,] HardHandScore, Score[,] SoftHandScore, int[] results, ScoreStreak streak)
            : base(results, streak)
        {
            // Hard hands range in value from 4 (pair of two's) to 20.
            // The matrix is therefore length 16.  Any hand value must have 4 subtracted before
            // looking up its score table in the matrix.
            _hardHandScore = new Score[10, 17];
            // Soft hands range in value from 12 (pair of aces) to 21.
            // The matrix is length 10.  Any hand value must have 12 subtracted before lookup.
            _softHandScore = new Score[10, 10];
            if (HardHandScore.GetLength(0) != 10 || HardHandScore.GetLength(1) != 17)
            {
                throw new Exception("ScorePerHand() was fed a hard hand tracking array of unexpected size");
            }
            if (SoftHandScore.GetLength(0) != 10 || SoftHandScore.GetLength(1) != 10)
            {
                throw new Exception("ScorePerHand() was fed a soft hand tracking array of unexpected size");
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    _hardHandScore[i, j] = HardHandScore[i,j];
                    if (j < 10) { _softHandScore[i, j] = SoftHandScore[i,j]; }
                }
            }
        }

        public override void IncrementScore(GameResult Result, Card UpCard, Hand PlayerHand)
        {
            int UpCardIdx = UpCard.Value - 1;
            if (PlayerHand.Value == 21 && Result == GameResult.Lost)
            {
                Console.WriteLine("A 21 lost against the dealer?  Wait what?");
            }
            if (PlayerHand.IsSoftPrePlay)
            {
                _softHandScore[UpCardIdx, PlayerHand.ValuePrePlay - 12].IncrementScore(Result, UpCard, PlayerHand);
            }
            else
            {
                _hardHandScore[UpCardIdx, PlayerHand.ValuePrePlay - 4].IncrementScore(Result, UpCard, PlayerHand);
            }
            base.IncrementScore(Result, UpCard, PlayerHand);
        }

        public static ScorePerHand operator +(ScorePerHand score1, ScorePerHand score2)
        {
            Score[,] addHardHandMatrix = new Score[10, 17];
            Score[,] addSoftHandMatrix = new Score[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    addHardHandMatrix[i, j] = score1._hardHandScore[i, j] + score2._hardHandScore[i, j];
                    if (j < 10) { addSoftHandMatrix[i, j] = score1._softHandScore[i, j] + score2._softHandScore[i, j]; }
                }
            }
            int[] addResultsMatrix = new int[Enum.GetValues(typeof(GameResult)).Length];
            for (int i = 0; i < Enum.GetValues(typeof(GameResult)).Length; i++)
            {
                addResultsMatrix[i] = score1._results[i] + score2._results[i];
            }
            return new ScorePerHand(addHardHandMatrix, addSoftHandMatrix, addResultsMatrix, score1.Streak + score2.Streak);
        }
        public string PerHandCSVHeader()
        {
            return String.Join(",", "Player Soft/Hard Hand", "Up Card Value", "Player Hand Value", 
                base.CSVHeader());
        }
        public string PerHandCSVString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    sb.AppendLine(String.Join(",", "Hard", (i + 1).ToString(), (j + 4).ToString(),
                        _hardHandScore[i, j].CSVString()));
                }
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    sb.AppendLine(String.Join(",", "Soft", (i + 1).ToString(), (j + 12).ToString(),
                        _softHandScore[i, j].CSVString()));
                }
            }
            return sb.ToString();
        }
    }
}
