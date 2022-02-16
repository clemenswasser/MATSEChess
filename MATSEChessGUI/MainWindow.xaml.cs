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
        private ChessGame game = new ChessGame();

        public MainWindow()
        {
            InitializeComponent();
            
            renderer = new ChessRenderer(TILE_SIZE);
            renderer.Initialize();

            ResetGame();
            Rerender();
        }

        private void ResetGame()
        {
            game.Reset();
        }

        private void Rerender()
        {
            boardImage.Source = renderer.Render(game.Board, game.Selection);
        }

        private void OnBoardMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(boardImage);
            int relativeX = (int)Math.Floor(pos.X / TILE_SIZE);
            int relativeY = (int)Math.Floor(pos.Y / TILE_SIZE);

            var boardPos = new ChessBoardPosition(relativeX, relativeY);
            game.Selection = boardPos;
            Rerender();
        }
    }
}

