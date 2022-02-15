using System.Collections.Generic;

namespace MATSEChess
{
    public abstract class ChessPiece
    {
        protected ChessColor color;
        protected ChessBoardPosition pos;

        public ChessColor Color { get => color; }
        public ChessBoardPosition Position { get => pos; }
        public abstract IEnumerable<ChessBoardPosition> GetPossibleMoves(ChessBoard state);

        public ChessPiece(ChessColor color, ChessBoardPosition pos)
        {
            this.color = color;
            this.pos = pos;
        }
    }

    public class Bauer : ChessPiece
    {
        public Bauer(ChessColor color, ChessBoardPosition pos) : base(color, pos)
        {

        }

        public override IEnumerable<ChessBoardPosition> GetPossibleMoves(ChessBoard state)
        {
            int moveDir = color == ChessColor.BLACK ? 1 : -1; // Black pawns can only move down

            // 1. Check one step forward
            if(!state.IsOccupied(pos.Move(0, moveDir)))
            {
                yield return pos.Move(0, moveDir);
            }

            // 2. Check two steps forward (only in initial row)
            int initialY = color == ChessColor.BLACK ? 1 : 6;
            if(pos.Y == initialY && !state.IsOccupied(pos.Move(0, 2*moveDir)))
            {
                yield return pos.Move(0, 2*moveDir);
            }

            // 3. Check attack positions
            ChessBoardPosition leftPos = pos.Move(-1, moveDir);
            ChessBoardPosition rightPos = pos.Move(1, moveDir);
            if(!state.IsOccupied(leftPos))
                yield return leftPos;
            if(!state.IsOccupied(rightPos))
                yield return rightPos;
        }
    }
}
