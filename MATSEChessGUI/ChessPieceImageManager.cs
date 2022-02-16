using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Resources;
using MATSEChess;

namespace MATSEChessGUI
{
    public class ChessPieceImageManager
    {
        private Dictionary<ChessPieceInstance, Bitmap> images = new Dictionary<ChessPieceInstance, Bitmap>();
        private int imageWidth;
        private int imageHeight;
        private Bitmap fallback = new Bitmap(16, 16);
        public void LoadImages()
        {
            var uri = new Uri("/resources/pieces.png", UriKind.Relative);
            var pieces = Application.GetResourceStream(uri);
            Bitmap bmpOrig = new Bitmap(pieces.Stream);


            var pieceOrder = new ChessPieceType[] { ChessPieceType.QUEEN, ChessPieceType.KING, ChessPieceType.ROOK, ChessPieceType.KNIGHT, ChessPieceType.BISHOP, ChessPieceType.PAWN };
            var colorOrder = new ChessColor[] { ChessColor.BLACK, ChessColor.WHITE };

            imageWidth = bmpOrig.Width / pieceOrder.Length;
            imageHeight = bmpOrig.Height / colorOrder.Length;

            ReadImageTiles(bmpOrig, pieceOrder, colorOrder);
            CreateFallbackImage();
        }

        private void ReadImageTiles(Bitmap all, ChessPieceType[] pieceOrder, ChessColor[] colorOrder)
        {
            for (var row = 0; row < colorOrder.Length; row++)
            {
                var color = colorOrder[row];
                for (var col = 0; col < pieceOrder.Length; col++)
                {
                    var piece = pieceOrder[col];
                    images[new ChessPieceInstance(piece, color)] = CopyBitmap(all, new Rectangle(col * imageWidth, row * imageHeight, imageWidth, imageHeight));
                }
            }
        }

        private void CreateFallbackImage()
        {
            fallback = new Bitmap(imageWidth, imageHeight);
            using (Graphics g = Graphics.FromImage(fallback))
            {
                g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, imageWidth, imageHeight));
            }
        }

        public Bitmap GetImageTile(ChessPieceType type, ChessColor color)
        {
            var key = new ChessPieceInstance(type, color);
            if (!images.ContainsKey(key))
                return fallback;
            return images[key];
        }

        static public Bitmap CopyBitmap(Bitmap srcBitmap, Rectangle section)
        {
            // from MSDN
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(srcBitmap, 0, 0, section, GraphicsUnit.Pixel);
            g.Dispose();
            return bmp;
        }
    }


}
