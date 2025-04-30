using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Weiqi;

public enum StoneColor
{
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
    private const double Margin = 20;

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
            (GoBoard.ActualWidth - 2 * Margin) / (BoardSize - 1),
            (GoBoard.ActualHeight - 2 * Margin) / (BoardSize - 1)
        );

        for (int i = 0; i < BoardSize; i++)
        {
            // Draw vertical lines  
            var verticalLine = new Line
            {
                X1 = Margin + i * cellSize,
                Y1 = Margin,
                X2 = Margin + i * cellSize,
                Y2 = Margin + (BoardSize - 1) * cellSize,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            GoBoard.Children.Add(verticalLine);

            // Draw horizontal lines  
            var horizontalLine = new Line
            {
                X1 = Margin,
                Y1 = Margin + i * cellSize,
                X2 = Margin + (BoardSize - 1) * cellSize,
                Y2 = Margin + i * cellSize,
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

                Canvas.SetLeft(starPoint, Margin + x * cellSize - starPoint.Width / 2);
                Canvas.SetTop(starPoint, Margin + y * cellSize - starPoint.Height / 2);
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
            (GoBoard.ActualWidth - 2 * Margin) / (BoardSize - 1),
            (GoBoard.ActualHeight - 2 * Margin) / (BoardSize - 1)
        );

        // Get mouse position and calculate nearest grid point  
        Point clickPosition = e.GetPosition(GoBoard);
        int x = (int)Math.Round((clickPosition.X - Margin) / cellSize);
        int y = (int)Math.Round((clickPosition.Y - Margin) / cellSize);

        // Ensure the click is within bounds  
        if (x < 0 || x >= BoardSize || y < 0 || y >= BoardSize || _stonePositions[x, y] != null)
            return;

        // Alternate between black and white stones  
        _stonePositions[x, y] = new Stone { Color = _nextMover };

        // Draw the stone  
        DrawStone(x, y, _nextMover, cellSize);

        _nextMover = _nextMover == StoneColor.Black ? StoneColor.White : StoneColor.Black;

        // Check for dead stones and remove them  
        RemoveDeadStones();
    }

    private void RemoveDeadStones()
    {
        var visited = new HashSet<(int, int)>();
        var toRemove = new List<(int, int)>();

        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                if (_stonePositions[x, y] != null && !visited.Contains((x, y)))
                {
                    var group = new List<(int, int)>();
                    var liberties = CalculateLiberties((x, y), group, visited);

                    if (liberties == 0)
                    {
                        toRemove.AddRange(group);
                    }
                }
            }
        }

        if (toRemove.Count > 0)
        { 
            foreach (var (x, y) in toRemove)
            {
                _stonePositions[x, y] = null;
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

        Canvas.SetLeft(stone, Margin + x * cellSize - stone.Width / 2);
        Canvas.SetTop(stone, Margin + y * cellSize - stone.Height / 2);
        GoBoard.Children.Add(stone);
    }
}

