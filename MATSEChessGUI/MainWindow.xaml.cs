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
        private ChessBoard board = new ChessBoard();

        private ChessBoardPosition? selectedPos;
        private ChessColor currentPlayer;

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
            board.Reset();
            selectedPos = null;
            currentPlayer = ChessColor.WHITE;
        }

        private void Rerender()
        {
            boardImage.Source = renderer.Render(board, selectedPos);
        }

        private void OnBoardMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(boardImage);
            int relativeX = (int)Math.Floor(pos.X / TILE_SIZE);
            int relativeY = (int)Math.Floor(pos.Y / TILE_SIZE);

            var boardPos = new ChessBoardPosition(relativeX, relativeY);
            ChessPiece? selected = board.GetPositionPiece(boardPos);
            if(selected != null && selected.Color == currentPlayer)
            {
                selectedPos = boardPos;
                Rerender();
            }

        }
    }
}

