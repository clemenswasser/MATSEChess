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

        public ChessBoardPosition? Selection
        {
            get => selection;
            set
            {
                if(value == null)
                {
                    selection = null;
                    return;
                }

                if(!value.Valid)
                {
                    throw new ArgumentException("Invalid position provided");
                }

                ChessPiece? clickedPos = board.GetPositionPiece(value);

                if(selection == null) // Enter selection mode
                {
                    if(clickedPos != null && clickedPos.Color == currentPlayer)
                    {
                        selection = value;
                    }
                } 
                else // Already selected something
                {
                    if(clickedPos != null && clickedPos.Color == currentPlayer) // Change selection
                    {
                        selection = value;
                    } else
                    { // Check if possible move
                        ChessPiece? currentlySelected = board.GetPositionPiece(selection);
                        if(currentlySelected == null)
                        {
                            throw new DataMisalignedException("No piece at current selection");
                        }

                        if(currentlySelected.GetPossibleMoves(board).Contains(value))
                        {
                            MoveSelected(value);
                        } else
                        {
                            selection = null;
                        }
                    }
                }
            }
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
