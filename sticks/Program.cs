using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sticks
{
    class Program
    {
        static void Main(string[] args)
        {
            //AImove(2, 1, 1, 1);
            //Console.ReadLine();
            game();
            //AIgame();
        }

        static void AIgame()
        {
            int a = 1, b = 1, p = 1, q = 1;
            while (true)
            {
                Console.ReadLine();
                userDisplay(a, b, p, q);
                Console.WriteLine("AI1 moved");
                int[] ab = AImove(p, q, a, b);
                a = ab[0];
                b = ab[1];
                if (gameEnd(a, b, p, q) != 0) break;
                Console.ReadLine();
                userDisplay(a, b, p, q);
                Console.WriteLine("AI2 moved");
                int[] pq = AImove(a, b, p, q);
                p = pq[0];
                q = pq[1];
                if (gameEnd(a, b, p, q) != 0) break;
            }
            if (gameEnd(a, b, p, q) == -1) Console.WriteLine("AI2 won");
            if (gameEnd(a, b, p, q) == 1) Console.WriteLine("AI1 won");
            string com = Console.ReadLine();
        }

        static void game()
        {
            bool exit = false;
            while(!exit)
            {
                bool userFirst = WhoFirst();
                int a = 1, b = 1, p = 1, q = 1;
                if (userFirst)
                {
                    int[] pq = getUserMove(a, b, p, q);
                    p = pq[0];
                    q = pq[1];
                }
                while (true)
                {
                    userDisplay(a, b, p, q);
                    Console.WriteLine("AI moved");
                    int[] ab = AImove(p, q, a, b);
                    a = ab[0];
                    b = ab[1];
                    if (gameEnd(a, b, p, q) != 0) break;
                    int[] pq = getUserMove(a, b, p, q);
                    p = pq[0];
                    q = pq[1];
                    if (gameEnd(a, b, p, q) != 0) break;
                }
                if (gameEnd(a, b, p, q) == -1) Console.WriteLine("AI won");
                if (gameEnd(a, b, p, q) == 1) Console.WriteLine("You won");
                exit = endComm();
            }
        }

        static bool endComm()
        {
            Console.WriteLine("End or restart? (e/r)");
            char[] choice = new char[] { 'e', 'r' };
            int indx = Array.IndexOf(choice, (Console.ReadKey().KeyChar + "").ToLower()[0]);
            Console.WriteLine("");
            if (indx == -1)
            {
                Console.WriteLine("Invalid input");
                return endComm();
            }
            return indx == 0;
        }

        static bool WhoFirst()
        {
            Console.WriteLine("Would you like to go first? (y/n)");
            char[] choice = new char[] { 'y', 'n' };
            int indx = Array.IndexOf(choice, (Console.ReadKey().KeyChar + "").ToLower()[0]);
            Console.WriteLine("");
            if (indx == -1)
            {
                Console.WriteLine("Invalid input");
                return WhoFirst();
            }
            return indx == 0;
        }

        static int[] AImove(int a, int b, int p, int q)
        {
            bool debug = false;
            List<Tuple<int, int>> pos = possible(a, b, p, q);

            int min = 1000; //Random really large positive number
            int minIndex = -1;
            for (int i = 0; i < pos.Count; i++)
            {
                //Assume that we do this move
                int np = pos[i].Item1, nq = pos[i].Item2;
                if (debug) Console.WriteLine("");
                if (debug) Console.WriteLine("ASSUMING AI:");
                if (debug) userDisplay(a, b, np, nq, true);

                if (gameEnd(a, b, np, nq) == 1)
                {
                    //We will win if this move happens
                    return new int[] { np, nq };
                }

                List<Tuple<int, int>> posHuman = possible(np, nq, a, b);
                int val = -1000; //Random really large negative number
                //int maxindex = -1;
                //Simulate Human Player
                for (int j = 0; j < posHuman.Count; j++)
                {
                    int cstateScore = stateScore(np, nq, posHuman[j].Item1, posHuman[j].Item2);
                    if (val < cstateScore) //Max
                    {
                        //maxindex = j;
                        val = cstateScore;
                    }
                    if (debug) Console.WriteLine("ASSUMING PLAYER:");
                    if (debug) Console.WriteLine("VALUE: {0}", cstateScore);
                    if (debug) userDisplay(np, nq, posHuman[j].Item1, posHuman[j].Item2);
                }

                if (min > val)
                {
                    min = val;
                    minIndex = i;
                }
            }

            if (debug) Console.WriteLine("");
            if (debug) Console.WriteLine("FINAL WITH PLAYER SCORE OF {0}", min);
            if (debug) userDisplay(a, b, pos[minIndex].Item1, pos[minIndex].Item2, true);
            return new int[] { pos[minIndex].Item1, pos[minIndex].Item2 };
        }

        static int gameEnd(int a, int b, int p, int q)
        {
            if (a == 0 && b == 0) return -1;
            if (p == 0 && q == 0) return 1;
            return 0;
        }

        static void userDisplay(int a, int b, int p, int q, bool swap = false)
        {
            if (swap)
            {
                Console.WriteLine("Player: {0}|{1}", p, q);
                Console.WriteLine("AI:     {0}|{1}", a, b);
                return;
            }
            Console.WriteLine("AI:  {0}|{1}", p, q);
            Console.WriteLine("You: {0}|{1}", a, b);
        }

        static int[] getUserMove(int a, int b, int p, int q)
        {
            Console.WriteLine("Your Move");
            userDisplay(a, b, p, q);
            while (true)
            {
                try
                {
                    int[] op = getUserInput();
                    return doMove(a, b, p ,q, op[0], op[1]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static int[] doMove(int a, int b, int p, int q, int s, int d)
        {
            if (s == 0 && a == 0) throw new Exception("Invalid move");
            if (s == 1 && b == 0) throw new Exception("Invalid move");
            if (d == 0 && p == 0) throw new Exception("Invalid move");
            if (d == 1 && q == 0) throw new Exception("Invalid move");

            if (s == 0 && d == 0) return new int[] { add(a, p), q };
            else if (s == 1 && d == 0) return new int[] { add(b, p), q };
            else if (s == 0 && d == 1) return new int[] { p, add(a, q) };
            else if (s == 1 && d == 1) return new int[] { p, add(b, q) };
            else throw new Exception("Invalid Move");
        }

        public static int[] getUserInput()
        {
            Console.Write("move>");
            string str = Console.ReadLine();
            //string[] strs = str.Split(',');
            if (str.Length != 2)
            {
                throw new Exception("Invalid move");
            }
            string[] opts = { "1", "2" };
            if (!opts.Contains(str[0] + ""))
            {
                throw new Exception("Invalid move");
            }
            if (!opts.Contains(str[1] + ""))
            {
                throw new Exception("Invalid move");
            }
            return new int[] { Array.IndexOf(opts, str[0] + ""), Array.IndexOf(opts, str[1] + "") };
        }

        public static int stateScore(int a, int b, int p, int q)
        {
            int score = 0;
            if (a == 0) score--;
            if (b == 0) score--;
            if (p == 0) score++;
            if (q == 0) score++;
            return score;
        }

        public static List<Tuple<int, int>> possible(int a, int b, int p, int q)
        {
            List<Tuple<int, int>> ret = new List<Tuple<int, int>>();
            if (a != 0)
            {
                if (p != 0)
                {
                    ret.Add(new Tuple<int, int>(add(a, p), q));
                }
                if (q != 0)
                {
                    ret.Add(new Tuple<int, int>(p, add(a, q)));
                }
            }
            if (b != 0)
            {
                if (p != 0)
                {
                    ret.Add(new Tuple<int, int>(add(b, p), q));
                }
                if (q != 0)
                {
                    ret.Add(new Tuple<int, int>(p, add(b, q)));
                }
            }

            return ret;
        }

        public static int add(int a, int b)
        {
            return (a + b) % 5;
        }
    }
}
