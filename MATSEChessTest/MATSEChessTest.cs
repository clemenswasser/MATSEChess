using Xunit;
using MATSEChess;

namespace MATSEChessTest
{
    public class MATSEChessTest
    {
        public static ChessBoard chessBoard = new ChessBoard();

        [Fact]
        public void PawnTest()
        {
            Assert.Equal(new[] { new ChessBoardPosition(5, 4) },
                         new Pawn(ChessColor.WHITE, new ChessBoardPosition(5, 5)).GetPossibleMoves(chessBoard));
        }
    }
}