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
    // Add event handler for window size changes
    public MainWindow()
    {
        InitializeComponent();
        SizeChanged += (s, e) => DrawGoBoard();
        DrawGoBoard();
    }

    private void DrawGoBoard()
    {
        const int boardSize = 19;
        const double margin = 20;

        // Clear existing children to redraw on resize  
        GoBoard.Children.Clear();

        // Calculate cell size dynamically based on window size  
        double cellSize = Math.Min(
            (GoBoard.ActualWidth - 2 * margin) / (boardSize - 1),
            (GoBoard.ActualHeight - 2 * margin) / (boardSize - 1)
        );

        for (int i = 0; i < boardSize; i++)
        {
            // Draw vertical lines  
            var verticalLine = new Line
            {
                X1 = margin + i * cellSize,
                Y1 = margin,
                X2 = margin + i * cellSize,
                Y2 = margin + (boardSize - 1) * cellSize,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            GoBoard.Children.Add(verticalLine);

            // Draw horizontal lines  
            var horizontalLine = new Line
            {
                X1 = margin,
                Y1 = margin + i * cellSize,
                X2 = margin + (boardSize - 1) * cellSize,
                Y2 = margin + i * cellSize,
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

                Canvas.SetLeft(starPoint, margin + x * cellSize - starPoint.Width / 2);
                Canvas.SetTop(starPoint, margin + y * cellSize - starPoint.Height / 2);
                GoBoard.Children.Add(starPoint);
            }
        }
    }
}
