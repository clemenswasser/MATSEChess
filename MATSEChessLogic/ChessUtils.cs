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

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Color);
        }
    }

    public enum CastlingType
    {
        BLACK_QUEENSIDE,
        BLACK_KINGSIDE,
        WHITE_QUEENSIDE,
        WHITE_KINGSIDE,
    }

    public static class ChessUtils
    {
        public static char CastlingTypeToChar(CastlingType t)
        {
            switch(t)
            {
                case CastlingType.BLACK_QUEENSIDE:
                    return 'q';
                case CastlingType.BLACK_KINGSIDE:
                    return 'k';
                case CastlingType.WHITE_QUEENSIDE:
                    return 'Q';
                default:
                    return 'K';
            }
        }

        public static string ColorToString(ChessColor color)
        {
            switch (color)
            {
                case ChessColor.BLACK:
                    return "Black";
                case ChessColor.WHITE:
                    return "White";
                default:
                    return "NONE";
            }
        }

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

