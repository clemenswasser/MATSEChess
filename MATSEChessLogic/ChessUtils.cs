using System.Diagnostics.CodeAnalysis;

namespace MATSEChess
{
    public enum ChessColor
    {
        BLACK,
        WHITE,
        NONE
    }

    public enum ChessPieceType
    {
        KING,
        QUEEN,
        ROOK,
        KNIGHT,
        BISHOP,
        PAWN
    }

    public struct ChessPieceInstance
    {
        public ChessPieceType Type;
        public ChessColor Color;

        public ChessPieceInstance(ChessPieceType type, ChessColor color)
        {
            Type = type;
               Color = color;

        }

        public override bool Equals(object? obj)
        {
            if(obj == null)
                return false;
            ChessPieceInstance? other = obj as ChessPieceInstance?;
            if (other == null)
                return false;

            return Type == other?.Type && Color == other?.Color;
        }
    }

    public static class ChessUtils
    {
        public static ChessColor GetOpponentColor(ChessColor color)
        {
            switch (color)
            {
                case ChessColor.BLACK:
                    return ChessColor.WHITE;
                case ChessColor.WHITE:
                    return ChessColor.BLACK;
                default:
                    return ChessColor.NONE;
            }
        }
    }

}
