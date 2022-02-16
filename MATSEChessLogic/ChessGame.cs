using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATSEChess
{
    public class ChessGame
    {
        private ChessBoard board = new ChessBoard();
        private ChessBoardPosition? selection;
        private ChessColor currentPlayer;
        public ChessBoard Board { get { return board; } }

        public ChessColor Winner
        {
            get
            {
                var kings = board.Pieces.FindAll(piece => piece.Type == ChessPieceType.KING);

                if (kings.Count != 1)
                    return ChessColor.NONE;
                return kings[0].Color;
            }
        }

        public ChessColor CurrentPlayer { get { return currentPlayer; } }

        public ChessBoardPosition? Selection
        {
            get => selection;
        }

        public bool SetSelection(ChessBoardPosition value)
        {
            if (value == null || Winner != ChessColor.NONE)
            {
                selection = null;
                return true; // Ignore this selection
            }

            if (!value.Valid)
            {
                return false;
            }

            ChessPiece? clickedPos = board.GetPositionPiece(value);

            if (selection == null) // Enter selection mode
            {
                if (clickedPos == null) // Clicked empty tile
                    return true;

                if (clickedPos.Color == currentPlayer)
                {
                    selection = value;
                    return true;
                } 
                else if(clickedPos != null && clickedPos.Color == ChessUtils.GetOpponentColor(CurrentPlayer))
                { // Clicked different team
                    return false;
                }
            }
            else // Already selected something
            {
                if (clickedPos != null && clickedPos.Color == currentPlayer) // Change selection
                {
                    selection = value;
                }
                else
                { // Check if possible move
                    ChessPiece? currentlySelected = board.GetPositionPiece(selection);
                    if (currentlySelected == null)
                    {
                        return false;
                    }

                    if (currentlySelected.GetPossibleMoves(board).Contains(value))
                    {
                        MoveSelected(value);
                    }
                    else
                    {
                        selection = null;
                    }
                }
            }

            return true;
        }

        private void MoveSelected(ChessBoardPosition value)
        {
            if(selection == null)
            {
                return;
            }
            if(board.Move(selection, value))
            {
                selection = null;
                currentPlayer = ChessUtils.GetOpponentColor(currentPlayer);
            }
        }

        public ChessGame()
        {
            Reset();
        }

        public void Reset()
        {
            board.Reset();
            selection = null;
            currentPlayer = ChessColor.WHITE;
        }

    }
}
