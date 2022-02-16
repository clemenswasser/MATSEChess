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
        private static double tileSize;
        private ChessRenderer renderer;
        private ChessGame game = new ChessGame();

        public MainWindow()
        {
            InitializeComponent();

            renderer = new ChessRenderer();
            renderer.Initialize();
        }

        private void ResetGame()
        {
            game.Reset();
        }

        private void Rerender()
        {
            boardImage.Source = renderer.Render(game.Board, game.Selection, (int)tileSize);
        }

        private void OnBoardMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(boardImage);
            int relativeX = (int)(pos.X / tileSize);
            int relativeY = (int)(pos.Y / tileSize);

            var boardPos = new ChessBoardPosition(relativeX, relativeY);
            game.Selection = boardPos;
            Rerender();
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            tileSize = Math.Min(ActualWidth, ActualHeight-40) / 8.0;
            ResetGame();
            Rerender();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            tileSize = Math.Min(boardImage.ActualWidth, boardImage.ActualHeight) / 8.0;

            if (tileSize <= 0) return;

            Rerender();
        }

    }
}

