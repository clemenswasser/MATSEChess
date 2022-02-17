using MATSEChess;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace MATSEChessGUI
{
    internal class ChessRenderer
    {
        private static Brush BRIGHT_BRUSH = new SolidBrush(Color.FromArgb(255, 240, 217, 181));
        private static SolidBrush DARK_BRUSH = new SolidBrush(Color.FromArgb(255, 181, 136, 99));
        private static Brush SELECTION_BRUSH = new SolidBrush(Color.FromArgb(255, 130, 151, 105));
        private static Brush ERROR_BRUSH = new SolidBrush(Color.FromArgb(255, 216, 52, 52));
        public static int tileSize;

        public static BitmapImage Render(ChessBoard board, ChessBoardPosition? selectedPos, ChessBoardPosition? errorPos, int inTileSize)
        {
            tileSize = inTileSize;
            var bitmap = DrawBasicBoard();
            DrawSelection(bitmap, board, selectedPos);
            DrawTile(bitmap, errorPos, ERROR_BRUSH);
            DrawPieces(bitmap, board);
            return BitmapToImageSource(bitmap);
        }

        private static Bitmap DrawBasicBoard()
        {
            var bitmap = new Bitmap(tileSize * 8, tileSize * 8);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                for (int x = 0; x < 8; ++x)
                {
                    for (int y = 0; y < 8; ++y)
                    {
                        Brush b = ((x + y) % 2 == 0) ? DARK_BRUSH : BRIGHT_BRUSH;
                        g.FillRectangle(b, GetTileRectangle(x, y));
                    }
                }
            }

            return bitmap;
        }

        private static void DrawPieces(Bitmap bitmap, ChessBoard board)
        {
            using Graphics g = Graphics.FromImage(bitmap);
            foreach (var piece in board.Pieces)
            {
                g.DrawImageUnscaled(ChessPieceImageManager.GetImageTile(piece.Type, piece.Color, tileSize),
                                    tileSize * piece.Position.X, tileSize * piece.Position.Y);
            }
        }

        private static void DrawSelection(Bitmap bitmap, ChessBoard board, ChessBoardPosition? selectedPos)
        {
            if (selectedPos == null)
                return;

            var selected = board.GetPositionPiece(selectedPos);
            if (selected == null)
                return;

            using Graphics g = Graphics.FromImage(bitmap);
            g.FillRectangle(SELECTION_BRUSH, GetTileRectangle(selectedPos.X, selectedPos.Y));

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (var pos in selected.GetPossibleMoves(board))
            {
                g.FillEllipse(SELECTION_BRUSH, GetTileCircle(pos.X, pos.Y));
            }
        }

        private static void DrawTile(Bitmap bitmap, ChessBoardPosition? pos, Brush b)
        {
            if (pos == null || !pos.Valid)
            {
                return;
            }

            using Graphics g = Graphics.FromImage(bitmap);
            g.FillRectangle(b, GetTileRectangle(pos.X, pos.Y));
        }

        private static Rectangle GetTileRectangle(int x, int y)
        {
            return new Rectangle(tileSize * x, tileSize * y, tileSize, tileSize);
        }

        private static Rectangle GetTileCircle(int x, int y)
        {
            const double sizeMult = 0.3;

            var rect = GetTileRectangle(x, y);

            int width = (int)(rect.Width * sizeMult);
            int height = (int)(rect.Height * sizeMult);
            int adjX = rect.X + Math.Abs(rect.Width - width) / 2;
            int adjY = rect.Y + Math.Abs(rect.Height - height) / 2;

            return new Rectangle(adjX, adjY, width, height);
        }

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
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
