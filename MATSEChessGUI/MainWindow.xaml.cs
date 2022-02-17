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
        private static int tileSize;
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
                res = Microsoft.VisualBasic.Interaction.InputBox("To which piece do you want to promote? (Q=Queen, K=Knight, R=Rook, B=Bishop)", "Pawn Promotion", "Q").Trim().ToUpper();
            } while (res == null || res.Length != 1);


            if (res.Equals("K"))
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
                currentPlayerText.Text = $"Winner: {ChessUtils.ColorToString(winner)}";
            }
            else
            {
                currentPlayerText.Text = $"Current Player: {ChessUtils.ColorToString(game.Board.CurrentPlayer)}";
            }

            fullmoveText.Text = $"Fullmove counter: {game.Board.FullmoveCounter}";
            halfmoveText.Text = $"Halfmove clock: {game.Board.HalfmoveClock}";
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
            // Dummy BitMap
            var bmp = new Bitmap(1, 1);
            boardImage.Source = ChessRenderer.BitmapToImageSource(bmp);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            tileSize = (int)Math.Floor(Math.Min(boardCanvas.ActualWidth, boardCanvas.ActualHeight) / 8.0);

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

