using MATSEChess;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MATSEChessGUI
{
    internal class ChessRenderer
    {
        private readonly Brush BRIGHT_BRUSH = new SolidBrush(Color.FromArgb(255, 240, 217, 181));
        private readonly SolidBrush DARK_BRUSH = new SolidBrush(Color.FromArgb(255, 181, 136, 99));
        private readonly Brush SELECTION_BRUSH = new SolidBrush(Color.FromArgb(255, 130, 151, 105));
        private ChessPieceImageManager manager = new ChessPieceImageManager();

        public ChessRenderer()
        {
        }

        public void Initialize()
        {
            manager.LoadImages();
        }

        public BitmapImage Render(ChessBoard board, ChessBoardPosition? selectedPos, int tileSize)
        {
            var bitmap = DrawBasicBoard(tileSize);
            DrawSelection(bitmap, board, selectedPos, tileSize);
            DrawPieces(bitmap, board, tileSize);
            return BitmapToImageSource(bitmap);
        }

        private Bitmap DrawBasicBoard(int tileSize)
        {
            var bitmap = new Bitmap(tileSize * 8, tileSize * 8);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                for (int x = 0; x < 8; ++x)
                {
                    for (int y = 0; y < 8; ++y)
                    {
                        Brush b = ((x + y) % 2 == 0) ? DARK_BRUSH : BRIGHT_BRUSH;
                        g.FillRectangle(b, GetTileRectangle(x, y, tileSize));
                    }
                }
            }

            return bitmap;
        }

        private static Dictionary<(ChessColor, ChessPieceType), string> dict = new Dictionary<(ChessColor, ChessPieceType), string>()
        {
            { (ChessColor.WHITE, ChessPieceType.BISHOP), "/resources/Chess_bdt45.svg" },
            { (ChessColor.BLACK, ChessPieceType.BISHOP), "/resources/Chess_blt45.svg" },
            { (ChessColor.WHITE, ChessPieceType.KING), "/resources/Chess_kdt45.svg" },
            { (ChessColor.BLACK, ChessPieceType.KING), "/resources/Chess_klt45.svg" },
            { (ChessColor.WHITE, ChessPieceType.KNIGHT), "/resources/Chess_ndt45.svg" },
            { (ChessColor.BLACK, ChessPieceType.KNIGHT), "/resources/Chess_nlt45.svg" },
            { (ChessColor.WHITE, ChessPieceType.PAWN), "/resources/Chess_pdt45.svg" },
            { (ChessColor.BLACK, ChessPieceType.PAWN), "/resources/Chess_plt45.svg" },
            { (ChessColor.WHITE, ChessPieceType.QUEEN), "/resources/Chess_qdt45.svg" },
            { (ChessColor.BLACK, ChessPieceType.QUEEN), "/resources/Chess_qlt45.svg" },
            { (ChessColor.WHITE, ChessPieceType.ROOK), "/resources/Chess_rdt45.svg" },
            { (ChessColor.BLACK, ChessPieceType.ROOK), "/resources/Chess_rlt45.svg" },
        };

        private void DrawPieces(Bitmap bitmap, ChessBoard board, int tileSize)
        {
            using Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (var piece in board.Pieces)
            {
                var uri = new Uri(dict[(piece.Color, piece.Type)], UriKind.Relative);
                var p = Application.GetResourceStream(uri);
                var document = Svg.SvgDocument.Open<Svg.SvgDocument>(p.Stream);
                g.DrawImageUnscaled(document.Draw(tileSize, tileSize), tileSize * piece.Position.X - 2, tileSize * piece.Position.Y - 2);
            }
        }

        private void DrawSelection(Bitmap bitmap, ChessBoard board, ChessBoardPosition? selectedPos, int tileSize)
        {
            if (selectedPos == null)
                return;

            var selected = board.GetPositionPiece(selectedPos);
            if (selected == null)
                return;

            using Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.FillRectangle(SELECTION_BRUSH, GetTileRectangle(selectedPos.X, selectedPos.Y, tileSize));

            foreach (var pos in selected.GetPossibleMoves(board))
            {
                g.FillEllipse(SELECTION_BRUSH, GetTileCircle(pos.X, pos.Y, tileSize));
            }
        }

        private Rectangle GetTileRectangle(int x, int y, int tileSize)
        {
            return new Rectangle(tileSize * x, tileSize * y, tileSize, tileSize);
        }

        private Rectangle GetTileCircle(int x, int y, int tileSize)
        {
            var rect = GetTileRectangle(x, y, tileSize);
            return new Rectangle(rect.X + rect.Width / 4, rect.Y + rect.Height / 4, rect.Width / 2, rect.Height / 2);
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
