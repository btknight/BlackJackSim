using BlackJackSim;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BlackJackCon
{
    class Program
    {
        /// <summary>
        /// The console program.  Stakes seven players with 100,000 chips each (or $1,000.00 if you like), and 
        /// plays blackjack until all but the card counters lose their entire stack.
        /// </summary>
        /// <param name="args">(optional) Filename of a CSV file - one line is output after every game</param>
        static int Main(string[] args)
        {
            Console.WriteLine("{0} - version {1}",
                Assembly.GetExecutingAssembly().GetName().Name,
                Assembly.GetExecutingAssembly().GetName().Version
                );

            // Initialize the blackjack table.
            TableRules Rules = new TableRules(500, -1, 8, 8, 4, 0.10, 0.15);
            int InitialStake = 100000;

            Player[] Players = new Player[7];
            Players[0] = new WikiPlayer(new ChipStack(InitialStake));
            Players[1] = new DarwinCCPlayer(new ChipStack(InitialStake));
            Players[2] = new PositiveProgDarwinPlayer(new ChipStack(InitialStake));
            Players[3] = new NoBustPlayer(new ChipStack(InitialStake));
            Players[4] = new Dealer(new ChipStack(InitialStake));
            for (int i = 3; i < Players.Length; i++)
            {
                Players[i] = new DarwinPlayer(new ChipStack(InitialStake));
            }
            Table table = new Table(Rules, Players);

            bool[] PlayerBankrupt = new bool[Players.Length];
            int GamesPlayed = 0;
            Stopwatch Timer = new Stopwatch();

            DateTime LastShowedProgress = DateTime.Now;
            DateTime LastShowedStats = DateTime.Now;

            StreamWriter CSVWriter = null;

            // Consider the DarwinCCPlayer bankrupt, so as to end the processing without letting it go on and on.
            PlayerBankrupt[1] = true;

            if (args.Length == 1)
            {
                FileInfo FI = new FileInfo(args[0]);
                try
                {
                    CSVWriter = FI.CreateText();
                }
                catch (IOException e)
                {
                    Console.Error.WriteLine("Error: Could not open {0} for write: {1}", args[0], e.Message);
                    return -1;
                }
                string CSVHeader = String.Join(",", "Games Played", "Time in Play (ticks)", table.CSVHeader());
                CSVWriter.WriteLine(CSVHeader);
            }
            
            Predicate<bool[]> AllPlayersBusted = (x) =>
            {
                bool Busted = true;
                for (int i = 0; i < x.Length; i++)
                {
                    Busted = Busted && x[i];
                }
                return Busted;
            };

            Console.WriteLine("Starting blackjack games");

            // Go play, as long as players still have money.
            while (! AllPlayersBusted(PlayerBankrupt))
            {
                Timer.Start();
                table.Play();
                Timer.Stop();
                GamesPlayed++;
                Debug.WriteLine(String.Format("Round {0} played", GamesPlayed));

                // See who is bankrupt.  Fill in the PlayerBankrupt[] array.
                for (int i = 0; i < table.Players.Length; i++)
                {
                    if (PlayerBankrupt[i] == false &&
                        table.Players[i].CashOnHand < table.Rules.MinimumBet)
                    {
                        Console.Write("\r\n");
                        Console.WriteLine("Player {0} bankrupt after {1} games played", i, GamesPlayed);
                        PlayerBankrupt[i] = true;
                    }
                }

                // Output statistics, if needed.
                if (CSVWriter != null)
                {
                    string CSVLine = String.Join(",", GamesPlayed, Timer.ElapsedTicks, table.CSVString());
                    CSVWriter.WriteLine(CSVLine);
                }

                // Give the user some feedback to let them know the system is still working.
                if (GamesPlayed % 500 == 0 && (DateTime.Now - LastShowedProgress).Milliseconds > 100)
                {
                    Console.Write("Games Played: {0}\r", GamesPlayed);
                    if (CSVWriter != null) { CSVWriter.Flush(); }
                    LastShowedProgress = DateTime.Now;
                }
                if (GamesPlayed % 5000 == 0 && (DateTime.Now - LastShowedStats).Seconds > 1)
                {
                    Console.Write("\r\n");
                    Console.WriteLine(table.Scoreboard());
                    LastShowedStats = DateTime.Now;
                }
            }

            Console.Write("---ALL PLAYERS BANKRUPT---\r\nStatistics for this run:\r\n");
            Console.WriteLine(table.Scoreboard());

            if (CSVWriter != null)
            {
                ScorePerHand Aggregate = new ScorePerHand();
                foreach (Player player in table.Players)
                {
                    Aggregate += player.Score;
                }
                CSVWriter.WriteLine(Aggregate.PerHandCSVHeader());
                CSVWriter.WriteLine(Aggregate.PerHandCSVString());
                CSVWriter.WriteLine(Aggregate.StreakCSVHeader());
                CSVWriter.WriteLine(Aggregate.StreakCSVString());
                CSVWriter.Close();
            }

            ScorePerHand SavedScore;
            FileStream ScoreFileSR;
            try
            {
                ScoreFileSR = new FileStream(@"d:\users\brian\documents\ScorePerHand.bin", FileMode.Open);
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    SavedScore = (ScorePerHand)bf.Deserialize(ScoreFileSR);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to read aggregate scores: Serialization error: {0}", e.Message);
                    SavedScore = new ScorePerHand();
                }
                ScoreFileSR.Close();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Failed to read aggregate scores: File error: {0}", e.Message);
                SavedScore = new ScorePerHand();
            }

            for (int i = 0; i < table.Players.Length; i++)
            {
                try
                {
                    SavedScore += table.Players[i].Score;
                }
                catch (OverflowException e)
                {
                    Console.WriteLine("Failed to add score: {0}", e.Message);
                }
            }

            try
            {
                ScoreFileSR = new FileStream(@"d:\users\brian\documents\ScorePerHand.bin", FileMode.OpenOrCreate);
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ScoreFileSR, SavedScore);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to save aggregate scores: Serialization error: {0}", e.Message);
                }
                ScoreFileSR.Close();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Failed to save aggregate scores: File error: {0}", e.Message);
            }

            Console.Write("\r\n\r\nStatistics for all runs:\r\n");
            Console.WriteLine(SavedScore.Scoreboard());
            /* "In the casino, the cardinal rule is to keep them playing and to keep them coming back. 
             * The longer they play, the more they lose, and in the end, we get it all."
             */
#if DEBUG
            Console.WriteLine("Hit any key to quit");
            Console.ReadKey();
#endif
            return 0;
        }
    }
}
