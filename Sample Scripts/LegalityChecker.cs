using System.Collections.Generic;
using UnityEngine;

namespace Detective.Puzzles.PushFight
{

    public class LegalityChecker : MonoBehaviour
    {
        [SerializeField]
        private PushFightMode pushFightMode;

        private int[,] distanceFromSource = new int[Constants.NUM_ROWS, Constants.NUM_COLS];
        private Vector2Int[,] predBFS = new Vector2Int[Constants.NUM_ROWS, Constants.NUM_COLS];

        const int NEVER_VISTED = 0;

        private void Start()
        {

            Constants.NOT_VALID_INDICES.Add(new Vector2Int(0, 0));
            Constants.NOT_VALID_INDICES.Add(new Vector2Int(0, 3));
            Constants.NOT_VALID_INDICES.Add(new Vector2Int(1, 3));
            Constants.NOT_VALID_INDICES.Add(new Vector2Int(6, 0));
            Constants.NOT_VALID_INDICES.Add(new Vector2Int(7, 0));
            Constants.NOT_VALID_INDICES.Add(new Vector2Int(7, 3));
        }

        private void ResetShortestPathVariables()
        {
            for (int i = 0; i < Constants.NUM_ROWS; i++)
            {
                for (int j = 0; j < Constants.NUM_COLS; j++)
                {
                    distanceFromSource[i, j] = 999;
                    predBFS[i, j].x = -1;
                    predBFS[i, j].y = -1;
                }
            }
        }

        public bool DoesPushWin(List<PieceManager> pieceLine, Vector2Int direction)
        {
            PieceManager lastPiece = pieceLine[pieceLine.Count - 1];
            return !IsAdjacentSquareLegal(pushFightMode.occupiedSquares, lastPiece.currentPosition, lastPiece.currentPosition + direction);
        }

        public List<PieceManager> GetLineOfPieces(PieceManager start, Vector2Int direction)
        {
            List<PieceManager> pieceLine = new List<PieceManager>();
            Vector2Int currentIndex = start.currentPosition;
            PieceManager current = start;
            while (IsLegalPosition(currentIndex))
            {
                pieceLine.Add(current);
                currentIndex += direction;
                current = pushFightMode.GetPieces().Find((p) => p.currentPosition == currentIndex);
                if (null == current) { break; }
            }

            return pieceLine;
        }

        public bool[,] ComputeLegalSquares(PieceManager piece)
        {
            C_PieceType[,] board = pushFightMode.occupiedSquares;
            bool[,] legalSquares = new bool[Constants.NUM_ROWS, Constants.NUM_COLS];
            Vector2Int index = piece.currentPosition;

            if (pushFightMode.currentState == GameState.WHITE_SETUP)
            {
                for (int i = Constants.NUM_ROWS / 2; i < Constants.NUM_ROWS; i++)
                {
                    for (int j = 0; j < Constants.NUM_COLS; j++)
                    {
                        legalSquares[i, j] = board[i, j] == C_PieceType.NONE;
                    }
                }
                return legalSquares;

            }
            else if (pushFightMode.currentState == GameState.BLACK_SETUP)
            {
                for (int i = 0; i < Constants.NUM_ROWS / 2; i++)
                {
                    for (int j = 0; j < Constants.NUM_COLS; j++)
                    {
                        legalSquares[i, j] = board[i, j] == C_PieceType.NONE;
                    }
                }
                return legalSquares;

            }
            else
            {
                //BFS
                Queue<Vector2Int> toAdd = new Queue<Vector2Int>();
                Vector2Int zero = new Vector2Int(0, 0);
                toAdd.Enqueue(index);
                ResetShortestPathVariables();
                distanceFromSource[index.x, index.y] = 0;

                void AddToQueue(Queue<Vector2Int> toAdd, Vector2Int source, Vector2Int target)
                {
                    toAdd.Enqueue(target);
                    legalSquares[target.x, target.y] = true;
                    distanceFromSource[target.x, target.y] = distanceFromSource[source.x, source.y] + 1;
                    predBFS[target.x, target.y] = source;
                }

                while (toAdd.Count > 0)
                {
                    Vector2Int popped = toAdd.Dequeue();
                    Vector2Int s1 = new Vector2Int(popped.x - 1, popped.y);
                    Vector2Int s2 = new Vector2Int(popped.x + 1, popped.y);
                    Vector2Int s3 = new Vector2Int(popped.x, popped.y - 1);
                    Vector2Int s4 = new Vector2Int(popped.x, popped.y + 1);

                    //Legal square and not already added
                    if (IsAdjacentSquareLegal(pushFightMode.occupiedSquares, popped, s1) && !legalSquares[s1.x, s1.y]) { AddToQueue(toAdd, popped, s1); }
                    if (IsAdjacentSquareLegal(pushFightMode.occupiedSquares, popped, s2) && !legalSquares[s2.x, s2.y]) { AddToQueue(toAdd, popped, s2); }
                    if (IsAdjacentSquareLegal(pushFightMode.occupiedSquares, popped, s3) && !legalSquares[s3.x, s3.y]) { AddToQueue(toAdd, popped, s3); }
                    if (IsAdjacentSquareLegal(pushFightMode.occupiedSquares, popped, s4) && !legalSquares[s4.x, s4.y]) { AddToQueue(toAdd, popped, s4); }
                }
            }

            return legalSquares;
        }

        public List<PieceManager> ComputeLegalPushes(PusherManager pusher)
        {
            Vector2Int index = pusher.currentPosition;

            PieceManager up = pushFightMode.GetPieces().Find((p) => p.currentPosition == index + Constants.directions[0]);
            PieceManager down = pushFightMode.GetPieces().Find((p) => p.currentPosition == index + Constants.directions[1]);
            PieceManager left = pushFightMode.GetPieces().Find((p) => p.currentPosition == index + Constants.directions[2]);
            PieceManager right = pushFightMode.GetPieces().Find((p) => p.currentPosition == index + Constants.directions[3]);

            List<PieceManager> legalPushes = new List<PieceManager>();
            PieceManager[] toCheck = new PieceManager[4] { up, down, left, right };
            List<PieceManager> pieceLine = new List<PieceManager>();
            bool legal = true;

            //For a push to be legal, 
            //  1. The line being pushed must be able to move one space, and
            //  2. None of the pieces in the line being pushed should be blocked 
            for (int i = 0; i < toCheck.Length; i++)
            {
                if (null == toCheck[i]) { continue; }
                pieceLine = GetLineOfPieces(toCheck[i], Constants.directions[i]);
                legal = true;

                //Check if the line can move
                //A line being pushed up or down is always possible (without blocking)
                if (i == 2 || i == 3)
                {
                    PieceManager lastElement = pieceLine[pieceLine.Count - 1];
                    if (lastElement.currentPosition.y == 0 && i == 2 || lastElement.currentPosition.y == 3 && i == 3)
                    {
                        continue;
                    }
                }

                //Check if any piece is blocked
                foreach (PieceManager p in pieceLine)
                {
                    if (p is PusherManager)
                    {
                        if ((p as PusherManager).isBlocked)
                        {
                            legal = false;
                            break;
                        }
                    }
                }

                if (legal) { legalPushes.Add(toCheck[i]); }
            }

            return legalPushes;
        }

        //t is known to be reachable from s; s is the currently selected piece
        public List<Vector2Int> ComputeShortestPath(PieceManager s, Vector2Int t)
        {
            List<Vector2Int> shortestPath = new List<Vector2Int>();
            Vector2Int current = t;
            Vector2Int next;

            while (current != s.currentPosition)
            {
                next = predBFS[current.x, current.y]; ;
                shortestPath.Add(current - next);
                current = next;
            }

            shortestPath.Reverse();

            //Hideous, but I think it works
            List<Vector2Int> compressedShortestPath = new List<Vector2Int>();
            int index = 0;
            current = shortestPath[index];
            while (index < shortestPath.Count - 1)
            {
                if (shortestPath[index] == shortestPath[index + 1])
                {
                    current += shortestPath[index + 1];
                }
                else
                {
                    compressedShortestPath.Add(current);
                    current = shortestPath[index + 1];
                }

                index++;
            }

            if (index > 0 && current == shortestPath[index - 1])
            {
                compressedShortestPath.Add(shortestPath[index - 1]);
            }
            else
            {
                compressedShortestPath.Add(current);
            }

            return compressedShortestPath;
        }
    }
}