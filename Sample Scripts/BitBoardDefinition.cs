using System.Collections.Generic;
using UnityEngine;
using System;

namespace Detective.Puzzles.PushFight
{
    /// <summary>
    /// Treated as 8x4 board, top-left is the leftmost bit, bottom-right
    /// is the rightmost bit, left to right, top to bottom. 
    /// </summary>
    public class PFBitBoard
    {
        //White's Pieces
        public uint wAllPawns;
        public uint wAllPushers;

        //Blue's Pieces
        public uint bAllPawns;
        public uint bAllPushers;

        //Utility
        public uint allPieces;
        public uint blocker; //Blocker

        /// <summary>
        /// Initializes the board
        /// </summary>
        /// <param name="board"></param>
        public PFBitBoard(C_PieceType[,] board)
        {
            wAllPawns = 0;
            wAllPushers = 0;

            bAllPawns = 0;
            bAllPushers = 0;

            blocker = 0;
            allPieces = 0;
            uint indexBB = 0;

            for (int i = 0; i < Constants.NUM_ROWS; i++)
            {
                for (int j = 0; j < Constants.NUM_COLS; j++)
                {
                    indexBB = BitBoardConstants.indexBitBoards[i, j];
                    switch (board[i, j])
                    {
                        case C_PieceType.NONE:
                        case C_PieceType.HOLE:
                            break;

                        case C_PieceType.WHITE_PAWN:
                            wAllPawns |= indexBB;
                            break;

                        case C_PieceType.BLACK_PAWN:
                            bAllPawns |= indexBB;
                            break;

                        case C_PieceType.WHITE_PUSHER:
                            wAllPushers |= indexBB;
                            break;

                        case C_PieceType.BLACK_PUSHER:
                            bAllPushers |= indexBB;
                            break;

                        case C_PieceType.WHITE_BLOCKED_PUSHER:
                            wAllPushers |= indexBB;
                            blocker |= indexBB;
                            break;

                        case C_PieceType.BLACK_BLOCKED_PUSHER:
                            bAllPushers |= indexBB;
                            blocker |= indexBB;
                            break;
                    }
                }
            }

            allPieces = wAllPawns | wAllPushers | bAllPawns | bAllPushers;
        }

        public PFBitBoard(PFBitBoard board)
        {
            allPieces = board.allPieces;
            blocker = board.blocker;
            wAllPawns = board.wAllPawns;
            wAllPushers = board.wAllPushers;
            bAllPawns = board.bAllPawns;
            bAllPushers = board.bAllPushers;
        }

        public bool IsEqual(PFBitBoard board2)
        {
            if (allPieces != board2.allPieces) { return false; }
            if (blocker != board2.blocker) { return false; }
            if (wAllPawns != board2.wAllPawns) { return false; }
            if (wAllPushers != board2.wAllPushers) { return false; }
            if (bAllPawns != board2.bAllPawns) { return false; }
            if (bAllPushers != board2.bAllPushers) { return false; }

            return true;
        }

        /// <summary>
        /// Populates the arrays, listing where each piece is. 
        /// </summary>
        /// <param name="wPawns"></param>
        /// <param name="wPushers"></param>
        /// <param name="bPawns"></param>
        /// <param name="bPushers"></param>
        public void PopulateArrays(uint[] wPawns, uint[] wPushers, uint[] bPawns, uint[] bPushers)
        {
            uint index;

            int wPawnIndex = 0;
            int wPusherIndex = 0;
            int bPawnIndex = 0;
            int bPusherIndex = 0;

            for (int i = 0; i < Constants.NUM_ROWS; i++)
            {
                for (int j = 0; j < Constants.NUM_COLS; j++)
                {
                    index = BitBoardConstants.indexBitBoards[i, j];
                    if ((wAllPawns & index) != 0)
                    {
                        wPawns[wPawnIndex] = index; wPawnIndex++;
                    }
                    else if ((wAllPushers & index) != 0)
                    {
                        wPushers[wPusherIndex] = index; wPusherIndex++;
                    }
                    else if ((bAllPawns & index) != 0)
                    {
                        bPawns[bPawnIndex] = index; bPawnIndex++;
                    }
                    else if ((bAllPushers & index) != 0)
                    {
                        bPushers[bPusherIndex] = index; bPusherIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// Populates the arrays, listing where each piece is. 
        /// </summary>
        /// <param name="wPieces"></param>
        /// <param name="bPieces"></param>
        public void PopulateArrays(uint[] wPieces, uint[] bPieces)
        {
            uint index;

            int wPawnIndex = 0;
            int wPusherIndex = 2;
            int bPawnIndex = 0;
            int bPusherIndex = 2;

            for (int i = 0; i < Constants.NUM_ROWS; i++)
            {
                for (int j = 0; j < Constants.NUM_COLS; j++)
                {
                    index = BitBoardConstants.indexBitBoards[i, j];
                    if ((wAllPawns & index) != 0)
                    {
                        wPieces[wPawnIndex] = index; wPawnIndex++;
                    }
                    else if ((wAllPushers & index) != 0)
                    {
                        wPieces[wPusherIndex] = index; wPusherIndex++;
                    }
                    else if ((bAllPawns & index) != 0)
                    {
                        bPieces[bPawnIndex] = index; bPawnIndex++;
                    }
                    else if ((bAllPushers & index) != 0)
                    {
                        bPieces[bPusherIndex] = index; bPusherIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// Shifts the index in the given direction. 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static uint ShiftInDirection(uint index, PushDirection direction)
        {
            switch (direction)
            {
                case PushDirection.Up:
                    return index << 4;

                case PushDirection.Down:
                    return index >> 4;

                case PushDirection.Left:
                    return index << 1;

                case PushDirection.Right:
                    return index >> 1;
            }

            return index;
        }

        /// <summary>
        /// Reverse the Direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static PushDirection ReverseDirection(PushDirection direction)
        {
            switch (direction)
            {
                case PushDirection.Up:
                    return PushDirection.Down;

                case PushDirection.Down:
                    return PushDirection.Up;

                case PushDirection.Left:
                    return PushDirection.Right;

                case PushDirection.Right:
                    return PushDirection.Left;
            }

            return direction;
        }

        /// <summary>
        /// Makes a move, updates the bitboards.
        /// </summary>
        /// <param name="move"></param>
        public void MakeMove(MoveBB move)
        {
            if ((wAllPawns & move.source) != 0)
            {
                wAllPawns ^= move.fullbb;
            }
            else if ((wAllPushers & move.source) != 0)
            {
                wAllPushers ^= move.fullbb;
            }
            else if ((bAllPawns & move.source) != 0)
            {
                bAllPawns ^= move.fullbb;
            }
            else if ((bAllPushers & move.source) != 0)
            {
                bAllPushers ^= move.fullbb;
            }

            allPieces ^= move.fullbb;
        }

        /// <summary>
        /// Undos the move. 
        /// </summary>
        /// <param name="move"></param>
        public void UndoMove(MoveBB move)
        {
            if ((wAllPawns & move.target) != 0)
            {
                wAllPawns ^= move.fullbb;
            }
            else if ((wAllPushers & move.target) != 0)
            {
                wAllPushers ^= move.fullbb;
            }
            else if ((bAllPawns & move.target) != 0)
            {
                bAllPawns ^= move.fullbb;
            }
            else if ((bAllPushers & move.target) != 0)
            {
                bAllPushers ^= move.fullbb;
            }

            allPieces ^= move.fullbb;
        }

        /// <summary>
        /// Does the push. 
        /// </summary>
        /// <param name="push"></param>
        public void MakePush(PushBB push)
        {
            for (int i = push.pushAsMoves.Count - 1; i >= 0; i--)
            {
                MakeMove(push.pushAsMoves[i]);
            }

            blocker = ShiftInDirection(push.source, push.direction);
        }

        /// <summary>
        /// Undos the push. 
        /// </summary>
        public void UndoPush(PushBB push)
        {
            for (int i = 0; i < push.pushAsMoves.Count; i++)
            {
                UndoMove(push.pushAsMoves[i]);
            }

            blocker = push.blocker;
        }

        /// <summary>
        /// Prints the board. For debugging. 
        /// </summary>
        public void PrintBoard()
        {
            string allLines = "";
            string line = "";
            for (int i = 0; i < Constants.NUM_ROWS; i++)
            {
                line = "";
                for (int j = 0; j < Constants.NUM_COLS; j++)
                {
                    uint o1 = allPieces & BitBoardConstants.indexBitBoards[i, j];
                    line += o1 == 0 ? "0" : "1";
                }
                allLines += line + "\n";
            }
            Debug.Log(allLines);
        }

        public static void PrintBitBoard(uint bb)
        {
            string allLines = "";
            string line = "";
            for (int i = 0; i < Constants.NUM_ROWS; i++)
            {
                line = "";
                for (int j = 0; j < Constants.NUM_COLS; j++)
                {
                    uint o1 = bb & BitBoardConstants.indexBitBoards[i, j];
                    line += o1 == 0 ? "0" : "1";
                }
                allLines += line + "\n";
            }
            Debug.Log(allLines);

        }
    }

    public class BoardAndPiece
    {
        public PFBitBoard board;
        public uint piece;

        public BoardAndPiece(PFBitBoard _board, uint _piece)
        {
            board = new PFBitBoard(_board);
            piece = _piece;
        }
    }

    /// <summary>
    /// Defines moving a piece using bitboards. Always legal.
    /// </summary>
    [Serializable]
    public class MoveBB
    {
        public uint source;
        public uint target;
        public uint fullbb;

        public MoveBB(uint _source, uint _target)
        {
            source = _source;
            target = _target;
            fullbb = source | target;
        }
    }

    /// <summary>
    /// Defines pushing a line using bitboards. Always legal. 
    /// </summary>
    [Serializable]
    public class PushBB
    {
        public uint source;
        public List<MoveBB> pushAsMoves;
        public PushDirection direction;
        public uint blocker;

        public PushBB(uint _source, List<MoveBB> _pushAsMoves, PushDirection _direction, uint _blocker)
        {
            source = _source;
            pushAsMoves = _pushAsMoves;
            direction = _direction;
            blocker = _blocker;
        }
    }

    /// <summary>
    /// Redefines what a turn is using Bit Boards
    /// </summary>
    [Serializable]
    public class TurnBB
    {
        public MoveBB move1, move2;

        public PushBB push;

        [NonSerialized]
        public int numMoved = 0;

        public TurnBB(MoveBB _move1, MoveBB _move2, PushBB _push)
        {
            move1 = _move1;
            move2 = _move2;
            push = _push;
            numMoved = 2;
        }

        public TurnBB(MoveBB _move, PushBB _push)
        {
            move1 = _move;
            move2 = null;
            push = _push;
            numMoved = 1;
        }

        public TurnBB(PushBB _push)
        {
            move1 = null;
            move2 = null;
            push = _push;
            numMoved = 0;
        }

        public bool IsEmptyMove(MoveBB move)
        {
            return null == move || move.source == 0 || move.target == 0;
        }

        public void MakeTurn(PFBitBoard board)
        {
            if (!IsEmptyMove(move1)) { board.MakeMove(move1); }
            if (!IsEmptyMove(move2)) { board.MakeMove(move2); }
            board.MakePush(push);
        }

        public void UndoTurn(PFBitBoard board)
        {
            board.UndoPush(push);
            if (!IsEmptyMove(move2)) { board.UndoMove(move2); }
            if (!IsEmptyMove(move1)) { board.UndoMove(move1); }
        }

        public void MakeMoves(PFBitBoard board)
        {
            if (!IsEmptyMove(move1)) { board.MakeMove(move1); }
            if (!IsEmptyMove(move2)) { board.MakeMove(move2); }
        }

        public void UndoMoves(PFBitBoard board)
        {
            if (!IsEmptyMove(move2)) { board.UndoMove(move2); }
            if (!IsEmptyMove(move1)) { board.UndoMove(move1); }
        }
    }
}