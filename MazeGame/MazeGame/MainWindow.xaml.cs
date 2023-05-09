using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MazeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Maze maze;
        private DispatcherTimer timer;
        private DateTime startTime;
        private bool started = false;
        private Tile current;
        private List<Tile> path = new List<Tile> { };
        private List<Tile> solvePath = new List<Tile> { };
        private int difficulty = 0;
        private int customSize = 20;
        int tileSize;
        private bool keyMode = false;
        private bool key = false;
        private Tile? keyTile = null;
        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            RestartMenuItem.Visibility = Visibility.Collapsed;
            TimerMenuItem.Visibility = Visibility.Collapsed;
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private  async void InitializeMaze(int difficulty)
        {

            switch (difficulty)
            {
                case 0:
                    maze = new Maze(20, 20);
                    break;
                case 1:
                    maze = new Maze(50, 50);
                    break;
                case 2:
                    maze = new Maze(100, 100);
                    break;
                default:
                    maze = new Maze(customSize, customSize);
                    break;


            }
            await maze.GenerateAsync();
            current = maze.getTile(0, 0);
            if (keyMode)
            {
                Random rnd = new Random();
                int rx = rnd.Next(1, maze.width-2);
                int ry = rnd.Next(1, maze.height-2);
                keyTile = maze.getTile(rx, ry);
                keyTile.type = TileType.key;

            }
            path.Add(current);
            DrawMaze(maze.GetGrid());
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan timeElapsed = DateTime.Now - startTime;
            TimerMenuItem.Header = timeElapsed.ToString(@"mm\:ss");
        }

        private void EasyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CustomSizeMenuItem.Header = $"Custom Size";
            difficulty = 0;
        }

        private void MediumMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CustomSizeMenuItem.Header = $"Custom Size";
            difficulty = 1;
        }

        private void HardMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CustomSizeMenuItem.Header = $"Custom Size";
            difficulty = 2;
        }
        private void KeyModeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!started)
            {
                if (KeyModeMenuItem.Effect is DropShadowEffect)
                {
                    keyMode = false;
                    KeyModeMenuItem.Effect = null;
                }
                else
                {
                    keyMode = true;
                    KeyModeMenuItem.Effect = (DropShadowEffect)FindResource("ButtonGlow");
                }
            }
            
        }



        private void StartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TimerMenuItem.Visibility = Visibility.Visible;
            StartMenuItem.Visibility = Visibility.Collapsed;
            RestartMenuItem.Visibility = Visibility.Visible;
            InitializeMaze(difficulty);
            startTime = DateTime.Now;
            started = true;
            timer.Start();
        }

        private void RestartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MazeCanvas.Children.Clear();
            timer.Stop();
            StartMenuItem.Visibility = Visibility.Visible;
            RestartMenuItem.Visibility = Visibility.Collapsed;
            TimerMenuItem.Visibility = Visibility.Collapsed;
            path.Clear();
            solvePath.Clear();
            started = false;
            current = null;
            key = false;
            InitializeTimer();
        }
        private void SolveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (started)
            {
                if (keyMode)
                {
                   solvePath =  maze.SolveMaze(maze.getTile(0, 0), keyTile).Concat(maze.SolveMaze(keyTile, maze.getTile(maze.width - 1, maze.height - 1))).ToList();
                }
                else
                {
                    solvePath = maze.SolveMaze(maze.getTile(0, 0), maze.getTile(maze.width - 1, maze.height - 1));
                }
                DisplayPath();
            }
           
        }

        private void CustomSizeButton_Click(object sender, RoutedEventArgs e)
        {
            
            string customSize = CustomSizeTextBox.Text;

            
            if (int.TryParse(customSize, out int size))
            {
                if (size >= 10 && size <= 200)
                {
                    CustomSizeMenuItem.Header = $"Size: {customSize}";
                    this.customSize = size;
                    difficulty = 4;
                }
                else
                {
                    MessageBox.Show("Please enter a size between 10 and 200.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid integer.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            
            CustomSizeTextBox.Clear();
        }

        private void DisplayPath()
        {
            foreach (Tile tile in solvePath) {
                if(tile.type == TileType.none)
                {
                    tile.type = TileType.path;
                }

                DrawCircle(tile.x, tile.y, tileSize);
            }
        }











        public void DrawMaze(Tile[,] grid)
        {
            int minDimension = Math.Min((int)ActualWidth-100, (int)ActualHeight-100);
            tileSize = minDimension/ grid.GetLength(0);

            MazeCanvas.Width = tileSize * grid.GetLength(0);
            MazeCanvas.Height = tileSize * grid.GetLength(1);

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    Tile tile = grid[i, j];
                    int x = i * tileSize;
                    int y = j * tileSize;

                    if (tile.northWall)
                    {
                       DrawLine(x, y, x + tileSize, y);
                    }

                    if (tile.southWall)
                    {
                       DrawLine(x, y + tileSize, x + tileSize, y + tileSize);
                    }

                    if (tile.westWall)
                    {
                        DrawLine(x, y, x, y + tileSize);
                    }

                    if (tile.eastWall)
                    {
                        DrawLine(x + tileSize, y, x + tileSize, y + tileSize);
                    }
                    DrawCircle(i, j, tileSize);
                    //TextBox textBox = new TextBox
                    //{
                    //    Width = tileSize,
                    //    Height = tileSize,
                    //    Text = $"{tile.x}, {tile.y}",
                    //    FontWeight = FontWeights.Bold,
                    //    FontSize = 10,
                    //    IsReadOnly = true,
                    //    Background = Brushes.Transparent,
                    //    BorderThickness = new Thickness(0),
                    //    HorizontalContentAlignment = HorizontalAlignment.Center,
                    //    VerticalContentAlignment = VerticalAlignment.Center
                    //};

                    //Canvas.SetLeft(textBox, x);
                    //Canvas.SetTop(textBox, y);
                    //MazeCanvas.Children.Add(textBox);

                }
            }
        }

        private void DrawCircle(int x, int y, int size)
        {
            Tile[,] grid = maze.GetGrid();
            Ellipse? circle;

            if (grid[x,y].type == TileType.start)
            {
                circle = new Ellipse
                {
                    Width = size,
                    Height = size,
                    Fill = Brushes.Green
                };
                Canvas.SetLeft(circle, x*size);
                Canvas.SetTop(circle, y*size);
                MazeCanvas.Children.Add(circle);
            }
            else if (grid[x,y].type == TileType.end){
                circle = new Ellipse
                {
                    Width = size,
                    Height = size,
                    Fill = Brushes.Red
                };
                Canvas.SetLeft(circle, x*size);
                Canvas.SetTop(circle, y*size);
                MazeCanvas.Children.Add(circle);
            }
            else if (grid[x,y] == current)
            {
                circle = grid[x, y].getCircle();

                if(circle != null)
                {
                    MazeCanvas.Children.Remove(circle);
                }

                circle = new Ellipse
                {
                    Width = size,
                    Height = size,
                    Fill = Brushes.Green
                };
                grid[x,y].addCircle(circle);
                Canvas.SetLeft(circle, x * size);
                Canvas.SetTop(circle, y * size);
                MazeCanvas.Children.Add(circle);

            }
            else if (grid[x, y].type == TileType.path)
            {
                circle = new Ellipse
                {
                    Width = size,
                    Height = size,
                    Fill = Brushes.Aqua,
                    Opacity= 0.5
                };
                Canvas.SetLeft(circle, x * size);
                Canvas.SetTop(circle, y * size);
                MazeCanvas.Children.Add(circle);

            }
            else if (grid[x,y].type == TileType.key)
            {
                circle = new Ellipse
                {
                    Width = size,
                    Height = size,
                    Fill = Brushes.Gold
                };
                grid[x,y].addCircle(circle);
                Canvas.SetLeft(circle, x * size);
                Canvas.SetTop(circle, y * size);
                MazeCanvas.Children.Add(circle);
            }
            else if (path.Contains(grid[x,y]))
            {
                circle = grid[x, y].getCircle();

                if (circle != null)
                {
                    MazeCanvas.Children.Remove(circle);
                }

                circle = new Ellipse
                {
                    Width = size,
                    Height = size,
                    Fill = Brushes.Gray,
                    Opacity = 0.5
                };
                grid[x, y].addCircle(circle);
                Canvas.SetLeft(circle, x * size);
                Canvas.SetTop(circle, y * size);
                MazeCanvas.Children.Add(circle);
            }
            

        }

        private void CheckWin()
        {
            if (keyMode && key )
            {
                if (current.type == TileType.end)
                {

                    timer.Stop();
                    started = false;

                    TimeSpan timeElapsed = DateTime.Now - startTime;
                    MessageBox.Show($"Congratulations! You completed the maze in {timeElapsed.ToString(@"mm\:ss")} ", "You Won!");
                }
            }
            else if(!keyMode)
            {
                if (current.type == TileType.end)
                {

                    timer.Stop();
                    started = false;

                    TimeSpan timeElapsed = DateTime.Now - startTime;
                    MessageBox.Show($"Congratulations! You completed the maze in {timeElapsed.ToString(@"mm\:ss")} ", "You Won!");
                }
            }
            

        }

        private void checkKey()
        {
            if (keyMode && current.type == TileType.key)
            {
                key = true;
            }
        }


        private void drawPath(Tile current, Tile last)
        {
            if (path.Contains(current))
            {
                MazeCanvas.Children.Remove(last.getCircle());
                path.Remove(last);
                path.Remove(current);
                
            }
            else
            {
                DrawCircle(last.x, last.y, tileSize);
                
            }
            DrawCircle(current.x, current.y, tileSize);
            path.Add(current);
            
        }

        private void moveDown(Tile current)
        {
            if (current.isValidMove(MoveDirection.Down))
            {
                 Tile lastTile = current;
                 this.current = maze.GetGrid()[current.x, current.y+1];
                
                
                drawPath(this.current,lastTile);
                checkKey();
                CheckWin();

                //DrawCircle(this.current.x, this.current.y, tileSize);

            }
        }
        private void moveUp(Tile current)
        {
            if (current.isValidMove(MoveDirection.Up))
            {
                Tile lastTile = current;
                this.current = maze.GetGrid()[current.x, current.y-1];
                drawPath(this.current, lastTile);
                checkKey();
                CheckWin();
                //DrawCircle(lastTile.x, lastTile.y, tileSize);
                //DrawCircle(this.current.x, this.current.y, tileSize);
            }
        }

        private void moveLeft(Tile current)
        {
            if (current.isValidMove(MoveDirection.Left))
            {
                Tile lastTile = current;
                this.current = maze.GetGrid()[current.x-1, current.y];
               
                drawPath(this.current, lastTile);
                checkKey();
                CheckWin();
                //DrawCircle(lastTile.x, lastTile.y, tileSize);
                //DrawCircle(this.current.x, this.current.y, tileSize);
            }
        }

        private void moveRight(Tile current)
        {
            if (current.isValidMove(MoveDirection.Right))
            {
                Tile lastTile = current;
                this.current = maze.GetGrid()[current.x+1, current.y];
                
                drawPath(this.current, lastTile);
                checkKey();
                CheckWin();
                // DrawCircle(lastTile.x, lastTile.y, tileSize);
                // DrawCircle(this.current.x, this.current.y, tileSize);
            }
        }




        private void DrawLine(double x1, double y1, double x2, double y2)
        {
            Line line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            MazeCanvas.Children.Add(line);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if(current != null && started)
            {
                switch (e.Key)
                {
                    case Key.W:
                    case Key.Up:
                        moveUp(current);
                        break;
                    case Key.A:
                    case Key.Left:
                        moveLeft(current);
                        break;
                    case Key.S:
                    case Key.Down:
                        moveDown(current);
                        break;
                    case Key.D:
                    case Key.Right:
                        moveRight(current);
                        break;
                }
            }

        }




    }

}


