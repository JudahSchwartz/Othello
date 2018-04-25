using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Othello
{
    
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    class Spot
    {
        private Board board;
        const bool Black = false;
        const bool White = true;
        public Point Point { get; }
        public bool? Color { get; set; }//null for empty spot

        public override int GetHashCode()
        {
            return 0;
        }
        public Spot(int x, int y, bool? color, Board board)
        {
           Point = new Point(x,y);
            Color = color;
            this.board = board;
        }

        private bool WithinRange(int x, int y)
        {
            return x <= board._spots.GetLength(0) && x >= 0 && y <= board._spots.GetLength(1) && y >= 0;
        }

        private ISet<Spot> CheckSpots(int i, int j)
        {
            ISet<Spot> spots = new HashSet<Spot>();
            Spot nextSpot;

            do
            {
                if (!WithinRange(Point.X + i, Point.Y + j))
                {
                    spots.Clear();
                    return spots;
                }

                nextSpot = board._spots[Point.X + i, Point.Y + j];
                i += i;
                j += j;
                if (nextSpot.Color == Color)
                {
                    spots.Add(nextSpot);
                }
            } while (nextSpot.Color != Color);

            if (nextSpot.Color == null)
            {
                spots.Clear();
            }

            return spots;


        }
        public ISet<Spot> SpotsToTurn(bool color)
        {
            if (Color != null)
            {
                throw new Exception("Spot already contains a piece");
            }

            ISet<Spot> spots = new HashSet<Spot>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if(!(i==0 && j == 0))
                        spots.UnionWith(CheckSpots(0, 1));//TODO CHECK THAT THIS IS Equivalent to add all
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
        public Spot[,] _spots = new Spot[8,8];

        public Board()
        {
            for (int i = 0; i < _spots.GetLength(0); i++)
            {
                for (int j = 0; j < _spots.GetLength(1); j++)
                {
                    _spots[i,j] = new Spot(i,j,null,this);
                }
            }
        }
        public bool CanPlacePiece(int x, int y, bool color)
        {
            return _spots[x, y].Color == null &&  _spots[x, y].SpotsToTurn(color).Count==0;
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
            _spots[x, y].Color = color;
            return _spots[x, y].TurnPieces();
        }

        
    }
}
