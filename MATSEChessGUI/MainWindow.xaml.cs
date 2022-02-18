using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using MATSEChess;

namespace MATSEChessGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int tileSize = 0;
        private int boardSize = 0;
        private double dpi = 0;
        private ChessGame game = new ChessGame();
        private ChessBoardPosition? errorPos;

        public MainWindow()
        {
            InitializeComponent();
            ResetGame();
        }

        private void ResetGame()
        {
            game.Reset();
            game.Board.OnPromotion = OnPromotion;
            errorPos = null;
        }

        private ChessPieceType OnPromotion()
        {
            string res;
            do
            {
                res = Microsoft.VisualBasic.Interaction.InputBox("To which piece do you want to promote? (Q=Queen, N=Knight, R=Rook, B=Bishop)", "Pawn Promotion", "Q").Trim().ToUpper();
            } while (res == null || res.Length != 1);


            if (res.Equals("N"))
                return ChessPieceType.KNIGHT;
            else if (res.Equals("B"))
                return ChessPieceType.BISHOP;
            else if (res.Equals("R"))
                return ChessPieceType.ROOK;
            else
                return ChessPieceType.QUEEN;
        }

        private void Rerender()
        {
            boardImage.Source = ChessRenderer.Render(game.Board, game.Selection, errorPos, tileSize);
            ChessColor winner = game.Winner;

            if (game.Winner != ChessColor.NONE)
            {
                CurrentPlayerText.Text = $"Winner: {ChessUtils.ColorToString(winner)}";
            }
            else
            {
                CurrentPlayerText.Text = $"Current Player: {ChessUtils.ColorToString(game.Board.CurrentPlayer)}";
            }

            FullmoveText.Text = $"Fullmove counter: {game.Board.FullmoveCounter}";
            HalfmoveText.Text = $"Halfmove clock: {game.Board.HalfmoveClock}";
            CastlingText.Text = $"Castling: {game.Board.GetCastlingString()}";
        }

        private void OnBoardMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(boardImage);
            int relativeX = (int)(pos.X * dpi / tileSize);
            int relativeY = (int)(pos.Y * dpi / tileSize);

            var boardPos = new ChessBoardPosition(relativeX, relativeY);
            var success = game.SetSelection(boardPos);
            errorPos = success ? null : boardPos;

            Rerender();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newBoardSize = (int)Math.Min(boardCanvas.ActualWidth * dpi, boardCanvas.ActualHeight * dpi);
            if (newBoardSize == boardSize) return;

            boardSize = newBoardSize;
            tileSize = (int)(boardSize / 8.0);

            if (tileSize < 1) return;

            Rerender();
        }

        private void OnResetClicked(object sender, RoutedEventArgs e)
        {
            ResetGame();
            Rerender();
        }

        private void OnExportClicked(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Copy to Clipboard?", "Chess Export", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                Clipboard.SetText(game.Board.ToString());
        }

        private void OnImportClicked(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Import from Clipboard?", "Chess Import", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                var result = game.Board.FromFENString(Clipboard.GetText());
                if (result != null && result.Length > 0)
                {
                    MessageBox.Show($"Import Failed: {result}", "Chess Import", MessageBoxButton.OK, MessageBoxImage.Error);
                    ResetGame();
                }
            }
            errorPos = null;
            Rerender();
        }

        private void OnSpecialFeatureTriggered(object sender, MouseButtonEventArgs e)
        {
            ChessPieceImageManager.DeployUnicorns();
            Rerender();
        }

        private void OnDpiChanged(object sender, DpiChangedEventArgs e)
        {
            dpi = e.NewDpi.PixelsPerDip;
        }
    }
}

