using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Weiqi;

public enum StoneColor
{
    Invalid,
    Empty,
    Black,
    White
}

public class Stone
{
    public StoneColor Color { get; set; }
}

/// <summary>  
/// Interaction logic for MainWindow.xaml  
/// </summary>  
public partial class MainWindow : Window
{
    private const int BoardSize = 19;
    private const double BoardMargin = 20;

    private static (int x, int y)[] _directions = { (-1, 0), (1, 0), (0, -1), (0, 1) };

    private readonly Stone?[,] _stonePositions = new Stone[BoardSize, BoardSize]; // Stores stone objects  
    private StoneColor _nextMover = StoneColor.Black; 

    public MainWindow()
    {
        InitializeComponent();
        SizeChanged += (s, e) => DrawGoBoard();
        GoBoard.MouseLeftButtonDown += OnBoardClick;
        DrawGoBoard();
    }

    private void DrawGoBoard()
    {
        // Clear existing children to redraw on resize  
        GoBoard.Children.Clear();

        // Calculate cell size dynamically based on window size  
        double cellSize = Math.Min(
            (GoBoard.ActualWidth - 2 * BoardMargin) / (BoardSize - 1),
            (GoBoard.ActualHeight - 2 * BoardMargin) / (BoardSize - 1)
        );

        for (int i = 0; i < BoardSize; i++)
        {
            // Draw vertical lines  
            var verticalLine = new Line
            {
                X1 = BoardMargin + i * cellSize,
                Y1 = BoardMargin,
                X2 = BoardMargin + i * cellSize,
                Y2 = BoardMargin + (BoardSize - 1) * cellSize,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            GoBoard.Children.Add(verticalLine);

            // Draw horizontal lines  
            var horizontalLine = new Line
            {
                X1 = BoardMargin,
                Y1 = BoardMargin + i * cellSize,
                X2 = BoardMargin + (BoardSize - 1) * cellSize,
                Y2 = BoardMargin + i * cellSize,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            GoBoard.Children.Add(horizontalLine);
        }

        // Define star point positions for a 19x19 board  
        int[] starPoints = { 3, 9, 15 };

        foreach (int x in starPoints)
        {
            foreach (int y in starPoints)
            {
                var starPoint = new Ellipse
                {
                    Width = 5,
                    Height = 5,
                    Fill = Brushes.Black
                };

                Canvas.SetLeft(starPoint, BoardMargin + x * cellSize - starPoint.Width / 2);
                Canvas.SetTop(starPoint, BoardMargin + y * cellSize - starPoint.Height / 2);
                GoBoard.Children.Add(starPoint);
            }
        }

        // Redraw existing stones  
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                if (_stonePositions[x, y] != null)
                {
                    DrawStone(x, y, _stonePositions[x, y].Color, cellSize);
                }
            }
        }
    }

    private void OnBoardClick(object sender, MouseButtonEventArgs e)
    {
        // Calculate cell size dynamically based on window size  
        double cellSize = Math.Min(
            (GoBoard.ActualWidth - 2 * BoardMargin) / (BoardSize - 1),
            (GoBoard.ActualHeight - 2 * BoardMargin) / (BoardSize - 1)
        );

        // Get mouse position and calculate nearest grid point  
        Point clickPosition = e.GetPosition(GoBoard);
        int x = (int)Math.Round((clickPosition.X - BoardMargin) / cellSize);
        int y = (int)Math.Round((clickPosition.Y - BoardMargin) / cellSize);

        // Ensure the click is within bounds  
        if (x < 0 || x >= BoardSize || y < 0 || y >= BoardSize || _stonePositions[x, y] != null)
            return;

        // Alternate between black and white stones  
        _stonePositions[x, y] = new Stone { Color = _nextMover };

        // Draw the stone  
        DrawStone(x, y, _nextMover, cellSize);

        // Check for dead stones and remove them  
        RemoveDeadStones(x, y, _nextMover);

        _nextMover = _nextMover == StoneColor.Black ? StoneColor.White : StoneColor.Black;
    }

    private void RemoveDeadStones(int x, int y, StoneColor color)
    {
        var visited = new HashSet<(int, int)>();
        var toRemove = new List<(int, int)>();

        foreach (var diff in _directions)
        {
            int x0 = x + diff.Item1;

            if (x0 >= 0 && x0 < BoardSize)
            {
                int y0 = y + diff.Item2;

                if (y0 >= 0 && y0 < BoardSize && _stonePositions[x0, y0] != null && _stonePositions[x0, y0].Color != color)
                {
                    var group = new List<(int, int)>();
                    var liberties = CalculateLiberties((x0, y0), group, visited);

                    if (liberties == 0)
                    {
                        toRemove.AddRange(group);
                    }
                }
            }
        }

        if (toRemove.Count > 0)
        { 
            foreach (var (_x, _y) in toRemove)
            {
                _stonePositions[_x, _y] = null;
            }

            DrawGoBoard(); // Redraw the board to reflect changes
        }
    }

    private int CalculateLiberties((int x, int y) position, List<(int, int)> group, HashSet<(int, int)> visited)
    {
        var stack = new Stack<(int, int)>();
        stack.Push(position);
        visited.Add(position);
        group.Add(position);

        StoneColor color = _stonePositions[position.x, position.y].Color;
        int liberties = 0;

        while (stack.Count > 0)
        {
            var (x, y) = stack.Pop();

            foreach (var (nx, ny) in GetNeighbors(x, y))
            {
                if (nx < 0 || nx >= BoardSize || ny < 0 || ny >= BoardSize)
                    continue;

                if (_stonePositions[nx, ny] == null)
                {
                    liberties++;
                }
                else if (_stonePositions[nx, ny].Color == color && !visited.Contains((nx, ny)))
                {
                    visited.Add((nx, ny));
                    group.Add((nx, ny));
                    stack.Push((nx, ny));
                }
            }
        }

        return liberties;
    }

    private IEnumerable<(int, int)> GetNeighbors(int x, int y)
    {
        yield return (x - 1, y);
        yield return (x + 1, y);
        yield return (x, y - 1);
        yield return (x, y + 1);
    }

    private void DrawStone(int x, int y, StoneColor color, double cellSize)
    {
        var stone = new Ellipse
        {
            Width = cellSize * 0.8,
            Height = cellSize * 0.8,
            Fill = color == StoneColor.Black ? Brushes.Black : Brushes.White,
            Stroke = Brushes.Black,
            StrokeThickness = 1
        };

        Canvas.SetLeft(stone, BoardMargin + x * cellSize - stone.Width / 2);
        Canvas.SetTop(stone, BoardMargin + y * cellSize - stone.Height / 2);
        GoBoard.Children.Add(stone);
    }
}

