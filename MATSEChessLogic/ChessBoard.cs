using System.Text;
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
        private int fullmoveCounter = 0;
        private int halfmoveClock = 0;
        private ChessColor currentPlayer;
        private ChessBoardPosition? enPassantSquare;
        List<CastlingType> allowedCastlings = new List<CastlingType>();

        public int FullmoveCounter { get { return fullmoveCounter; } }

        public int HalfmoveClock { get { return halfmoveClock; } }

        public List<ChessPiece> Pieces { get { return pieces; } }

        public ChessColor CurrentPlayer { get { return currentPlayer; } }

        public PromotionFunction? OnPromotion { get; set; }

        public ChessColor GetPositionState(ChessBoardPosition pos)
        {
            return GetPositionPiece(pos)?.Color ?? ChessColor.NONE;
        }

        public ChessPiece? GetPositionPiece(ChessBoardPosition pos)
        {
            foreach (var piece in pieces)
            {
                if (piece.Position.Equals(pos))
                    return piece;
            }
            return null;
        }

        public void AddPiece(ChessPiece piece)
        {
            if (piece == null || GetPositionState(piece.Position) != ChessColor.NONE)
            {
                throw new ArgumentException("Invalid chess piece provided");
            }

            pieces.Add(piece);
        }

        public bool Move(ChessBoardPosition from, ChessBoardPosition to)
        {
            ChessPiece? moving = GetPositionPiece(from);

            if (moving == null) return false;

            ChessPiece? target = GetPositionPiece(to);
            if (target != null && target.Color == moving.Color)
            { // Trying to beat own color
                return false;
            }

            bool resetHalfmoves = moving.Type == ChessPieceType.PAWN;

            if (target != null)
            { // Caputure another piece
                if (!pieces.Remove(target))
                    return false; // Could not remove target element

                resetHalfmoves = true; // Capture has been made
            }


            // Halfmove clock
            if (resetHalfmoves)
                halfmoveClock = 0;
            else
                ++halfmoveClock;

            // Fullmove counter
            if (currentPlayer == ChessColor.BLACK)
                ++fullmoveCounter;

            CheckForCastlingChange(moving);

            // En Passant
            if (moving.Type == ChessPieceType.PAWN && (to.Y == from.Y + 2 || to.Y == from.Y - 2))
            {
                ChessBoardPosition leftPos = to.Move(-1, 0);
                ChessBoardPosition rightPos = to.Move(1, 0);
                ChessPiece? leftPiece = GetPositionPiece(leftPos);
                ChessPiece? rightPiece = GetPositionPiece(rightPos);
                if (leftPiece != null && leftPiece.Color != moving.Color)
                {
                    leftPiece.EnPassant = true;
                    enPassantSquare = leftPos;
                }
                if (rightPiece != null && rightPiece.Color != moving.Color)
                {
                    rightPiece.EnPassant = true;
                    enPassantSquare = rightPos;
                }
            }
            else
                enPassantSquare = null;

            if (moving.EnPassant)
            {
                ChessPiece? enPassantTarget = GetPositionPiece(to.Move(0, moving.Color == ChessColor.BLACK ? -1 : 1));
                if (enPassantTarget != null && enPassantTarget.Color != moving.Color)
                    pieces.Remove(enPassantTarget);
            }


            moving.Position = to;

            // Castling
            if (moving.Type == ChessPieceType.KING && Math.Abs(from.X - to.X) > 1)
            {
                PerformCastling(moving);
            }

            // Promotions
            CheckForPromotion(moving);


            currentPlayer = ChessUtils.GetOpponentColor(currentPlayer);
            return true;
        }

        private void PerformCastling(ChessPiece movedKing)
        {
            var movedToLeft = movedKing.Position.X < 4;

            var rook = GetPositionPiece(new ChessBoardPosition(movedToLeft ? 0 : 7, movedKing.Position.Y));
            if (rook == null)
            {
                throw new InvalidDataException("Rook was not found at desired position");
            }

            rook.Position = new ChessBoardPosition(movedKing.Position.X + (movedToLeft ? 1 : -1), movedKing.Position.Y);
        }

        private void CheckForCastlingChange(ChessPiece moving)
        {
            if (moving.Type == ChessPieceType.KING)
            {
                if (moving.Color == ChessColor.BLACK)
                {
                    allowedCastlings.Remove(CastlingType.BLACK_QUEENSIDE);
                    allowedCastlings.Remove(CastlingType.BLACK_KINGSIDE);
                }
                else
                {
                    allowedCastlings.Remove(CastlingType.WHITE_QUEENSIDE);
                    allowedCastlings.Remove(CastlingType.WHITE_KINGSIDE);
                }
            }

            if (moving.Type == ChessPieceType.ROOK)
            {
                if (moving.Color == ChessColor.BLACK)
                {
                    if (moving.Position.Y != 0)
                        return;

                    allowedCastlings.Remove(moving.Position.X == 0 ? CastlingType.BLACK_QUEENSIDE : CastlingType.BLACK_KINGSIDE);
                }
                else
                {
                    if (moving.Position.Y != 7)
                        return;

                    allowedCastlings.Remove(moving.Position.X == 0 ? CastlingType.WHITE_QUEENSIDE : CastlingType.WHITE_KINGSIDE);
                }
            }
        }

        private void CheckForPromotion(ChessPiece piece)
        {
            if (OnPromotion == null || piece.Type != ChessPieceType.PAWN)
            {
                return;
            }
            if ((piece.Color == ChessColor.WHITE && piece.Position.Y != 0) || (piece.Color == ChessColor.BLACK && piece.Position.Y != 7))
            {
                return;
            }

            var choice = OnPromotion();
            var pos = piece.Position;
            var color = piece.Color;
            pieces.Remove(piece);
            AddPiece(ChessPiece.Create(choice, color, pos));
        }

        public string GetCastlingString()
        {
            var normal = String.Concat(allowedCastlings.Select(x => ChessUtils.CastlingTypeToChar(x)));
            return normal.Length > 0 ? normal : "-";
        }

        public void Reset()
        {
            halfmoveClock = 0;
            fullmoveCounter = 0;
            currentPlayer = ChessColor.WHITE;

            allowedCastlings.Clear();
            allowedCastlings.Add(CastlingType.WHITE_KINGSIDE);
            allowedCastlings.Add(CastlingType.WHITE_QUEENSIDE);
            allowedCastlings.Add(CastlingType.BLACK_KINGSIDE);
            allowedCastlings.Add(CastlingType.BLACK_QUEENSIDE);

            pieces.Clear();

            // Pawns
            for (int i = 0; i < 8; ++i)
            {
                pieces.Add(new Pawn(ChessColor.BLACK, new ChessBoardPosition(i, 1)));
                pieces.Add(new Pawn(ChessColor.WHITE, new ChessBoardPosition(i, 6)));
            }

            var rows = new int[] { 0, 7 };

            foreach (var row in rows)
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

        public override string ToString()
        {
            var result = new StringBuilder();

            for (int j = 0; j < 8; j++)
            {
                int empty = -1;
                for (int i = 0; i < 8; i++)
                {
                    var piece = GetPositionPiece(new ChessBoardPosition(i, j));
                    if (piece == null)
                    {
                        if (empty == -1) empty = i;
                        continue;
                    }

                    if (empty != -1)
                    {
                        result.Append(i - empty);
                        empty = -1;
                    }

                    result.Append(piece);
                }

                if (empty != -1) result.Append(8 - empty);
                if (j < 7) result.Append('/');
            }

            result.Append($" {(CurrentPlayer == ChessColor.WHITE ? 'w' : 'b')} {GetCastlingString()} {(enPassantSquare != null ? enPassantSquare.ToString() : '-')} {HalfmoveClock} {FullmoveCounter}");

            return result.ToString();
        }

        internal bool IsCastlingAllowed(CastlingType type)
        {
            return allowedCastlings.Contains(type);
        }

        public void FromFENString(string s)
        {
            pieces.Clear();
            int x = 0, y = 0;
            int end = 0;
            foreach (char c in s)
            {
                if (c == ' ') break;
                else if (c == '/')
                {
                    ++y;
                    x = 0;
                }
                else if (char.IsDigit(c)) x += c - '0';
                else
                {
                    pieces.Add(ChessPiece.fromFEN(c, x, y));
                    ++x;
                }

                ++end;
            }

            var properties = s.Substring(end).Split().Skip(1).ToArray();
            currentPlayer = properties[0] == "w" ? ChessColor.WHITE : ChessColor.BLACK;
            allowedCastlings = ChessUtils.StringToCastlingTypes(properties[1]);
            enPassantSquare = properties[2] == "-" ? null : ChessBoardPosition.FromAlgebraic(properties[2]);
            halfmoveClock = Convert.ToInt32(properties[3]);
            fullmoveCounter = Convert.ToInt32(properties[4]);
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
            return $"{(char)((int)'a' + x)}{8 - y}";
        }

        public static ChessBoardPosition FromAlgebraic(string input)
        {
            if(input.Length != 2)
            {
                throw new ArgumentException("Invalid input length");
            }

            int x = input[0] - 'a';
            int y = 8 - (int)(input[1] - '0');

            var result = new ChessBoardPosition(x, y);
            if(!result.Valid)
            {
                throw new ArgumentException("Invalid position provided");
            }
            return result;

        }
    }


    public delegate ChessPieceType PromotionFunction();
}

