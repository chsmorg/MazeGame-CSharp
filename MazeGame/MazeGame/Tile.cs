using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MazeGame
{

    public enum TileType
    {
        start,
        end,
        path,
        key,
        door,
        none
    }

    public enum MoveDirection
    {
        Up, Down, Left, Right
    }

    public class Tile
    {

        private Ellipse? circle;

        public int x, y;

        public bool northWall, southWall, eastWall, westWall = false;
        public bool visited { get; set; } = false;
        public TileType type { get; set; } = TileType.none;

        public Tile(int x, int y)
        {
            this.x = x;
            this.y = y;

        }

        public Ellipse? getCircle()
        {
            return circle;
        }

        public void addCircle(Ellipse circle)
        {
            this.circle = circle;
        }

        public void removeCircle()
        {
            this.circle = null;
        }

        public bool isValidMove(MoveDirection direction)
        {
            switch (direction)
            {
                case MoveDirection.Up:
                    if (northWall)
                    {
                        return false;
                    }
                    break;
                case MoveDirection.Down: 
                    if (southWall)
                    {
                        return false;
                    }
                    break;
                case MoveDirection.Left:
                    if (westWall)
                    {
                        return false;
                    }
                        break;
                case MoveDirection.Right:
                    if (eastWall)
                    {
                       return false;
                    }
                    break;
            }
            return true;
        }


     
    }
}
