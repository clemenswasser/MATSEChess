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
        private ChessBoard board = new ChessBoard();

        private ChessBoardPosition? selectedPos;
        private ChessColor currentPlayer;

        public MainWindow()
        {
            InitializeComponent();

            renderer = new ChessRenderer();
            renderer.Initialize();
        }

        private void ResetGame()
        {
            board.Reset();
            selectedPos = null;
            currentPlayer = ChessColor.WHITE;
        }

        private void Rerender()
        {
            boardImage.Source = renderer.Render(board, selectedPos, (int)tileSize);
        }

        private void OnBoardMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(boardImage);
            int relativeX = (int)(pos.X / tileSize);
            int relativeY = (int)(pos.Y / tileSize);

            var boardPos = new ChessBoardPosition(relativeX, relativeY);
            ChessPiece? selected = board.GetPositionPiece(boardPos);
            if (selected != null && selected.Color == currentPlayer)
            {
                selectedPos = boardPos;
                Rerender();
            }

        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            tileSize = Math.Min(ActualWidth, ActualHeight-50) / 8.0;
            ResetGame();
            Rerender();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            tileSize = Math.Min(ActualWidth, ActualHeight-50) / 8.0;
            Rerender();
        }

    }
}

