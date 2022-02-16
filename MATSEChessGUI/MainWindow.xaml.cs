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
        private ChessGame game = new ChessGame();
        private ChessBoardPosition? errorPos;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ResetGame()
        {
            game.Reset();
        }

        private void Rerender()
        {
            boardImage.Source = ChessRenderer.Render(game.Board, game.Selection, errorPos, (int)tileSize);
            ChessColor winner = game.Winner;

            if(game.Winner != ChessColor.NONE)
            {
                currentPlayerText.Text = $"Winner: {ChessUtils.ColorToString(winner)}";
            } else
            {
                currentPlayerText.Text = $"Current Player: {ChessUtils.ColorToString(game.CurrentPlayer)}";
            }
        }

        private void OnBoardMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(boardImage);
            int relativeX = (int)(pos.X / tileSize);
            int relativeY = (int)(pos.Y / tileSize);

            var boardPos = new ChessBoardPosition(relativeX, relativeY);
            var success = game.SetSelection(boardPos);
            errorPos = success ? null : boardPos;
            Rerender();
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Calculate a initial size from the window dimensions
            tileSize = Math.Min(ActualWidth, ActualHeight - 40) / 8.0;
            Rerender();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            tileSize = Math.Min(boardImage.ActualWidth, boardImage.ActualHeight) / 8.0;

            if (tileSize < 1) return;

            Rerender();
        }

        private void OnResetClicked(object sender, RoutedEventArgs e)
        {
            ResetGame();
            Rerender();
        }
    }
}

