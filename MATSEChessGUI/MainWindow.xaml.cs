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
        private static int tileSize;
        private static int boardSize;
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
            boardImage.Source = ChessRenderer.Render(game.Board, game.Selection, errorPos, tileSize, boardSize);
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
            var delta = (boardSize - tileSize * 8) / 2;
            int relativeX = (int)((pos.X - delta) / tileSize);
            int relativeY = (int)((pos.Y - delta) / tileSize);;

            var boardPos = new ChessBoardPosition(relativeX, relativeY);
            var success = game.SetSelection(boardPos);
            errorPos = success ? null : boardPos;

            Rerender();
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Dummy BitMap
            var bmp = new Bitmap(1, 1);
            boardImage.Source = ChessRenderer.BitmapToImageSource(bmp);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            boardSize = (int)Math.Round(Math.Min(boardCanvas.ActualWidth, boardCanvas.ActualHeight));
            tileSize = (int)Math.Floor(boardSize / 8.0);

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
            if (MessageBox.Show("Import from Clipboard?", "Chess Import", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                var result = game.Board.FromFENString(Clipboard.GetText());
                if(result != null && result.Length > 0)
                {
                    MessageBox.Show($"Import Failed: {result}", "Chess Import", MessageBoxButton.OK, MessageBoxImage.Error);
                    ResetGame();
                }
            }
            errorPos = null;
            Rerender();
        }
    }
}

