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

        public List<ChessPiece> Pieces { get { return pieces; } }

        public ChessColor GetPositionState(ChessBoardPosition pos)
        {
            foreach (var piece in pieces)
            {
                if (piece.Position.Equals(pos))
                    return piece.Color;
            }
            return ChessColor.NONE;
        }

        public void AddPiece(ChessPiece piece)
        {
            if(piece == null || GetPositionState(piece.Position) != ChessColor.NONE)
            {
                throw new ArgumentException("Invalid chess piece provided");
            }

            pieces.Add(piece);
        }

        public void Reset()
        {
            pieces.Clear();

            // Pawns
            for(int i = 0; i < 8; ++i)
            {
                pieces.Add(new Pawn(ChessColor.BLACK, new ChessBoardPosition(i, 1)));
                pieces.Add(new Pawn(ChessColor.WHITE, new ChessBoardPosition(i, 6)));
            }

            var rows = new int[]{ 0, 7 };

            foreach(var row in rows)
            {
                ChessColor color = row == 0 ? ChessColor.BLACK : ChessColor.WHITE;

                pieces.Add(new Rook(color, new ChessBoardPosition(0, row)));
                pieces.Add(new Knight(color, new ChessBoardPosition(1, row)));
                pieces.Add(new Bishop(color, new ChessBoardPosition(2, row)));
                pieces.Add(new Queen(color, new ChessBoardPosition(3, row)));
                pieces.Add(new King(color, new ChessBoardPosition(4, row)));
                pieces.Add(new Bishop(color, new ChessBoardPosition(5, row)));
                pieces.Add(new Knight(color, new ChessBoardPosition(6, row)));
                pieces.Add(new Rook(color, new ChessBoardPosition(7, row)));
            }
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

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}

