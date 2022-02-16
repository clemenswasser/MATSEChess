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

        [Fact]
        public void KingTest()
        {
            Assert.Equal(new[] { new ChessBoardPosition(4, 4), new ChessBoardPosition(5, 4), new ChessBoardPosition(6, 4),
                                 new ChessBoardPosition(4, 5), /* ----------------------- */ new ChessBoardPosition(6, 5),
                                 new ChessBoardPosition(4, 6), new ChessBoardPosition(5, 6), new ChessBoardPosition(6, 6)},
                         new King(ChessColor.WHITE, new ChessBoardPosition(5, 5)).GetPossibleMoves(chessBoard));
        }
    }
}