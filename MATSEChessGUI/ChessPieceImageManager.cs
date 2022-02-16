using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using MATSEChess;

namespace MATSEChessGUI
{
    public class ChessPieceImageManager
    {
        private static Dictionary<ChessPieceInstance, string> imageResourcePaths = new Dictionary<ChessPieceInstance, string>()
        {
            { new ChessPieceInstance(ChessPieceType.BISHOP, ChessColor.BLACK), "/resources/Chess_bdt45.svg" },
            { new ChessPieceInstance(ChessPieceType.BISHOP, ChessColor.WHITE), "/resources/Chess_blt45.svg" },
            { new ChessPieceInstance(ChessPieceType.KING, ChessColor.BLACK), "/resources/Chess_kdt45.svg" },
            { new ChessPieceInstance(ChessPieceType.KING, ChessColor.WHITE), "/resources/Chess_klt45.svg" },
            { new ChessPieceInstance(ChessPieceType.KNIGHT, ChessColor.BLACK), "/resources/Chess_ndt45.svg" },
            { new ChessPieceInstance(ChessPieceType.KNIGHT, ChessColor.WHITE), "/resources/Chess_nlt45.svg" },
            { new ChessPieceInstance(ChessPieceType.PAWN, ChessColor.BLACK), "/resources/Chess_pdt45.svg" },
            { new ChessPieceInstance(ChessPieceType.PAWN, ChessColor.WHITE), "/resources/Chess_plt45.svg" },
            { new ChessPieceInstance(ChessPieceType.QUEEN, ChessColor.BLACK), "/resources/Chess_qdt45.svg" },
            { new ChessPieceInstance(ChessPieceType.QUEEN, ChessColor.WHITE), "/resources/Chess_qlt45.svg" },
            { new ChessPieceInstance(ChessPieceType.ROOK, ChessColor.BLACK), "/resources/Chess_rdt45.svg" },
            { new ChessPieceInstance(ChessPieceType.ROOK, ChessColor.WHITE), "/resources/Chess_rlt45.svg" },
        };

        private static Dictionary<(ChessPieceInstance, int), Bitmap> imageCache = new Dictionary<(ChessPieceInstance, int), Bitmap>();

        public static Bitmap GetImageTile(ChessPieceType type, ChessColor color, int tileSize)
        {
            var chessPiece = new ChessPieceInstance(type, color);
            var key = (chessPiece, tileSize);
            if (!imageCache.ContainsKey(key))
            {
                var uri = new Uri(imageResourcePaths[chessPiece], UriKind.Relative);
                var p = Application.GetResourceStream(uri);
                imageCache[key] = Svg.SvgDocument.Open<Svg.SvgDocument>(p.Stream).Draw(tileSize, tileSize);
            }
            return imageCache[key];
        }
    }
}

