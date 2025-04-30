using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Weiqi;

/// <summary>  
/// Interaction logic for MainWindow.xaml  
/// </summary>  
public partial class MainWindow : Window
{
    private const int BoardSize = 19;
    private const double Margin = 20;
    private readonly Dictionary<(int, int), string> _stonePositions = new(); // Stores stone positions and colors

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
        foreach (var ((x, y), color) in _stonePositions)
        {
            DrawStone(x, y, color, cellSize);
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
        if (x < 0 || x >= BoardSize || y < 0 || y >= BoardSize || _stonePositions.ContainsKey((x, y)))
            return;

        // Alternate between black and white stones
        string color = _stonePositions.Count % 2 == 0 ? "Black" : "White";
        _stonePositions[(x, y)] = color;

        // Draw the stone
        DrawStone(x, y, color, cellSize);
    }

    private void DrawStone(int x, int y, string color, double cellSize)
    {
        var stone = new Ellipse
        {
            Width = cellSize * 0.8,
            Height = cellSize * 0.8,
            Fill = color == "Black" ? Brushes.Black : Brushes.White,
            Stroke = Brushes.Black,
            StrokeThickness = 1
        };

        Canvas.SetLeft(stone, Margin + x * cellSize - stone.Width / 2);
        Canvas.SetTop(stone, Margin + y * cellSize - stone.Height / 2);
        GoBoard.Children.Add(stone);
    }
}
