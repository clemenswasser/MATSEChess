namespace MATSEChess
{
    public abstract class ChessPiece
    {
        protected ChessColor color;
        protected ChessBoardPosition pos;
        protected ChessPieceType type;

        public ChessColor Color { get => color; }
        public ChessBoardPosition Position { get => pos; }

        public ChessPieceType Type { get => type; }
        public abstract IEnumerable<ChessBoardPosition> GetPossibleMoves(ChessBoard state);

        public ChessPiece(ChessColor color, ChessBoardPosition pos, ChessPieceType type)
        {
            if (color == ChessColor.NONE)
            {
                throw new ArgumentException("The color of a chess piece must not be none");
            }
            this.color = color;
            this.pos = pos;
            this.type = type;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            ChessPiece? piece = obj as ChessPiece;
            if (piece == null)
                return false;

            return piece.color == color && piece.pos.Equals(pos);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(color, pos);
        }
    }

    public class Pawn : ChessPiece
    {
        public Pawn(ChessColor color, ChessBoardPosition pos) : base(color, pos, ChessPieceType.PAWN)
        {

        }

        public override IEnumerable<ChessBoardPosition> GetPossibleMoves(ChessBoard state)
        {
            int moveDir = color == ChessColor.BLACK ? 1 : -1; // Black pawns can only move down

            var oneForward = pos.Move(0, moveDir);
            var oneForwardOccupation = state.GetPositionState(oneForward);

            // 1. Check one step forward
            if (oneForward.Valid && oneForwardOccupation != color)
            {
                yield return pos.Move(0, moveDir);

                if (oneForwardOccupation == ChessColor.NONE)
                {
                    // 2. Check two steps forward (only in initial row)
                    int initialY = color == ChessColor.BLACK ? 1 : 6;
                    var twoForward = pos.Move(0, 2 * moveDir);
                    if (pos.Y == initialY && state.GetPositionState(twoForward) != color)
                    {
                        yield return pos.Move(0, 2 * moveDir);
                    }
                }

            }

            // 3. Check (diagonal) attack positions
            ChessBoardPosition leftPos = pos.Move(-1, moveDir);
            ChessBoardPosition rightPos = pos.Move(1, moveDir);
            if (leftPos.Valid && state.GetPositionState(leftPos) == ChessUtils.GetOpponentColor(color))
                yield return leftPos;
            if (rightPos.Valid && state.GetPositionState(rightPos) == ChessUtils.GetOpponentColor(color))
                yield return rightPos;
        }
    }

    public class King : ChessPiece
    {
        public King(ChessColor color, ChessBoardPosition pos) : base(color, pos, ChessPieceType.KING)
        {

        }

        public override IEnumerable<ChessBoardPosition> GetPossibleMoves(ChessBoard state)
        {
            foreach (var dY in new[] { -1, 0, 1 })
            {
                foreach (var dX in new[] { -1, 0, 1 })
                {
                    if (dX == 0 && dY == 0) continue;
                    var possibleMove = pos.Move(dX, dY);
                    if (possibleMove.Valid && state.GetPositionState(possibleMove) != color) yield return possibleMove;
                }
            }
        }
    }

    public class Rook : ChessPiece
    {
        public Rook(ChessColor color, ChessBoardPosition pos) : base(color, pos, ChessPieceType.ROOK)
        {

        }

        public override IEnumerable<ChessBoardPosition> GetPossibleMoves(ChessBoard state)
        {
            for (int i = 1; i < 9; ++i)
            {
                foreach (var dY in new[] { -i, 0, i })
                {
                    foreach (var dX in new[] { -i, 0, i })
                    {
                        if (Math.Abs(dX) == Math.Abs(dY)) continue;
                        var possibleMove = pos.Move(dX, dY);
                        if (possibleMove.Valid) yield return possibleMove;
                    }
                }
            }
        }
    }

    public class Bishop : ChessPiece
    {
        public Bishop(ChessColor color, ChessBoardPosition pos) : base(color, pos, ChessPieceType.BISHOP)
        {

        }

        public override IEnumerable<ChessBoardPosition> GetPossibleMoves(ChessBoard state)
        {
            for (int i = 1; i < 9; ++i)
            {
                foreach (var dY in new[] { -i, i })
                {
                    foreach (var dX in new[] { -i, i })
                    {
                        var possibleMove = pos.Move(dX, dY);
                        if (possibleMove.Valid) yield return possibleMove;
                    }
                }
            }
        }
    }

    public class Queen : ChessPiece
    {
        public Queen(ChessColor color, ChessBoardPosition pos) : base(color, pos, ChessPieceType.QUEEN)
        {

        }

        public override IEnumerable<ChessBoardPosition> GetPossibleMoves(ChessBoard state)
        {
            for (int i = 1; i < 9; ++i)
            {
                foreach (var dY in new[] { -i, 0, i })
                {
                    foreach (var dX in new[] { -i, 0, i })
                    {
                        if (dX == 0 && dY == 0) continue;
                        var possibleMove = pos.Move(dX, dY);
                        if (possibleMove.Valid) yield return possibleMove;
                    }
                }
            }
        }
    }

    public class Knight : ChessPiece
    {
        public Knight(ChessColor color, ChessBoardPosition pos) : base(color, pos, ChessPieceType.KNIGHT)
        {

        }

        public override IEnumerable<ChessBoardPosition> GetPossibleMoves(ChessBoard state)
        {
            foreach (var dY in new[] { -1, 1, -2, 2 })
            {
                foreach (var dX in new[] { -1, 1, -2, 2 })
                {
                    if (Math.Abs(dX) == Math.Abs(dY)) continue;
                    var possibleMove = pos.Move(dX, dY);
                    if (possibleMove.Valid && state.GetPositionState(possibleMove) != color) yield return possibleMove;
                }
            }
        }
    }
}

