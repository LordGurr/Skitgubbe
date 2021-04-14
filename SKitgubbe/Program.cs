using System;
using System.Collections.Generic;
using System.Linq;

namespace SKitgubbe
{
    public enum Suit { Hjärter, Ruter, Spader, Klöver, Wild };

    internal class Program
    {
        private static void Main(string[] args)
        {
            List<Player> players = new List<Player>();
            players.Add(new HonestPlayer());
            players.Add(new RandomPlayer());
            players.Add(new MyPlayer());

            Console.WriteLine("Hur många spelare ska mötas?");
            string temp = Console.ReadLine();
            int antalSpelare = int.Parse(IsDigitsOnly(temp, "") ? temp : "2");
            if (antalSpelare < 2 || antalSpelare > 5)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                int length = antalSpelare.ToString().Length;
                antalSpelare = Math.Clamp(antalSpelare, 2, 5);
                Console.Write(antalSpelare);
                for (int a = 0; a < length; a++)
                {
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.WriteLine("Vilka spelare skall mötas?");
            for (int i = 1; i <= players.Count; i++)
            {
                Console.WriteLine(i + ": {0}", players[i - 1].Name);
            }
            int[] spelarIndex = new int[antalSpelare];
            for (int i = 0; i < spelarIndex.Length; i++)
            {
                temp = Console.ReadLine();
                spelarIndex[i] = int.Parse(IsDigitsOnly(temp, "") ? temp : "1");
                if (spelarIndex[i] < 0 || spelarIndex[i] > players.Count)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    int length = spelarIndex[i].ToString().Length;
                    spelarIndex[i] = Math.Clamp(spelarIndex[i], 1, players.Count);
                    Console.Write(spelarIndex[i]);
                    for (int a = 0; a < length; a++)
                    {
                        Console.Write(" ");
                    }
                    Console.WriteLine();
                }
            }
            //int p1 = int.Parse(Console.ReadLine());
            //int p2 = int.Parse(Console.ReadLine());
            Player[] spelarna = new Player[antalSpelare];
            for (int i = 0; i < spelarna.Length; i++)
            {
                for (int a = 0; a < spelarIndex.Length; a++)
                {
                    if (a != i)
                    {
                        if (spelarIndex[a] == spelarIndex[i])
                        {
                            if (spelarIndex[a] == 1)
                            {
                                spelarna[i] = new HonestPlayer();
                            }
                            else if (spelarIndex[a] == 2)
                            {
                                spelarna[i] = new RandomPlayer();
                            }
                            else if (spelarIndex[a] == 3)
                            {
                                spelarna[i] = new MyPlayer();
                            }
                            break;
                        }
                    }
                }
                if (spelarna[i] == null)
                {
                    spelarna[i] = players[spelarIndex[i] - 1];
                }
            }
            Console.WriteLine("Hur många spel skall spelas?");
            temp = Console.ReadLine();
            int numberOfGames = int.Parse(IsDigitsOnly(temp, "") ? temp : "2000");
            Console.WriteLine("Vilket spel skall skrivas ut? (0 = Inget spel)");
            temp = Console.ReadLine();
            int gameToPrint = int.Parse(IsDigitsOnly(temp, "") ? temp : "0");
            //Console.WriteLine("Skriva ut första spelet? (y/n)");
            //string print = Console.ReadLine();
            Console.CursorVisible = false;
            PlayerInformation[] spelarInformation = new PlayerInformation[spelarna.Length];
            for (int i = 0; i < spelarna.Length; i++)
            {
                spelarInformation[i] = new PlayerInformation(spelarna[i], new List<Card>(), i);
            }
            Game game = new Game();
            int spelLika = 0;
            int sammanlagdaRunder = 0;
            DateTime dateTime = DateTime.Now;
            TimeSpan timeSpanPerGame = dateTime - dateTime;
            DateTime startTid = DateTime.Now;
            TimeSpan tidSomGått = DateTime.Now - startTid;

            for (int i = 0; i < numberOfGames; i++)
            {
                PlayerFunctions.Reason = "";
                dateTime = DateTime.Now;
                int[] stats = game.PlayAGame(spelarInformation, gameToPrint == i + 1);
                timeSpanPerGame += DateTime.Now - dateTime;
                tidSomGått = DateTime.Now - startTid;
                int win = stats[0];
                if (win >= 0)
                {
                    for (int a = 0; a < spelarInformation.Length; a++)
                    {
                        if (win == spelarInformation[a].index)
                        {
                            spelarInformation[a].wins++;
                            break;
                        }
                    }
                }
                else
                {
                    spelLika++;
                }
                sammanlagdaRunder += stats[1];
                OnUi(spelarInformation, numberOfGames, i + 1, Convert.ToInt32((double)sammanlagdaRunder / ((double)i + 1)), spelLika, gameToPrint == i + 1, timeSpanPerGame.TotalMilliseconds / ((double)i + 1), tidSomGått.TotalSeconds);
            }
            Console.ReadKey(true);
            Console.ReadKey(true);
        }

        private static void OnUi(PlayerInformation[] spelarna, int numberOfGames, int currentGame, int averageSpelLängd, int spelLika, bool printGame, double timeSpanPerGame, double timePassed)
        {
            int offset = 3;
            spelarna = SortPlayer(spelarna);
            if (currentGame < 2 || printGame)
            {
                Console.Clear();
                Console.CursorVisible = false;
                Console.ForegroundColor = ConsoleColor.White;
                for (int i = 0; i < spelarna.Length; i++)
                {
                    Console.SetCursorPosition(1, offset);
                    Console.Write(spelarna[i].player.Name + ":");
                    offset += 2;
                }
                Console.ForegroundColor = ConsoleColor.White;
                offset = 25;
                for (int i = 0; i < spelarna.Length; i++)
                {
                    Console.SetCursorPosition(offset, spelarna.Length * 2 + 4);
                    Console.Write(spelarna[i].player.Name);
                    offset += 15;
                }
                Console.SetCursorPosition(0, spelarna.Length * 2 + 5);
                Console.WriteLine("          Vunna spel:");
                Console.WriteLine("            Skillnad:");
                Console.WriteLine("Antal ronder i snitt:");
                Console.WriteLine("          Andel pass:");
                Console.WriteLine("   Andel specialkort:");
                Console.WriteLine("  Procent spel vunna:");
                Console.WriteLine("   Procent spel lika:");
                Console.WriteLine("        Spel spelade:");
                Console.WriteLine("        Tid per spel:");
                Console.WriteLine("        Tid som gått:");
                int Themitten = (spelarna.Length / 2) > 0 ? (spelarna.Length / 2) : 2;
                if (spelarna.Length % 2 == 0)
                {
                    Console.SetCursorPosition(25 + (15 * Themitten) - 5, spelarna.Length * 2 + 12);
                }
                else
                {
                    Console.SetCursorPosition(25 + (15 * Themitten), spelarna.Length * 2 + 12);
                }
                Console.Write("/" + numberOfGames);
                offset = 3;
                for (int i = 0; i < spelarna.Length; i++)
                {
                    if (spelarna[i].wins > 0)
                    {
                        Console.SetCursorPosition(Convert.ToInt32(19), offset);
                        Console.ForegroundColor = (ConsoleColor)i + 9;
                        for (int a = 0; a < ((double)spelarna[i].wins / (double)numberOfGames) * 100; a++)
                        {
                            Console.Write("█");
                        }
                    }
                    offset += 2;
                }
            }
            offset = 3;
            if (numberOfGames >= 100)
            {
                for (int i = 0; i < spelarna.Length; i++)
                {
                    if (spelarna[i].wins > 0)
                    {
                        Console.SetCursorPosition(Convert.ToInt32(19 + ((double)spelarna[i].wins / (double)numberOfGames) * 100), offset);
                        Console.ForegroundColor = (ConsoleColor)i + 9;
                        //for (int a = 0; a < ((double)spelarna[i].wins / (double)numberOfGames) * 100; a++)
                        //{
                        Console.Write("█");
                        // }
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(spelarna[i].wins);
                    }
                    offset += 2;
                }
            }
            else
            {
                for (int i = 0; i < spelarna.Length; i++)
                {
                    if (spelarna[i].wins > 0)
                    {
                        Console.SetCursorPosition(Convert.ToInt32(19), offset);
                        Console.ForegroundColor = (ConsoleColor)i + 9;
                        for (int a = 0; a < ((double)spelarna[i].wins / (double)numberOfGames) * 100; a++)
                        {
                            Console.Write("█");
                        }
                    }
                    offset += 2;
                }
            }
            int yOffset = 5;
            Console.ForegroundColor = ConsoleColor.White;
            int min = Min(spelarna);
            int mitten = (spelarna.Length / 2) > 0 ? (spelarna.Length / 2) : 2;

            offset = 25;
            for (int a = 0; a < spelarna.Length; a++)
            {
                yOffset = 5;
                Console.SetCursorPosition(offset, spelarna.Length * 2 + yOffset);

                Console.Write(spelarna[a].wins);
                yOffset++;
                if (spelarna[a].wins - min > 0)
                {
                    Console.SetCursorPosition(offset, spelarna.Length * 2 + yOffset);
                    Console.Write("+" + (spelarna[a].wins - min) + "  ");
                }
                else
                {
                    Console.SetCursorPosition(offset, spelarna.Length * 2 + yOffset);
                    Console.Write("        ");
                }
                yOffset++;
                if (mitten == a)
                {
                    if (spelarna.Length % 2 == 0)
                    {
                        Console.SetCursorPosition(offset - 6, spelarna.Length * 2 + yOffset);
                    }
                    else
                    {
                        Console.SetCursorPosition(offset, spelarna.Length * 2 + yOffset);
                    }
                    Console.Write(averageSpelLängd);
                }
                yOffset++;
                Console.SetCursorPosition(offset, spelarna.Length * 2 + yOffset);

                double andelPass = (double)spelarna[a].antalPass * 100 / ((double)averageSpelLängd * (double)currentGame);
                Console.Write(andelPass.ToString("F") + "%");
                yOffset++;
                Console.SetCursorPosition(offset, spelarna.Length * 2 + yOffset);
                double andelSpecialkort = (double)spelarna[a].antalSpecialkort * 100 / ((double)averageSpelLängd * (double)currentGame);
                Console.Write(andelSpecialkort.ToString("F") + "%");
                yOffset++;
                Console.SetCursorPosition(offset, spelarna.Length * 2 + yOffset);

                double andelvinster = (double)spelarna[a].wins * 100 / (double)currentGame;
                Console.Write(andelvinster.ToString("F") + "%");
                yOffset++;
                if (mitten == a)
                {
                    if (spelarna.Length % 2 == 0)
                    {
                        Console.SetCursorPosition(offset - 8, spelarna.Length * 2 + yOffset);
                    }
                    else
                    {
                        Console.SetCursorPosition(offset, spelarna.Length * 2 + yOffset);
                    }
                    double procentSpelLika = (double)spelLika * 100 / (double)currentGame;
                    Console.Write(procentSpelLika.ToString("F") + "%");
                }
                yOffset++;
                if (mitten == a)
                {
                    if (spelarna.Length % 2 == 0)
                    {
                        Console.SetCursorPosition(offset - currentGame.ToString().Length - 5, spelarna.Length * 2 + yOffset);
                    }
                    else
                    {
                        Console.SetCursorPosition(offset - currentGame.ToString().Length, spelarna.Length * 2 + yOffset);
                    }
                    Console.Write(currentGame);
                }
                yOffset++;
                if (mitten == a)
                {
                    if (spelarna.Length % 2 == 0)
                    {
                        Console.SetCursorPosition(offset - 8, spelarna.Length * 2 + yOffset);
                    }
                    else
                    {
                        Console.SetCursorPosition(offset, spelarna.Length * 2 + yOffset);
                    }
                    double tidSomGåttMilli = Convert.ToInt32(timeSpanPerGame * 100) / (double)100;
                    Console.Write(tidSomGåttMilli + " milli    ");
                }
                yOffset++;
                if (mitten == a)
                {
                    if (spelarna.Length % 2 == 0)
                    {
                        Console.SetCursorPosition(offset - 8, spelarna.Length * 2 + yOffset);
                    }
                    else
                    {
                        Console.SetCursorPosition(offset, spelarna.Length * 2 + yOffset);
                    }
                    double tidSomGåttSekund = Convert.ToInt32(timePassed * 100) / (double)100;
                    Console.Write(tidSomGåttSekund + " sek    ");
                }
                offset += 15;
            }

            //Console.ReadKey(true);
        }

        private static int Min(PlayerInformation[] spelarna)
        {
            int min = int.MaxValue;
            for (int i = 0; i < spelarna.Length; i++)
            {
                if (spelarna[i].wins < min)
                {
                    min = spelarna[i].wins;
                }
            }
            return min;
        }

        private static bool IsDigitsOnly(string str, string operators) //Den här kollar så att det bara finns nummer eller mellanslag i passwordet. Om inte för denna så skulle spelet krasha om du skrev en bokstav.
        {
            bool containsNumber = false;
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                {
                    if (c != ' ')
                    {
                        bool found = false;
                        foreach (char character in operators)
                        {
                            if (c == character)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            return false;
                        }
                    }
                }
                else if (!containsNumber && c > '0' && c < '9')
                {
                    containsNumber = true;
                }
            }
            if (!containsNumber || str.Length > 6)
            {
                return false;
            }
            return true;
        }

        private static PlayerInformation[] SortPlayer(PlayerInformation[] spelarna) // testing
        {
            return spelarna.OrderBy(o => o.index).ToArray();
        }

        internal class Game
        {
            private Random RNG = new Random();
            private static List<Card> CardDeck;
            private static List<Card> slängHögen = new List<Card>();
            private static bool wildcard;

            public Game()
            {
            }

            public int[] PlayAGame(PlayerInformation[] spelarna, bool printGame)
            {
                if (printGame)
                {
                    Console.Clear();
                }
                CardDeck = new List<Card>();
                wildcard = false;
                int value;
                int suit;
                CardDeck.Clear();
                slängHögen.Clear();
                for (int i = 0; i < 52; i++)
                {
                    value = i % 13 + 2;
                    suit = i % 4;
                    CardDeck.Add(new Card(value, (Suit)suit));
                }
                Shuffle();
                spelarna = ShufflePlayers(spelarna);
                for (int i = 0; i < spelarna.Length; i++)
                {
                    spelarna[i].player.Hand.Clear();
                    for (int a = 0; a < 3; a++)
                    {
                        spelarna[i].player.Hand.Add(CardDeck[0]);
                        CardDeck.RemoveAt(0);
                    }
                }
                for (int i = 0; i < spelarna.Length; i++)
                {
                    spelarna[i].player.VisibleHand.Clear();
                    for (int a = 0; a < 3; a++)
                    {
                        spelarna[i].player.VisibleHand.Add(CardDeck[0]);
                        CardDeck.RemoveAt(0);
                    }
                }
                //for (int i = 0; i < invisibleCards.Length; i++)
                //{
                //    invisibleCards[i] = new List<Card>();
                //}
                for (int i = 0; i < spelarna.Length; i++)
                {
                    spelarna[i].invisibleCards.Clear();
                    for (int a = 0; a < 3; a++)
                    {
                        spelarna[i].invisibleCards.Add(CardDeck[0]);
                        CardDeck.RemoveAt(0);
                    }
                }
                slängHögen.Add(CardDeck[0]);
                CardDeck.RemoveAt(0);
                int runderSpelade = 0;
                while (SpeletInteSlut(spelarna))
                {
                    PlayARound(spelarna, slängHögen, printGame);
                    runderSpelade++;
                    if (runderSpelade > 256)
                    {
                        break;
                    }
                }
                spelarna = SortPlayer(spelarna);
                for (int i = 0; i < spelarna.Length; i++)
                {
                    if (spelarna[i].invisibleCards.Count < 1)
                    {
                        if (printGame)
                        {
                            List<Card> senasteKortet = new List<Card>();
                            senasteKortet.Add(slängHögen[slängHögen.Count - 1]);
                            OnGameUi(spelarna[i], slängHögen[slängHögen.Count - 2] != null ? slängHögen[slängHögen.Count - 2] : new Card(2, Suit.Wild), senasteKortet, 0);
                        }
                        return new int[2] { spelarna[i].index, runderSpelade };
                    }
                }
                return new int[2] { -1, runderSpelade };
            }

            private bool SpeletInteSlut(PlayerInformation[] spelarna)
            {
                for (int i = 0; i < spelarna.Length; i++)
                {
                    if (spelarna[i].player.Hand.Count < 1)
                    {
                        if (spelarna[i].player.VisibleHand.Count < 1)
                        {
                            if (spelarna[i].invisibleCards.Count < 1)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }

            private void PlayARound(PlayerInformation[] spelarna, List<Card> slängHögen, bool printGame)
            {
                for (int i = 0; i < spelarna.Length; i++)
                {
                    Card senasteKortet = new Card(2, Suit.Hjärter);
                    List<Card> speladeKort = new List<Card>();
                    Card spelarensKort = new Card(2, Suit.Hjärter);
                    int kortSomTasUpp = 0;
                    if (printGame)
                    {
                        senasteKortet = new Card(wildcard ? 2 : slängHögen[slängHögen.Count - 1].Value, wildcard ? Suit.Wild : slängHögen[slängHögen.Count - 1].Suit);
                    }
                    while (spelarensKort.Value == 2 || spelarensKort.Value == 10)
                    {
                        if (spelarna[i].player.Hand.Count > 0 || CardDeck.Count > 0)
                        {
                            if (spelarna[i].player.Hand.Count < 1)
                            {
                                while (spelarna[i].player.Hand.Count < 3 && CardDeck.Count > 0)
                                {
                                    spelarna[i].player.Hand.Add(CardDeck[0]);
                                    CardDeck.RemoveAt(0);
                                }
                            }
                            spelarensKort = spelarna[i].player.LäggEttKort(wildcard ? 2 : slängHögen[slängHögen.Count - 1].Value, wildcard ? Suit.Wild : slängHögen[slängHögen.Count - 1].Suit);
                            if (spelarensKort == null)
                            {
                                if (printGame)
                                {
                                    kortSomTasUpp += slängHögen.Count;
                                }
                                while (slängHögen.Count > 0)
                                {
                                    spelarna[i].player.Hand.Add(slängHögen[slängHögen.Count - 1]);
                                    slängHögen.RemoveAt(slängHögen.Count - 1);
                                }
                                wildcard = true;
                                spelarna[i].antalPass++;
                                break;
                            }
                            else
                            {
                                if (spelarensKort.Value == 10 || spelarensKort.Value == 2 || spelarensKort.Value == 14)
                                {
                                    spelarna[i].antalSpecialkort++;
                                }
                                for (int a = 0; a < spelarna[i].player.Hand.Count; a++)
                                {
                                    if (spelarna[i].player.Hand[a].Value == spelarensKort.Value && spelarna[i].player.Hand[a].Suit == spelarensKort.Suit)
                                    {
                                        spelarna[i].player.Hand.RemoveAt(a);
                                        break;
                                    }
                                }
                                slängHögen.Add(spelarensKort);
                                wildcard = false;
                            }
                            if (spelarensKort != null && spelarensKort.Value == 10 || spelarensKort != null && spelarensKort.Value == 2)
                            {
                                if (spelarensKort.Value == 10)
                                {
                                    slängHögen.Clear();
                                }
                                wildcard = true;
                            }
                            while (spelarna[i].player.Hand.Count < 3 && CardDeck.Count > 0)
                            {
                                spelarna[i].player.Hand.Add(CardDeck[0]);
                                CardDeck.RemoveAt(0);
                            }
                        }
                        else if (spelarna[i].player.VisibleHand.Count > 0)
                        {
                            spelarensKort = spelarna[i].player.LäggVisibleKort(slängHögen.Count > 0 ? slängHögen[slängHögen.Count - 1].Value : 2, wildcard ? Suit.Wild : slängHögen[slängHögen.Count - 1].Suit);
                            if (spelarensKort == null)
                            {
                                if (printGame)
                                {
                                    kortSomTasUpp += slängHögen.Count;
                                }
                                while (slängHögen.Count > 0)
                                {
                                    spelarna[i].player.Hand.Add(slängHögen[slängHögen.Count - 1]);
                                    slängHögen.RemoveAt(slängHögen.Count - 1);
                                }
                                wildcard = true;
                                spelarna[i].antalPass++;
                                break;
                            }
                            else
                            {
                                if (spelarensKort.Value == 10 || spelarensKort.Value == 2 || spelarensKort.Value == 14)
                                {
                                    spelarna[i].antalSpecialkort++;
                                }
                                for (int a = 0; a < spelarna[i].player.VisibleHand.Count; a++)
                                {
                                    if (spelarna[i].player.VisibleHand[a].Value == spelarensKort.Value && spelarna[i].player.VisibleHand[a].Suit == spelarensKort.Suit)
                                    {
                                        spelarna[i].player.VisibleHand.RemoveAt(a);
                                        break;
                                    }
                                }
                                slängHögen.Add(spelarensKort);
                                wildcard = false;
                            }
                            if (spelarensKort != null && spelarensKort.Value == 10 || spelarensKort != null && spelarensKort.Value == 2)
                            {
                                if (spelarensKort.Value == 10)
                                {
                                    slängHögen.Clear();
                                }
                                wildcard = true;
                            }
                        }
                        else if (spelarna[i].invisibleCards.Count > 0)
                        {
                            spelarensKort = spelarna[i].invisibleCards[RNG.Next(spelarna[i].invisibleCards.Count)];
                            if (wildcard || spelarensKort.Value >= slängHögen[slängHögen.Count - 1].Value || spelarensKort.Value == 2 && spelarna[i].invisibleCards.Count > 0 || spelarensKort.Value == 10 && spelarna[i].invisibleCards.Count > 0)
                            {
                                if (spelarensKort.Value == 10 || spelarensKort.Value == 2)
                                {
                                    if (spelarensKort.Value == 10)
                                    {
                                        slängHögen.Clear();
                                    }
                                    wildcard = true;
                                }
                                if (spelarensKort.Value == 10 || spelarensKort.Value == 2 || spelarensKort.Value == 14)
                                {
                                    spelarna[i].antalSpecialkort++;
                                }
                                for (int a = 0; a < spelarna[i].invisibleCards.Count; a++)
                                {
                                    if (spelarna[i].invisibleCards[a].Value == spelarensKort.Value && spelarna[i].invisibleCards[a].Suit == spelarensKort.Suit)
                                    {
                                        spelarna[i].invisibleCards.RemoveAt(a);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int a = 0; a < spelarna[i].invisibleCards.Count; a++)
                                {
                                    if (spelarna[i].invisibleCards[a].Value == spelarensKort.Value && spelarna[i].invisibleCards[a].Suit == spelarensKort.Suit)
                                    {
                                        spelarna[i].player.Hand.Add(spelarna[i].invisibleCards[a]);
                                        spelarna[i].invisibleCards.RemoveAt(a);
                                        break;
                                    }
                                }
                                if (printGame)
                                {
                                    kortSomTasUpp += slängHögen.Count;
                                }
                                while (slängHögen.Count > 0)
                                {
                                    spelarna[i].player.Hand.Add(slängHögen[slängHögen.Count - 1]);
                                    slängHögen.RemoveAt(slängHögen.Count - 1);
                                }
                                wildcard = true;
                                spelarna[i].antalPass++;
                                break;
                            }
                        }
                        speladeKort.Add(spelarensKort);
                        if (spelarna[i].invisibleCards.Count < 1)
                        {
                            return;
                        }
                    }
                    if (printGame)
                    {
                        OnGameUi(spelarna[i], senasteKortet, speladeKort, kortSomTasUpp);
                    }
                }
            }

            private void Shuffle()
            {
                for (int i = 0; i < 200; i++)
                {
                    switchCards();
                }
            }

            private PlayerInformation[] ShufflePlayers(PlayerInformation[] spelarna)
            {
                for (int i = 0; i < spelarna.Length * 5; i++)
                {
                    int player1 = RNG.Next(spelarna.Length);
                    int player2 = RNG.Next(spelarna.Length);
                    PlayerInformation temp = spelarna[player1];
                    spelarna[player1] = spelarna[player2];//
                    spelarna[player2] = temp;
                }
                return spelarna;
            }

            private PlayerInformation[] SortPlayer(PlayerInformation[] spelarna)
            {
                return spelarna.OrderBy(o => o.index).ToArray();//ja men det är lite mer än A nivå
            }

            private void switchCards()
            {
                int card1 = RNG.Next(CardDeck.Count);
                int card2 = RNG.Next(CardDeck.Count);
                Card temp = CardDeck[card1];
                CardDeck[card1] = CardDeck[card2];
                CardDeck[card2] = temp;
            }

            private void OnGameUi(PlayerInformation spelaren, Card senasteKortet, List<Card> speladeKorten, int kortSomTasUpp)
            {
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = (ConsoleColor)spelaren.index + 9; ;
                Console.WriteLine(spelaren.player.Name + " har    ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Gömda korten:");
                WriteHand(spelaren.invisibleCards);
                Console.WriteLine("Top korten:");
                WriteHand(spelaren.player.VisibleHand);
                Console.WriteLine("Korten på hand:");
                WriteHand(spelaren.player.Hand);
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("                ");
                }
                int offset = 5;
                if (kortSomTasUpp > 0)
                {
                }
                if (speladeKorten.Count < 1)
                {
                    Console.SetCursorPosition(20, offset);
                    Console.Write(spelaren.player.Name + " kan inte lägga och tar upp " + kortSomTasUpp + " kort                                  ");
                    offset++;
                }
                for (int i = 0; i < speladeKorten.Count; i++)
                {
                    Console.SetCursorPosition(20, offset);
                    if (speladeKorten[i] != null)
                    {
                        Console.Write(spelaren.player.Name + " spelar");
                        speladeKorten[i].PrintCard();
                        Console.Write("                                             ");
                    }
                    else
                    {
                        Console.Write(spelaren.player.Name + " kan inte lägga och tar upp " + kortSomTasUpp + " kort                                  ");
                    }
                    offset++;
                }
                Console.ForegroundColor = (ConsoleColor)spelaren.index + 9; ;
                Console.SetCursorPosition(20, offset);
                Console.Write(PlayerFunctions.Reason + "                                            ");
                PlayerFunctions.Reason = "";
                Console.ForegroundColor = ConsoleColor.White;
                for (int i = 0; i < 5; i++)
                {
                    offset++;
                    Console.SetCursorPosition(20, offset);
                    Console.WriteLine("                                                                                                     ");
                }
                Console.ReadKey(true);
            }

            private void WriteHand(List<Card> Hand)
            {
                for (int i = 0; i < Hand.Count; i++)
                {
                    Hand[i].PrintCard();
                    Console.WriteLine();
                    if (i > 7)
                    {
                        Console.WriteLine(Hand.Count - i + " more cards");
                        break;
                    }
                }
            }
        }
    }

    public class Card
    {
        public int Value { get; private set; } //Kortets värde enligt reglerna i Bluffstopp, t.ex. dam = 12
        public Suit Suit { get; private set; }

        //public int Id { get; private set; } //Typ av kort, t.ex dam = 12

        public Card(int value, Suit suit)
        {
            Suit = suit;
            Value = value;
        }

        public void PrintCard()
        {
            string cardname = "";
            if (Value == 14)
            {
                cardname = "ess    ";
            }
            else if (Value == 11)
            {
                cardname = "knekt  ";
            }
            else if (Value == 12)
            {
                cardname = "dam     ";
            }
            else if (Value == 13)
            {
                cardname = "kung   ";
            }
            else
            {
                cardname = Value + "      ";
            }
            if (Suit == Suit.Hjärter)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (Suit == Suit.Ruter)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (Suit == Suit.Spader)
                Console.ForegroundColor = ConsoleColor.Gray;
            else
            if (Suit == Suit.Klöver)
                Console.ForegroundColor = ConsoleColor.Green;

            Console.Write(" " + Suit + " " + cardname);
            if (cardname == "")
            {
                Console.Write(Value + "     ");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void PrintCard(int i)
        {
            string cardname = "";
            if (Value == 14)
            {
                cardname = "ess    ";
            }
            else if (Value == 11)
            {
                cardname = "knekt  ";
            }
            else if (Value == 12)
            {
                cardname = "dam     ";
            }
            else if (Value == 13)
            {
                cardname = "kung   ";
            }
            else
            {
                cardname = Value + "      ";
            }
            if (Suit == Suit.Hjärter)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (Suit == Suit.Ruter)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (Suit == Suit.Spader)
                Console.ForegroundColor = ConsoleColor.Gray;
            else if (Suit == Suit.Klöver)
                Console.ForegroundColor = ConsoleColor.Green;

            Console.Write(" " + i + " " + Suit + " " + cardname);
            if (cardname == "")
            {
                Console.Write(Value + " ");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    internal class PlayerInformation
    {
        public Player player;
        public int wins;
        public List<Card> invisibleCards = new List<Card>();
        public int antalPass;
        public int antalSpecialkort;
        public int index { get; private set; }

        public PlayerInformation(Player _player, List<Card> _invisibleCards, int _index)
        {
            player = _player;
            invisibleCards = _invisibleCards;
            antalPass = 0;
            index = _index;
            antalSpecialkort = 0;
        }
    }

    public class PlayerFunctions
    {
        public static string Reason;
        private static Random RNG = new Random();

        public static List<Card> SortHandByValue(List<Card> hand) //Sorterar handen efter värde, lägst först.
        {
            return hand.OrderBy(o => o.Value).ToList();
        }

        public static void StateReason(string reason)
        {
            if (Reason.Length + reason.Length < 100)
            {
                Reason += reason + ". ";
            }
            else
            {
                Reason += " Långt!";
            }
        }

        public static List<Card> Shuffle(List<Card> Hand)
        {
            for (int i = 0; i < Hand.Count * 4; i++)
            {
                Hand = switchCards(Hand);
            }
            return Hand;
        }

        private static List<Card> switchCards(List<Card> Hand)
        {
            int card1 = RNG.Next(Hand.Count);
            int card2 = RNG.Next(Hand.Count);
            Card temp = Hand[card1];
            Hand[card1] = Hand[card2];
            Hand[card2] = temp;
            return Hand;
        }
    }

    internal abstract class Player
    {
        public string Name;
        public string OpponentName;  //Motståndarens namn
        public Card LatestCard;
        //private PlayerFunctions PlayerFunctions;

        // Dessa variabler får ej ändras
        public List<Card> Hand = new List<Card>();  // Lista med alla kort i handen.

        public List<Card> VisibleHand = new List<Card>();  // Lista med alla kort i handen.

        public abstract Card LäggEttKort(int cardValue, Suit cardSuit);

        public abstract Card LäggVisibleKort(int cardValue, Suit cardSuit);

        public abstract void SpelSlut(int cardsLeft, int opponentCardsLeft);
    }

    internal class HonestPlayer : Player //Denna spelare bluffar aldrig, och tror aldrig att moståndaren bluffar
    {
        private Card kortJagSpelade; //Håller reda på vilket kort som spelaren lagt.

        public HonestPlayer()
        {
            Name = "HonestPlayer";
        }

        public override Card LäggEttKort(int cardValue, Suit cardSuit)
        {
            Hand = PlayerFunctions.SortHandByValue(Hand); //Lägger de lägsta korten först i handen.
            if (cardSuit == Suit.Wild) //Om valfritt kort får spelas
            {
                PlayerFunctions.StateReason("Jag kan lägga vad jag vill, så jag lägger mitt lägsta kort");
                kortJagSpelade = Hand[0]; //Sparar kortet som skall spelas
                return Hand[0]; //Spelar ut det första kortet på handen
            }
            for (int i = 0; i < Hand.Count; i++) //Går igenom alla korten på handen
            {
                if (Hand[i].Value >= cardValue || Hand[i].Value == 2 || Hand[i].Value == 10) //Om den hittar ett kort på handen som är högre och i samma suit som det som ligger
                {
                    PlayerFunctions.StateReason("Jag lägger det lägsta kortet jag kan");
                    kortJagSpelade = Hand[i]; //Sparar kortet som skall spelas
                    return Hand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit
                }
            }
            PlayerFunctions.StateReason("Jag kan inte lägga något kort och tänker inte bluffa");
            return null; //Om inget kort hittats säger spelaren pass
        }

        public override Card LäggVisibleKort(int cardValue, Suit cardSuit)
        {
            VisibleHand = PlayerFunctions.SortHandByValue(VisibleHand); //Lägger de lägsta korten först i handen.
            if (cardSuit == Suit.Wild) //Om valfritt kort får spelas
            {
                PlayerFunctions.StateReason("Jag kan lägga vad jag vill, så jag lägger mitt lägsta kort");
                kortJagSpelade = VisibleHand[0]; //Sparar kortet som skall spelas
                return VisibleHand[0]; //Spelar ut det första kortet på handen
            }
            for (int i = 0; i < VisibleHand.Count; i++) //Går igenom alla korten på handen
            {
                if (VisibleHand[i].Value >= cardValue || VisibleHand[i].Value == 2 || VisibleHand[i].Value == 10) //Om den hittar ett kort på handen som är högre och i samma suit som det som ligger
                {
                    PlayerFunctions.StateReason("Jag lägger det lägsta kortet jag kan");
                    kortJagSpelade = VisibleHand[i]; //Sparar kortet som skall spelas
                    return VisibleHand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit
                }
            }
            PlayerFunctions.StateReason("Jag kan inte lägga något kort och tänker inte bluffa");
            return null; //Om inget kort hittats säger spelaren pass
        }

        public override void SpelSlut(int cardsLeft, int opponentCardsLeft)
        {
            //Behöver inte användas.
        }
    }

    internal class RandomPlayer : Player
    //Denna spelare bluffar ibland och synar(säger bluffstopp) ibland beroende på vilka värden som skrivs in i konstruktorn
    {
        private Card kortJagSpelade; //Håller reda på vilket kort som spelaren lagt.
        private Random RNG;

        public RandomPlayer()  //Konstruktor
        {
            Name = "RandomPlayer";
            RNG = new Random();
        }

        public override Card LäggEttKort(int cardValue, Suit cardSuit)
        {
            Hand = PlayerFunctions.Shuffle(Hand); //Lägger de lägsta korten först i handen.

            for (int i = 0; i < Hand.Count; i++) //Går igenom alla korten på handen
            {
                if (Hand[i].Value >= cardValue || Hand[i].Value == 2 || Hand[i].Value == 10) //Om den hittar ett kort på handen som är högre och i smma suit som det som ligger
                {
                    PlayerFunctions.StateReason("Jag lägger det lägsta kortet jag kan.");
                    kortJagSpelade = Hand[i]; //Sparar kortet som skall spelas
                    return Hand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit
                }
            }
            PlayerFunctions.StateReason("Jag kan inte lägga något kort, och säger pass");
            return null;  //Spelaren väljer att inte bluffa och säger pass
        }

        public override Card LäggVisibleKort(int cardValue, Suit cardSuit)
        {
            VisibleHand = PlayerFunctions.Shuffle(VisibleHand); //Lägger de lägsta korten först i handen.

            for (int i = 0; i < VisibleHand.Count; i++) //Går igenom alla korten på handen
            {
                if (VisibleHand[i].Value >= cardValue || VisibleHand[i].Value == 2 || VisibleHand[i].Value == 10) //Om den hittar ett kort på handen som är högre och i smma suit som det som ligger
                {
                    PlayerFunctions.StateReason("Jag lägger det lägsta kortet jag kan.");
                    kortJagSpelade = VisibleHand[i]; //Sparar kortet som skall spelas
                    return VisibleHand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit
                }
            }
            PlayerFunctions.StateReason("Jag kan inte lägga något kort, och säger pass");
            return null;  //Spelaren väljer att inte bluffa och säger pass
        }

        public override void SpelSlut(int cardsLeft, int opponentCardsLeft)
        {
            //Behöver inte användas.
        }
    }

    internal class MyPlayer : Player //Denna spelare bluffar aldrig, och tror aldrig att moståndaren bluffar
    {
        private Card kortJagSpelade; //Håller reda på vilket kort som spelaren lagt.

        public MyPlayer()
        {
            Name = "MyPlayer";
        }

        public override Card LäggEttKort(int cardValue, Suit cardSuit)
        {
            Hand = PlayerFunctions.SortHandByValue(Hand); //Lägger de lägsta korten först i handen.
            if (cardSuit == Suit.Wild) //Om valfritt kort får spelas
            {
                PlayerFunctions.StateReason("Jag kan lägga vad jag vill, så jag lägger mitt lägsta kort");
                for (int i = 0; i < Hand.Count; i++)
                {
                    if (Hand[i].Value != 2 && Hand[i].Value != 10)
                    {
                        kortJagSpelade = Hand[i]; //Sparar kortet som skall spelas
                        return Hand[i]; //Spelar ut det första kortet på handen
                    }
                }
                kortJagSpelade = Hand[0]; //Sparar kortet som skall spelas
                return Hand[0]; //Spelar ut det första kortet på handen
            }
            for (int i = 0; i < Hand.Count; i++) //Går igenom alla korten på handen
            {
                if (Hand[i].Value >= cardValue && Hand[i].Value != 2 && Hand[i].Value != 10) //Om den hittar ett kort på handen som är högre och i samma suit som det som ligger
                {
                    PlayerFunctions.StateReason("Jag lägger det lägsta kortet jag kan");
                    kortJagSpelade = Hand[i]; //Sparar kortet som skall spelas
                    return Hand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit
                }
            }
            for (int i = 0; i < Hand.Count; i++)
            {
                if (Hand[i].Value == 2 || Hand[i].Value == 10)
                {
                    PlayerFunctions.StateReason("Jag lägger ett special kort");
                    kortJagSpelade = Hand[i]; //Sparar kortet som skall spelas
                    return Hand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit
                }
            }
            PlayerFunctions.StateReason("Jag kan inte lägga något kort");
            return null; //Om inget kort hittats säger spelaren pass
        }

        public override Card LäggVisibleKort(int cardValue, Suit cardSuit)
        {
            VisibleHand = PlayerFunctions.SortHandByValue(VisibleHand); //Lägger de lägsta korten först i handen.
            if (cardSuit == Suit.Wild) //Om valfritt kort får spelas
            {
                PlayerFunctions.StateReason("Jag kan lägga vad jag vill, så jag lägger mitt lägsta kort");
                for (int i = 0; i < VisibleHand.Count; i++)
                {
                    if (VisibleHand[i].Value != 2 && VisibleHand[i].Value != 10)
                    {
                        kortJagSpelade = VisibleHand[i]; //Sparar kortet som skall spelas
                        return VisibleHand[i]; //Spelar ut det första kortet på handen
                    }
                }
                kortJagSpelade = VisibleHand[0]; //Sparar kortet som skall spelas
                return VisibleHand[0]; //Spelar ut det första kortet på handen
            }
            for (int i = 0; i < VisibleHand.Count; i++) //Går igenom alla korten på handen
            {
                if (VisibleHand[i].Value >= cardValue && VisibleHand[i].Value != 2 && VisibleHand[i].Value != 10) //Om den hittar ett kort på handen som är högre och i samma suit som det som ligger
                {
                    PlayerFunctions.StateReason("Jag lägger det lägsta kortet jag kan");
                    kortJagSpelade = VisibleHand[i]; //Sparar kortet som skall spelas
                    return VisibleHand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit
                }
            }
            for (int i = 0; i < VisibleHand.Count; i++)
            {
                if (VisibleHand[i].Value == 2 || VisibleHand[i].Value == 10)
                {
                    PlayerFunctions.StateReason("Jag lägger ett special kort");
                    kortJagSpelade = VisibleHand[i]; //Sparar kortet som skall spelas
                    return VisibleHand[i]; //Spelar ut det första kort den hittar som är högre och i samma suit
                }
            }
            PlayerFunctions.StateReason("Jag kan inte lägga något kort");
            return null; //Om inget kort hittats säger spelaren pass
        }

        public override void SpelSlut(int cardsLeft, int opponentCardsLeft)
        {
            //Behöver inte användas.
        }
    }
}