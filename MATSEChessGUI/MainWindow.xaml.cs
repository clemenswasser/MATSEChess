using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MATSEChess;

namespace MATSEChessGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int TILE_SIZE = 64;
        private ChessRenderer renderer;

        public MainWindow()
        {
            InitializeComponent();
            
            renderer = new ChessRenderer(TILE_SIZE);
            renderer.Initialize();

            Rerender();
        }

        private void Rerender()
        {
            ChessBoard board = new ChessBoard();
            board.Reset();

            boardImage.Source = renderer.Render(board);
        }

        private void OnBoardMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(boardImage);
            int relativeX = (int)Math.Floor(pos.X / TILE_SIZE);
            int relativeY = (int)Math.Floor(pos.Y / TILE_SIZE);
            MessageBox.Show($"X: {relativeX}, Y: {relativeY}");
        }
    }
}
