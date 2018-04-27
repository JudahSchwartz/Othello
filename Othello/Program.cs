using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    

    abstract class Player
    {
        protected bool Color { get; set; }

        public Player(bool color)
        {
            Color = color;
        }
        public abstract Point MakeMove(IEnumerable<Spot> possibleMoves);
    }
    class OthelloAI : Player
    {
        public override Point MakeMove(IEnumerable<Spot> possibleMoves)
        {
            int maxMoves = 0;
            Spot p = new Spot(-1,-1,null,null);
            foreach (var spot in possibleMoves)
            {
                if (spot.SpotsToTurn(Color).Count > maxMoves)
                {
                    maxMoves = spot.SpotsToTurn(Color).Count;
                    p = spot;
                }
            }

            return p.Point;
        }


        public OthelloAI(bool color) : base(color)
        {
        }
    }

    class Human : Player
    {
        public override Point MakeMove(IEnumerable<Spot> possibleMoves)
        {
            int x = Convert.ToInt32(Console.ReadLine());
            int y = Convert.ToInt32(Console.ReadLine());
            return new Point(x, y);
        }

        public Human(bool color) : base(color)
        {
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter 1 for user vs pc, 2 for user vs user, and 3 for pc vs pc");
            int choice = Convert.ToInt32(Console.ReadLine());
            Player p1 = new OthelloAI(Spot.Black);
            Player p2 = new OthelloAI(Spot.White);
            
            switch (choice)
            {
                case 1:
                    p1 = new Human(Spot.Black);
                    p2 = new OthelloAI(Spot.White);
                    break;
                case 2:
                    p1 = new Human(Spot.Black);
                    p2 = new Human(Spot.White);
                    break;
            }

            Board board = new Board();
            bool color = board.Turn;
            bool winner;
            while (!board.GameOver(out winner))
            {
                DisplayBoard(board);
                Console.Write(color ? "White" : "Black");
                Console.WriteLine("'s turn. Pick a spot(v,h)");
                Player currentTurn = color ? p2 : p1;
                Point p = currentTurn.MakeMove(board.PossibleMoves());
                if (board.PossibleMoves().Select(s=>s.Point).Contains(p))
                {
                    board.PlacePiece(p.X, p.Y, color);
                    color = !color;
                }
                else
                    Console.WriteLine("Not a valid move.");
            }
            DisplayBoard(board);
            Console.Write(winner ? "White" : "Black");
            Console.WriteLine(" is the winner! Enter to exit.");
            Console.ReadLine();

        }

       

        private static void DisplayBoard(Board board)
        {
            bool?[,] nullableArray = board.BoardState();
            Console.Write("| |");
            for (int index = 0; index < 8; ++index)
                Console.Write("|" + (object) index + "|");
            Console.WriteLine();
            for (int index1 = 0; index1 < nullableArray.GetLength(0); ++index1)
            {
                Console.Write("|" + (object) index1 + "|");
                for (int index2 = 0; index2 < nullableArray.GetLength(1); ++index2)
                {
                    Console.Write('|');
                    bool? nullable = nullableArray[index1, index2];
                    bool flag1 = true;
                    if (nullable.GetValueOrDefault() == flag1 && nullable.HasValue)
                    {
                        Console.Write('X');
                    }
                    else
                    {
                        nullable = nullableArray[index1, index2];
                        bool flag2 = false;
                        if (nullable.GetValueOrDefault() == flag2 && nullable.HasValue)
                            Console.Write('O');
                        else
                            Console.Write(' ');
                    }

                    Console.Write('|');
                }

                Console.WriteLine();
            }
        }





    }

    class Spot
    {
        private Board board;
        public const bool Black = false;
        public const bool White = true;
        public Point Point { get; }
        public bool? Color { get; set; } //null for empty spot

        public override int GetHashCode()
        {
            return 0;
        }

        public Spot(int x, int y, bool? color, Board board)
        {
            Point = new Point(x, y);
            Color = color;
            this.board = board;
        }

        private bool WithinRange(int x, int y)
        {
            return x < board.Spots.GetLength(0) && x >= 0 && y < board.Spots.GetLength(1) && y >= 0;
        }

        private ISet<Spot> CheckSpots(int i, int j, bool color)
        {
            ISet<Spot> spots = new HashSet<Spot>();
            Spot nextSpot;
            int x = i, y = j;
            do
            {
                if (!WithinRange(Point.X + x, Point.Y + y))
                {
                    spots.Clear();
                    return spots;
                }

                nextSpot = board.Spots[Point.X + x, Point.Y + y];
                x += i;
                y += j;
                if (nextSpot.Color == !color)
                {
                    spots.Add(nextSpot);
                }
            } while (nextSpot.Color == !color);

            if (nextSpot.Color == null)
            {
                spots.Clear();
            }

            return spots;


        }

        public ISet<Spot> SpotsToTurn(bool color)
        {
           

            ISet<Spot> spots = new HashSet<Spot>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (!(i == 0 && j == 0))
                        spots.UnionWith(CheckSpots(i, j, color)); //TODO CHECK THAT THIS IS Equivalent to add all
                }
            }

            return spots;
        }

        public int TurnPieces()
        {
            ISet<Spot> spots = SpotsToTurn((bool) Color);
            foreach (var spot in spots)
            {
                spot.Color = !spot.Color;
            }

            return spots.Count;
        }
    }

    class Board
    {
        public Spot[,] Spots = new Spot[8, 8];
        public bool Turn { get; set; } = false;

        public Board()
        {
            for (int i = 0; i < Spots.GetLength(0); i++)
            {
                for (int j = 0; j < Spots.GetLength(1); j++)
                {
                    Spots[i, j] = new Spot(i, j, null, this);
                }
            }

            Spots[3, 3].Color = Spot.White;
            Spots[4, 4].Color = Spot.White;
            Spots[3, 4].Color = Spot.Black;
            Spots[4, 3].Color = Spot.Black;
        }

        public ISet<Spot> PossibleMoves()
        {
            var pointSet = new HashSet<Spot>();
            foreach (var spot in Spots.Cast<Spot>().Where(s => s.Color == null))
            {
                if (CanPlacePiece(spot.Point.X, spot.Point.Y, Turn))
                    pointSet.Add(spot);
            }

            return pointSet;
        }

        public int Score()
        {
            return Spots.Cast<Spot>().Count(s => s.Color == Spot.White) -
                   Spots.Cast<Spot>().Count(s => s.Color == Spot.Black);
        }

        public bool GameOver(out bool winner)
        {
            if (Spots.Cast<Spot>().Count(s => s.Color == Spot.White)==0)
            {
                winner = Spot.Black;
                return true;
            }
            if (Spots.Cast<Spot>().Count(s => s.Color == Spot.Black) == 0)
            {
                winner = Spot.White;
                return true;
            }

            if (Spots.Cast<Spot>().Count(s => s.Color == null) == 0)
            {
                winner = Spots.Cast<Spot>().Count(s => s.Color == Spot.White) >
                         Spots.Cast<Spot>().Count(s => s.Color == Spot.Black);
                return true;
            }

            winner = false;
            return false;
        }


        public bool?[,] BoardState()
        {
            var spots = new bool?[Spots.GetLength(0), Spots.GetLength(1)];
            for (int i = 0; i < this.Spots.GetLength(0); i++)
            {
                for (int j = 0; j < Spots.GetLength(1); j++)
                    spots[i, j] = Spots[i, j].Color;
            }

            return spots;
        }

        public bool CanPlacePiece(int x, int y, bool color)
        {
            return Spots[x, y].Color == null && Spots[x, y].SpotsToTurn(color).Count != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <returns>Number of pieces turned</returns>
        public int PlacePiece(int x, int y, bool color)
        {
            Spots[x, y].Color = color;
            Turn = !Turn;
            return Spots[x, y].TurnPieces();
        }


    }
}
