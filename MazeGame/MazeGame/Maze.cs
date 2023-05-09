using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace MazeGame
{

    public enum Orientation
    {
        Horizontal,
        Vertical
    }
    class Maze
    {
        public int height { get; }
        public int width { get; }
        private Random random = new Random();
        private Tile[,] grid;
        public Maze(int height, int width) {
            this.height = height;
            this.width = width;
            this.grid = new Tile[height, width];
        }

        public async Task GenerateAsync()
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        grid[i,j] = new Tile(i, j);
                        if (i == 0 && j == 0)
                        {
                            grid[i, j].type = TileType.start;
                        }
                        else if (i == height - 1 && j == width - 1)
                        {
                            grid[i, j].type = TileType.end;
                        }

                        if (i == 0)
                        {
                            grid[i, j].westWall = true;
                        }
                        if (i == width - 1)
                        {
                            grid[i, j].eastWall = true;
                        }
                        if (j == 0)
                        {
                            grid[i, j].northWall = true;
                        }
                        if (j == height - 1)
                        {
                            grid[i, j].southWall = true;
                        }
                    }
                }
                DivideMaze(0, 0, width, height);
            });
        }


        public Tile getTile(int x, int y)
        {
            return grid[x,y];
        }

        private Orientation ChooseOrientation(int width, int height)
        {
            if (width < height)
            {
                return Orientation.Horizontal;
            }
            else if (height < width)
            {
                return Orientation.Vertical;
            }
            return random.Next(0, 2) == 0 ? Orientation.Horizontal : Orientation.Vertical;
        }

        public void DivideMaze(int x, int y, int width, int height)
        {
            if (width < 2 || height < 2)
            {
                return;
            }
            bool horizontal = ChooseOrientation(width, height) == Orientation.Horizontal;

            int horizontalWallX = x + (horizontal ? 0 : random.Next(0, width - 1));
            int verticalWallY = y + (horizontal ? random.Next(0, height - 1) : 0);

            int passageX = horizontalWallX + (horizontal ? random.Next(width) : 0);
            int passageY = verticalWallY + (horizontal ? 0 : random.Next(height));

            int directionX = horizontal ? 1 : 0;
            int directionY = horizontal ? 0 : 1;

            int length = horizontal ? width : height;

            for (int i = 0; i < length; i++)
            {
                if (passageX != horizontalWallX || passageY != verticalWallY)
                {
                    if (horizontal)
                    {
                        //grid[horizontalWallX, verticalWallY].southWall = true;
                        setSouthWall(horizontalWallX, verticalWallY);
                    }
                    else
                    {
                        //grid[horizontalWallX, verticalWallY].eastWall = true;
                        setEastWall(horizontalWallX, verticalWallY);
                    }
                }
                horizontalWallX += directionX;
                verticalWallY += directionY;
            }

            if (horizontal)
            {
                Parallel.Invoke(
                    () => DivideMaze(x, y, width, verticalWallY - y + 1),
                    () => DivideMaze(x, verticalWallY + 1, width, y + height - verticalWallY - 1)
                   );
               
               
            }
            else
            {
               Parallel.Invoke(
                () => DivideMaze(x, y, horizontalWallX - x + 1, height),
                () => DivideMaze(horizontalWallX + 1, y, x + width - horizontalWallX - 1, height)
                );
            }
        }

        public void addCircle(int x, int y, Ellipse circle)
        {
            grid[x,y].addCircle(circle);
        }

        public void removeCircle(int x, int y)
        {
            grid[x, y].removeCircle();
        }

        private void setWestWall(int x, int y)
        {
            grid[x, y].westWall= true;
            if (x >= 1)
            {
                grid[x-1,y].eastWall= true;
            }
        }
        
        private void setEastWall(int x, int y)
        {
            grid[x, y].eastWall = true;
            if (x < width-1)
            {
                grid[x+1, y].westWall = true;
            }
        }

        private void setNorthWall(int x, int y)
        {
            grid[x, y].northWall = true;
            if (y >= 1)
            {
                grid[x, y-1].southWall = true;
            }
        }

        private void setSouthWall(int x, int y)
        {
            grid[x, y].southWall = true;
            if (y < height-1)
            {
                grid[x, y+1].northWall = true;
            }
        }

        public List<Tile> SolveMaze(Tile start, Tile end)
        {
            Queue<Tile> queue = new Queue<Tile>();
            Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>(); 
            queue.Enqueue(start);
            cameFrom[start] = null;

            while (queue.Count > 0)
            {
                Tile current = queue.Dequeue();

                if (current == end)
                {
                    List<Tile> path = new List<Tile>();
                    while (current != null)
                    {
                        path.Add(current);
                        current = cameFrom[current];
                    }
                    path.Reverse();
                    return path;
                }

                
                foreach (Tile neighbor in GetNeighbors(current))
                {
                    if (!cameFrom.ContainsKey(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        cameFrom[neighbor] = current;
                    }
                }
            }

            return null;
        }

        public List<Tile> GetNeighbors(Tile tile)
        {
            List<Tile> neighbors = new List<Tile>();

            if (tile.isValidMove(MoveDirection.Up))
            {
                neighbors.Add(grid[tile.x, tile.y-1]);
            }
            if (tile.isValidMove(MoveDirection.Down))
            {
                neighbors.Add(grid[tile.x, tile.y + 1]);
            }
            if (tile.isValidMove(MoveDirection.Left))
            {
                neighbors.Add(grid[tile.x - 1, tile.y]);
            }
            if (tile.isValidMove(MoveDirection.Right))
            {
                neighbors.Add(grid[tile.x + 1, tile.y]);
            }

            return neighbors;
        }

        public Tile[,] GetGrid()
        {
            return grid;
        }
    }
}
