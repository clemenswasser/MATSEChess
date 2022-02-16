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

            var oneForward = pos.Move(0, moveDir);
            var oneForwardOccupation = state.GetPositionState(oneForward);

            // 1. Check one step forward
            if (oneForward.Valid && oneForwardOccupation != color)
            {
                yield return pos.Move(0, moveDir);

                if(oneForwardOccupation == ChessColor.NONE)
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

            

            // 3. Check attack positions
            ChessBoardPosition leftPos = pos.Move(-1, moveDir);
            ChessBoardPosition rightPos = pos.Move(1, moveDir);
            if(leftPos.Valid && state.GetPositionState(leftPos) == ChessUtils.GetOpponentColor(color))
                yield return leftPos;
            if(rightPos.Valid && state.GetPositionState(rightPos) == ChessUtils.GetOpponentColor(color))
                yield return rightPos;
        }
    }
}
