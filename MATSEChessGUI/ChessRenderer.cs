﻿using MATSEChess;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace MATSEChessGUI
{
    internal class ChessRenderer
    {
        private int tileSize;
        private readonly Brush BRIGHT_BRUSH = new SolidBrush(Color.FromArgb(255, 240, 217, 181));
        private readonly SolidBrush DARK_BRUSH = new SolidBrush(Color.FromArgb(255, 181, 136, 99));
        private readonly Brush SELECTION_BRUSH = new SolidBrush(Color.FromArgb(255, 130, 151, 105));
        private ChessPieceImageManager manager = new ChessPieceImageManager();

        public ChessRenderer(int tileSize)
        {
            this.tileSize = tileSize;
        }

        public void Initialize()
        {
            manager.LoadImages();
        }

        public BitmapImage Render(ChessBoard board, ChessBoardPosition? selectedPos)
        {
            var bitmap = DrawBasicBoard();
            DrawSelection(bitmap, board, selectedPos);
            DrawPieces(bitmap, board);
            return BitmapToImageSource(bitmap);
        }

        private Bitmap DrawBasicBoard()
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

        private void DrawPieces(Bitmap bitmap, ChessBoard board)
        {
            using Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (var piece in board.Pieces)
            {
                // TODO: Generify -2 to image centering
                g.DrawImageUnscaled(manager.GetImageTile(piece.Type, piece.Color), new Point(tileSize * piece.Position.X - 2, tileSize * piece.Position.Y - 2));
            }
        }

        private void DrawSelection(Bitmap bitmap, ChessBoard board, ChessBoardPosition? selectedPos)
        {
            if (selectedPos == null)
                return;

            var selected = board.GetPositionPiece(selectedPos);
            if (selected == null)
                return;

            using Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.FillRectangle(SELECTION_BRUSH, GetTileRectangle(selectedPos.X, selectedPos.Y));

            foreach(var pos in selected.GetPossibleMoves(board))
            {
                g.FillEllipse(SELECTION_BRUSH, GetTileCircle(pos.X, pos.Y));
            }
        }

        private Rectangle GetTileRectangle(int x, int y)
        {
            return new Rectangle(tileSize * x, tileSize * y, tileSize, tileSize);
        }

        private Rectangle GetTileCircle(int x, int y)
        {
            var rect = GetTileRectangle(x,y);
            return new Rectangle(rect.X + rect.Width/4, rect.Y+rect.Height/4, rect.Width/2, rect.Height/2);
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
