using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brackets1
{
    /**
     * Brackets:
     * 1. Players must be able to divide into teams of eight
     *    for each bracket competition.
     */
    public static class Program
    {
        private const int GAME_1 = 0;
        private const int GAME_2 = 1;
        private const int GAME_3 = 2;
        private const int TIER_1_COUNT = 8;
        private const int TIER_2_COUNT = 12;
        private const int TIER_3_COUNT = 14;
        private const int TIER_1_START = 0;
        private const int TIER_2_START = 8;
        private const int TIER_3_START = 12;
        private const int TIER_3_END = 13;

        private static List<Player> Players { get; set; }
        private static List<Game> Bracket { get; set; }
        private static string SecondPlace { get; set; }
        private static int BracketCount { get; set; }

        public static void Main(string[] args)
        {            
            Open();

            if (IsDivisibleByEight())
            {
                Console.Write("How many brackets would you like? ");
                int count = Convert.ToInt32(Console.ReadLine());
                BracketCount = 1;
                for (int i = 0; i < count; i++)
                {
                    Execute();
                    ++BracketCount;
                }
            }
            else
            {
                Console.WriteLine("Players must be evenly matched " +
                    "and have at least eight players to compete.");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void Execute()
        {            
            Shuffle();
            Play();
            Print();
        }

        private static string GetPath()
        {
            // requires that file being read is in the Documents folder
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(path, @"tournament.txt");
        }

        private static void Open()
        {
            Players = new List<Player>();                

            try
            {
                using (StreamReader reader = new StreamReader(GetPath()))
                {
                    List<string> lines = new List<string>();
                    bool hasNextLine = reader.Peek() > -1;
                        
                    if (hasNextLine)
                    {
                        while (hasNextLine)
                        {
                            lines.Add(reader.ReadLine());
                            hasNextLine = reader.Peek() > -1;
                        }

                        foreach (string line in lines)
                        {
                            List<string> tokens = line.Split(' ').ToList();
                            List<int> scores = new List<int>();

                            for (int i = 1; i < tokens.Count; i++)
                            {
                                scores.Add(Convert.ToInt32(tokens[i]));
                            }

                            Player member = new Player()
                            {
                                Name = tokens[0],
                                Scores = scores
                            };
                            Players.Add(member);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be found.");
                Console.WriteLine(e.Message);
            }          
        }

        private static bool IsDivisibleByEight()
        {
            return Players.Count >= 8 && Players.Count % 8 == 0;
        }

        private static void Shuffle()
        {
            Random r = new Random();
            int count = Players.Count;
            int set = Players.Count - 1;

            for (int i = 0; i < count; i++)
            {
                int index = r.Next(0, set);
                Swap(i, index);
            }            
        }

        private static void Swap(int i, int index)
        {
            Player temp = Players[i];
            Players[i] = Players[index];
            Players[index] = temp;
        }

        private static void Play()
        {
            Bracket = new List<Game>();
            
            int tier = 1;
            for (int i = 0; i < TIER_1_COUNT; i++)
            {
                Game game = new Game()
                {
                    Name = Players[i].Name,
                    PlayerIndex = i,
                    Score = Players[i].Scores[GAME_1],
                    Tier = tier
                };
                Bracket.Add(game);
            }

            tier++;
            Compare(TIER_1_START, TIER_1_COUNT, GAME_2, tier);

            tier++;
            Compare(TIER_2_START, TIER_2_COUNT, GAME_3, tier);

            tier++;
            Compare(TIER_3_START, TIER_3_COUNT, GAME_3, tier);
        }

        private static void Compare(int start, 
                                    int count, 
                                    int scoreIndex, 
                                    int tier)
        {            
            for (int i = start; i < count; i += 2)
            {
                if (Bracket[i].Score > Bracket[i + 1].Score)
                {                    
                    int playerIndex = Bracket[i].PlayerIndex;
                    Game game = new Game()
                    {
                        Name = Bracket[i].Name,
                        PlayerIndex = playerIndex,
                        Score = Players[playerIndex].Scores[scoreIndex],
                        Tier = tier
                    };
                    Bracket.Add(game);
                    if (i == TIER_3_START || i == TIER_3_END)
                        SecondPlace = Bracket[i + 1].Name;
                }
                else
                {
                    int playerIndex = Bracket[i + 1].PlayerIndex;
                    Game game = new Game()
                    {
                        Name = Bracket[i + 1].Name,
                        PlayerIndex = playerIndex,
                        Score = Players[playerIndex].Scores[scoreIndex],
                        Tier = tier
                    };
                    Bracket.Add(game);
                    if (i == TIER_3_START || i == TIER_3_END)
                        SecondPlace = Bracket[i].Name;
                }
            }
        }

        private static void Print()
        {
            Console.WriteLine();
            Console.WriteLine("BRACKET #" + BracketCount + " ----------" +
                "-------------------------------------");
            Console.WriteLine("Game 1       Game 2       Game 3       " +
                "Winner     Runner-Up");
            Console.WriteLine();
            Console.WriteLine(Bracket[0].Name + " " + Bracket[0].Score);
            Console.WriteLine("          >--" + Bracket[8].Name + " " +
                Bracket[8].Score);
            Console.WriteLine(Bracket[1].Name + " " + Bracket[1].Score);
            Console.WriteLine("                       >--" + 
                Bracket[12].Name + " " + Bracket[12].Score);
            Console.WriteLine(Bracket[2].Name + " " + Bracket[2].Score);
            Console.WriteLine("          >--" + Bracket[9].Name + " " +
                Bracket[9].Score);
            Console.WriteLine(Bracket[3].Name + " " + Bracket[3].Score +
                "                             >--" + Bracket[14].Name + 
                " $25 / " + SecondPlace + " $10");
            Console.WriteLine();
            Console.WriteLine(Bracket[4].Name + " " + Bracket[4].Score);
            Console.WriteLine("          >--" + Bracket[10].Name + " " +
                Bracket[10].Score);
            Console.WriteLine(Bracket[5].Name + " " + Bracket[5].Score);
            Console.WriteLine("                       >--" + 
                Bracket[13].Name + " " + Bracket[13].Score);
            Console.WriteLine(Bracket[6].Name + " " + Bracket[6].Score);
            Console.WriteLine("          >--" + Bracket[11].Name + " " +
                Bracket[11].Score);
            Console.WriteLine(Bracket[7].Name + " " + Bracket[7].Score);
        }
    }
}
