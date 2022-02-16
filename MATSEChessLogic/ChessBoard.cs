/// <summary>
/// The board is 8x8, where the position (0,0) is the upper left corner of the field.
/// 
/// The white pieces are initially place in the bottom rows (index 6+7), while the black
/// pieces occupy the first two (index 0+1).
/// </summary>
namespace MATSEChess
{
    public class ChessBoard
    {
        private List<ChessPiece> pieces = new List<ChessPiece>();

        public ChessColor GetPositionState(ChessBoardPosition pos)
        {
            foreach (var piece in pieces)
            {
                if(piece.Position.Equals(pos))
                    return piece.Color;
            }
            return ChessColor.NONE;
        }
    }

    public class ChessBoardPosition
    {
        private int x;
        private int y;

        public int X { get => x; }
        public int Y { get => y; }

        public bool Valid { get => x >= 0 && y >= 0 && x < 8 && y < 8; }

        public ChessBoardPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public ChessBoardPosition Move(int x, int y)
        {
            return new ChessBoardPosition(this.x + x, this.y + y);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            ChessBoardPosition? pos = obj as ChessBoardPosition;
            if (pos == null)
                return false;

            return pos.x == x && pos.y == y;
        }
    }
}
