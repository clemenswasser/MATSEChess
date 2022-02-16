

namespace MATSEChess
{
    public enum ChessColor
    {
        BLACK,
        WHITE,
        NONE
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
