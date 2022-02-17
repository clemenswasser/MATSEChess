namespace MATSEChess
{
    public abstract class ChessPiece
    {
        protected ChessColor color;
        protected ChessBoardPosition pos;
        protected ChessPieceType type;
        protected bool enPassant;

        public ChessColor Color { get => color; }
        public ChessBoardPosition Position
        {
            get => pos; set
            {
                pos = value;
            }
        }
        public bool EnPassant
        {
            get => enPassant;
            set { this.enPassant = value; }
        }

        public ChessPieceType Type { get => type; }
        public abstract IEnumerable<ChessBoardPosition> GetPossibleMoves(ChessBoard state);

        public IEnumerable<ChessBoardPosition> MoveVertical(ChessBoard state)
        {
            //Move up
            for (int i = 1; i < 9; ++i)
            {
                var possibleMove = pos.Move(0, -i);
                if (!possibleMove.Valid || state.GetPositionState(possibleMove) == color)
                    break;
                yield return possibleMove;
                if (state.GetPositionState(possibleMove) == ChessUtils.GetOpponentColor(color))
                    break;
            }
            //Move down
            for (int i = 1; i < 9; ++i)
            {
                var possibleMove = pos.Move(0, i);
                if (!possibleMove.Valid || state.GetPositionState(possibleMove) == color)
                    break;
                yield return possibleMove;
                if (state.GetPositionState(possibleMove) == ChessUtils.GetOpponentColor(color))
                    break;
            }
        }

        public IEnumerable<ChessBoardPosition> MoveHorizontal(ChessBoard state)
        {
            //Move left
            for (int i = 1; i < 9; ++i)
            {
                var possibleMove = pos.Move(-i, 0);
                if (!possibleMove.Valid || state.GetPositionState(possibleMove) == color)
                    break;
                yield return possibleMove;
                if (state.GetPositionState(possibleMove) == ChessUtils.GetOpponentColor(color))
                    break;
            }
            //Move right
            for (int i = 1; i < 9; ++i)
            {
                var possibleMove = pos.Move(i, 0);
                if (!possibleMove.Valid || state.GetPositionState(possibleMove) == color)
                    break;
                yield return possibleMove;
                if (state.GetPositionState(possibleMove) == ChessUtils.GetOpponentColor(color))
                    break;
            }
        }

        public IEnumerable<ChessBoardPosition> MoveVerticalHorizontal(ChessBoard state)
        {
            foreach(var i in MoveVertical(state))
                yield return i;

            foreach (var i in MoveHorizontal(state))
                yield return i;
            
        }
        public IEnumerable<ChessBoardPosition> MoveDiagonal(ChessBoard state)
        {
            //Move upleft
            for (int i = 1; i < 9; ++i)
            {
                var possibleMove = pos.Move(-i, -i);
                if (!possibleMove.Valid || state.GetPositionState(possibleMove) == color)
                    break;
                yield return possibleMove;
                if (state.GetPositionState(possibleMove) == ChessUtils.GetOpponentColor(color))
                    break;
            }
            //Move upright
            for (int i = 1; i < 9; ++i)
            {
                var possibleMove = pos.Move(i, -i);
                if (!possibleMove.Valid || state.GetPositionState(possibleMove) == color)
                    break;
                yield return possibleMove;
                if (state.GetPositionState(possibleMove) == ChessUtils.GetOpponentColor(color))
                    break;
            }
            //Move downleft
            for (int i = 1; i < 9; ++i)
            {
                var possibleMove = pos.Move(-i, i);
                if (!possibleMove.Valid || state.GetPositionState(possibleMove) == color)
                    break;
                yield return possibleMove;
                if (state.GetPositionState(possibleMove) == ChessUtils.GetOpponentColor(color))
                    break;
            }
            //Move downright
            for (int i = 1; i < 9; ++i)
            {
                var possibleMove = pos.Move(i, i);
                if (!possibleMove.Valid || state.GetPositionState(possibleMove) == color)
                    break;
                yield return possibleMove;
                if (state.GetPositionState(possibleMove) == ChessUtils.GetOpponentColor(color))
                    break;
            }
        }

        public static ChessPiece Create(ChessPieceType type, ChessColor color, ChessBoardPosition pos)
        {
            switch (type)
            {
                case ChessPieceType.KING:
                    return new King(color, pos);
                case ChessPieceType.QUEEN:
                    return new Queen(color, pos);
                case ChessPieceType.ROOK:
                    return new Rook(color, pos);
                case ChessPieceType.BISHOP:
                    return new Bishop(color, pos);
                case ChessPieceType.KNIGHT:
                    return new Knight(color, pos);
                case ChessPieceType.PAWN:
                default:
                    return new Pawn(color, pos);
            }
        }

        public ChessPiece(ChessColor color, ChessBoardPosition pos, ChessPieceType type)
        {
            if (color == ChessColor.NONE)
            {
                throw new ArgumentException("The color of a chess piece must not be none");
            }
            this.color = color;
            this.pos = pos;
            this.type = type;
            this.enPassant = false;
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

        private static Dictionary<ChessPieceType, char> typeDict = new Dictionary<ChessPieceType, char>()
        {
            { ChessPieceType.KING,   'k' },
            { ChessPieceType.QUEEN,  'q' },
            { ChessPieceType.ROOK,   'r' },
            { ChessPieceType.KNIGHT, 'n' },
            { ChessPieceType.BISHOP, 'b' },
            { ChessPieceType.PAWN,   'p' },
        };

        public override string ToString()
        {
            var typeIdent = typeDict[Type].ToString();

            if (color == ChessColor.WHITE) typeIdent = typeIdent.ToUpper();

            return typeIdent;
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
            if (oneForward.Valid && oneForwardOccupation == ChessColor.NONE)
            {
                yield return pos.Move(0, moveDir);

                // 2. Check two steps forward (only in initial row)
                int initialY = color == ChessColor.BLACK ? 1 : 6;
                var twoForward = pos.Move(0, 2 * moveDir);
                if (pos.Y == initialY && state.GetPositionState(twoForward) != color)
                {
                    yield return pos.Move(0, 2 * moveDir);
                }
            }

            // 3. Check (diagonal) attack positions
            ChessBoardPosition topLeftPos = pos.Move(-1, moveDir);
            ChessBoardPosition topRightPos = pos.Move(1, moveDir);
            if (topLeftPos.Valid && state.GetPositionState(topLeftPos) == ChessUtils.GetOpponentColor(color))
                yield return topLeftPos;
            if (topRightPos.Valid && state.GetPositionState(topRightPos) == ChessUtils.GetOpponentColor(color))
                yield return topRightPos;

            // 4. Check EnPassant
            ChessBoardPosition leftPos = pos.Move(-1, 0);
            ChessBoardPosition rightPos = pos.Move(1, 0);
            if (enPassant && state.GetPositionState(leftPos) == ChessUtils.GetOpponentColor(color))
                yield return topLeftPos;
            if (enPassant && state.GetPositionState(rightPos) == ChessUtils.GetOpponentColor(color))
                yield return topRightPos;
        }
    }

    public class King : ChessPiece
    {
        public King(ChessColor color, ChessBoardPosition pos) : base(color, pos, ChessPieceType.KING)
        {

        }

        public override IEnumerable<ChessBoardPosition> GetPossibleMoves(ChessBoard state)
        {
            for (var dY = -1; dY <= 1; ++dY)
            {
                for (var dX = -1; dX <= 1; ++dX)
                {
                    if (dX == 0 && dY == 0) continue;
                    var possibleMove = pos.Move(dX, dY);
                    if (possibleMove.Valid && state.GetPositionState(possibleMove) != color) yield return possibleMove;
                }
            }


            var toQueenside = (color == ChessColor.BLACK && state.IsCastlingAllowed(CastlingType.BLACK_QUEENSIDE)) || (color == ChessColor.WHITE && state.IsCastlingAllowed(CastlingType.WHITE_QUEENSIDE));
            var toKingside = (color == ChessColor.BLACK && state.IsCastlingAllowed(CastlingType.BLACK_KINGSIDE)) || (color == ChessColor.WHITE && state.IsCastlingAllowed(CastlingType.WHITE_KINGSIDE));

            if(toQueenside || toKingside)
            {
                foreach(var loc in MoveHorizontal(state))
                {
                    if (Math.Abs(loc.X - pos.X) != 2)
                        continue;

                    if (loc.X < pos.X && toQueenside)
                        yield return loc;
                    else if(loc.X > pos.X && toKingside) 
                        yield return loc;
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
            return MoveVerticalHorizontal(state);
        }
    }

    public class Bishop : ChessPiece
    {
        public Bishop(ChessColor color, ChessBoardPosition pos) : base(color, pos, ChessPieceType.BISHOP)
        {

        }

        public override IEnumerable<ChessBoardPosition> GetPossibleMoves(ChessBoard state)
        {
            return MoveDiagonal(state);
        }
    }

    public class Queen : ChessPiece
    {
        public Queen(ChessColor color, ChessBoardPosition pos) : base(color, pos, ChessPieceType.QUEEN)
        {

        }

        public override IEnumerable<ChessBoardPosition> GetPossibleMoves(ChessBoard state)
        {
            foreach (var possibleMove in MoveVerticalHorizontal(state)) yield return possibleMove;
            foreach (var possibleMove in MoveDiagonal(state)) yield return possibleMove;
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

