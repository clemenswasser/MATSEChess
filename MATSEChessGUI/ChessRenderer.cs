﻿using MATSEChess;
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

        public static BitmapImage Render(ChessBoard board, ChessBoardPosition? selectedPos, ChessBoardPosition? errorPos, int tileSize)
        {
            var bitmap = DrawBasicBoard(tileSize);
            DrawSelection(bitmap, board, selectedPos, tileSize);
            DrawTile(bitmap, errorPos, tileSize, ERROR_BRUSH);
            DrawPieces(bitmap, board, tileSize);
            return BitmapToImageSource(bitmap);
        }

        private static Bitmap DrawBasicBoard(int tileSize)
        {
            var bitmap = new Bitmap(tileSize * 8, tileSize * 8);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                for (int x = 0; x < 8; ++x)
                {
                    for (int y = 0; y < 8; ++y)
                    {
                        g.FillRectangle(GetBrushFor(x, y), GetTileRectangle(x, y, tileSize));
                    }
                }
            }

            return bitmap;
        }

        private static void DrawPieces(Bitmap bitmap, ChessBoard board, int tileSize)
        {
            using Graphics g = Graphics.FromImage(bitmap);
            foreach (var piece in board.Pieces)
            {
                g.DrawImageUnscaled(ChessPieceImageManager.GetImageTile(piece.Type, piece.Color, tileSize),
                                    tileSize * piece.Position.X, tileSize * piece.Position.Y);
            }
        }

        private static void DrawSelection(Bitmap bitmap, ChessBoard board, ChessBoardPosition? selectedPos, int tileSize)
        {
            if (selectedPos == null)
                return;

            var selected = board.GetPositionPiece(selectedPos);
            if (selected == null)
                return;

            using Graphics g = Graphics.FromImage(bitmap);
            g.FillRectangle(SELECTION_BRUSH, GetTileRectangle(selectedPos.X, selectedPos.Y, tileSize));

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (var pos in selected.GetPossibleMoves(board))
            {
                if (board.GetPositionState(pos) == ChessColor.NONE)
                {
                    g.FillEllipse(SELECTION_BRUSH, GetTileCircle(pos.X, pos.Y, tileSize, 0.3));
                }
                else
                {
                    g.FillRectangle(SELECTION_BRUSH, GetTileRectangle(pos.X, pos.Y, tileSize));
                    g.FillEllipse(GetBrushFor(pos.X, pos.Y), GetTileCircle(pos.X, pos.Y, tileSize));
                }
            }
        }

        private static void DrawTile(Bitmap bitmap, ChessBoardPosition? pos, int tileSize, Brush b)
        {
            if (pos == null || !pos.Valid)
            {
                return;
            }

            using Graphics g = Graphics.FromImage(bitmap);
            g.FillRectangle(b, GetTileRectangle(pos.X, pos.Y, tileSize));
        }

        private static Rectangle GetTileRectangle(int x, int y, int tileSize)
        {
            return new Rectangle(tileSize * x, tileSize * y, tileSize, tileSize);
        }

        private static Brush GetBrushFor(int x, int y)
        {
            return ((x + y) % 2 == 0) ? DARK_BRUSH : BRIGHT_BRUSH;
        }

        private static Rectangle GetTileCircle(int x, int y, int tileSize, double sizeMult = 1)
        {
            var rect = GetTileRectangle(x, y, tileSize);

            int width = (int)(rect.Width * sizeMult);
            int height = (int)(rect.Height * sizeMult);
            int adjX = rect.X + Math.Abs(rect.Width - width) / 2;
            int adjY = rect.Y + Math.Abs(rect.Height - height) / 2;

            return new Rectangle(adjX, adjY, width, height);
        }

        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
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
