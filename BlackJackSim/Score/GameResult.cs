using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJackSim
{
    /// <summary>
    /// Result of a game.  Encodes result as an integer in order to add values to a tracking matrix.
    /// </summary>
    public enum GameResult
    {
        Lost = 0,
        Pushed = 1,
        Won = 2
    }
}
