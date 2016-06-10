using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    /// <summary>
    /// Interface for scorekeeping objects.
    /// </summary>
    public interface IScore
    {
        /// <summary>
        /// Add one result to the tally.
        /// </summary>
        /// <param name="Result">GameResult object describing outcome of the game</param>
        /// <param name="UpCard">Dealer up card</param>
        /// <param name="HandValue">Numeric value of the player hand at the start of the game</param>
        /// <param name="IsSoft">Whether the player hand was soft</param>
        void IncrementScore(GameResult Result, Card UpCard, Hand PlayerHand);

        /// <summary>
        /// Generates a printable string suitable for console displays.
        /// </summary>
        /// <returns>Text summary of stats</returns>
        string Scoreboard();
        
        /// <summary>
        /// Generates a text header suitable for dumping to a CSV file.
        /// </summary>
        /// <returns>Text header for CSV</returns>
        string CSVHeader();

        /// <summary>
        /// Generates a CSV record summarizing this scoreboard.
        /// </summary>
        /// <returns>Text record for CSV</returns>
        string CSVString();

    }
}
