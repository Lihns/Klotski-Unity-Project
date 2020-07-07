using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    up, down, left, right
}

public enum ChessType
{
    oneByOne, oneByTwo, twoByOne, twoByTwo
}





[Serializable]
public struct ChessDescription
{
    public string name;
    public Vector2Int position;
    public ChessType type;
}



public class GameScript : MonoBehaviour
{
    public bool MoveChess(string chessName, Direction dir)
    {
        var chess = chesses[chessName];
        switch (dir)
        {
            case Direction.up:
                if (chess.MoveUp(chessBoard))
                {
                    return true;
                }
                break;
            case Direction.down:
                if (chess.MoveDown(chessBoard))
                {
                    return true;
                }
                break;
            case Direction.left:
                if (chess.MoveLeft(chessBoard))
                {
                    return true;
                }
                break;
            case Direction.right:
                if (chess.MoveRight(chessBoard))
                {
                    return true;
                }
                break;
            default:
                break;
        }
        return false;
    }

    private void Start()
    {
        foreach (var description in chessDescriptions)
        {
            //initialize chesses
            IChess chess = null;
            switch (description.type)
            {
                case ChessType.oneByOne:
                    chess = new Chess1x1(description.name, description.position);
                    break;
                case ChessType.oneByTwo:
                    chess = new Chess1x2(description.name, description.position);
                    break;
                case ChessType.twoByOne:
                    chess = new Chess2x1(description.name, description.position);
                    break;
                case ChessType.twoByTwo:
                    chess = new Chess2x2(description.name, description.position);
                    break;
                default:
                    break;
            }
            chesses.Add(description.name, chess);

            chess.PutToChessBoard(chessBoard);
        }
    }

    /* variables */
    public ChessDescription[] chessDescriptions = new ChessDescription[10];

    ChessBoard chessBoard = new ChessBoard();
    Dictionary<string, IChess> chesses = new Dictionary<string, IChess>();

    /* internal classes */
    internal interface IChess
    {
        bool MoveUp(ChessBoard chessBoard);
        bool MoveDown(ChessBoard chessBoard);
        bool MoveLeft(ChessBoard chessBoard);
        bool MoveRight(ChessBoard chessBoard);
        bool PutToChessBoard(ChessBoard chessBoard);
    }

    class Chess2x2 : IChess
    {
        Vector2Int upperLeft, upperRight, lowerLeft, lowerRight;
        public Chess2x2(string name, Vector2Int lowerLeft)
        {
            this.lowerLeft = lowerLeft;
            lowerRight = new Vector2Int(lowerLeft.x + 1, lowerLeft.y);

            upperLeft = new Vector2Int(lowerLeft.x, lowerLeft.y + 1);
            upperRight = new Vector2Int(lowerLeft.x + 1, lowerLeft.y + 1);
        }
        public bool MoveDown(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(lowerLeft + Vector2Int.down) && chessBoard.GridEmpty(lowerRight + Vector2Int.down))
            {
                chessBoard.SetGrid(lowerLeft, null);
                chessBoard.SetGrid(lowerRight, null);
                chessBoard.SetGrid(upperLeft, null);
                chessBoard.SetGrid(upperRight, null);
                lowerLeft += Vector2Int.down;
                lowerRight += Vector2Int.down;
                upperLeft += Vector2Int.down;
                upperRight += Vector2Int.down;
                chessBoard.SetGrid(lowerLeft, this);
                chessBoard.SetGrid(lowerRight, this);
                chessBoard.SetGrid(upperLeft, this);
                chessBoard.SetGrid(upperRight, this);
                return true;
            }
            return false;
        }

        public bool MoveLeft(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(upperLeft + Vector2Int.left) && chessBoard.GridEmpty(lowerLeft + Vector2Int.left))
            {
                chessBoard.SetGrid(lowerLeft, null);
                chessBoard.SetGrid(lowerRight, null);
                chessBoard.SetGrid(upperLeft, null);
                chessBoard.SetGrid(upperRight, null);
                lowerLeft += Vector2Int.left;
                lowerRight += Vector2Int.left;
                upperLeft += Vector2Int.left;
                upperRight += Vector2Int.left;
                chessBoard.SetGrid(lowerLeft, this);
                chessBoard.SetGrid(lowerRight, this);
                chessBoard.SetGrid(upperLeft, this);
                chessBoard.SetGrid(upperRight, this);
                return true;
            }
            return false;
        }

        public bool MoveRight(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(upperRight + Vector2Int.right) && chessBoard.GridEmpty(lowerRight + Vector2Int.right))
            {
                chessBoard.SetGrid(lowerLeft, null);
                chessBoard.SetGrid(lowerRight, null);
                chessBoard.SetGrid(upperLeft, null);
                chessBoard.SetGrid(upperRight, null);
                lowerLeft += Vector2Int.right;
                lowerRight += Vector2Int.right;
                upperLeft += Vector2Int.right;
                upperRight += Vector2Int.right;
                chessBoard.SetGrid(lowerLeft, this);
                chessBoard.SetGrid(lowerRight, this);
                chessBoard.SetGrid(upperLeft, this);
                chessBoard.SetGrid(upperRight, this);
                return true;
            }
            return false;
        }

        public bool MoveUp(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(upperLeft + Vector2Int.up) && chessBoard.GridEmpty(upperRight + Vector2Int.up))
            {
                chessBoard.SetGrid(lowerLeft, null);
                chessBoard.SetGrid(lowerRight, null);
                chessBoard.SetGrid(upperLeft, null);
                chessBoard.SetGrid(upperRight, null);
                lowerLeft += Vector2Int.up;
                lowerRight += Vector2Int.up;
                upperLeft += Vector2Int.up;
                upperRight += Vector2Int.up;
                chessBoard.SetGrid(lowerLeft, this);
                chessBoard.SetGrid(lowerRight, this);
                chessBoard.SetGrid(upperLeft, this);
                chessBoard.SetGrid(upperRight, this);
                return true;
            }
            return false;
        }

        public bool PutToChessBoard(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(lowerLeft) && chessBoard.GridEmpty(lowerRight) && chessBoard.GridEmpty(upperLeft) && chessBoard.GridEmpty(upperRight))
            {
                chessBoard.SetGrid(lowerLeft, this);
                chessBoard.SetGrid(lowerRight, this);
                chessBoard.SetGrid(upperLeft, this);
                chessBoard.SetGrid(upperRight, this);
                return true;
            }
            return false;
        }
    }
    class Chess1x2 : IChess
    {
        Vector2Int upper, lower;

        public Chess1x2(string name, Vector2Int lowerLeft)
        {
            lower = lowerLeft;
            upper = new Vector2Int(lower.x, lower.y + 1);
        }

        public bool MoveDown(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(lower + Vector2Int.down))
            {
                chessBoard.SetGrid(upper, null);
                chessBoard.SetGrid(lower, null);
                upper += Vector2Int.down;
                lower += Vector2Int.down;
                chessBoard.SetGrid(upper, this);
                chessBoard.SetGrid(lower, this);
                return true;
            }
            return false;
        }

        public bool MoveLeft(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(upper + Vector2Int.left) && chessBoard.GridEmpty(lower + Vector2Int.left))
            {
                chessBoard.SetGrid(upper, null);
                chessBoard.SetGrid(lower, null);
                upper += Vector2Int.left;
                lower += Vector2Int.left;
                chessBoard.SetGrid(upper, this);
                chessBoard.SetGrid(lower, this);
                return true;
            }
            return false;
        }

        public bool MoveRight(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(upper + Vector2Int.right) && chessBoard.GridEmpty(lower + Vector2Int.right))
            {
                chessBoard.SetGrid(upper, null);
                chessBoard.SetGrid(lower, null);
                upper += Vector2Int.right;
                lower += Vector2Int.right;
                chessBoard.SetGrid(upper, this);
                chessBoard.SetGrid(lower, this);
                return true;
            }
            return false;
        }

        public bool MoveUp(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(upper + Vector2Int.up))
            {
                chessBoard.SetGrid(upper, null);
                chessBoard.SetGrid(lower, null);
                upper += Vector2Int.up;
                lower += Vector2Int.up;
                chessBoard.SetGrid(upper, this);
                chessBoard.SetGrid(lower, this);
                return true;
            }
            return false;
        }

        public bool PutToChessBoard(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(upper) && chessBoard.GridEmpty(lower))
            {
                chessBoard.SetGrid(upper, this);
                chessBoard.SetGrid(lower, this);
                return true;
            }
            return false;
        }
    }
    class Chess2x1 : IChess
    {
        Vector2Int left, right;

        public Chess2x1(string name, Vector2Int lowerLeft)
        {
            left = lowerLeft;
            right = new Vector2Int(left.x + 1, left.y);
        }

        public bool MoveDown(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(left + Vector2Int.down) && chessBoard.GridEmpty(right + Vector2Int.down))
            {
                chessBoard.SetGrid(left, null);
                chessBoard.SetGrid(right, null);
                left += Vector2Int.down;
                right += Vector2Int.down;
                chessBoard.SetGrid(left, this);
                chessBoard.SetGrid(right, this);
                return true;
            }
            return false;
        }

        public bool MoveLeft(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(left + Vector2Int.left))
            {
                chessBoard.SetGrid(left, null);
                chessBoard.SetGrid(right, null);
                left += Vector2Int.left;
                right += Vector2Int.left;
                chessBoard.SetGrid(left, this);
                chessBoard.SetGrid(right, this);
                return true;
            }
            return false;
        }

        public bool MoveRight(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(right + Vector2Int.right))
            {
                chessBoard.SetGrid(left, null);
                chessBoard.SetGrid(right, null);
                left += Vector2Int.right;
                right += Vector2Int.right;
                chessBoard.SetGrid(left, this);
                chessBoard.SetGrid(right, this);
                return true;
            }
            return false;
        }

        public bool MoveUp(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(left + Vector2Int.up) && chessBoard.GridEmpty(right + Vector2Int.up))
            {
                chessBoard.SetGrid(left, null);
                chessBoard.SetGrid(right, null);
                left += Vector2Int.up;
                right += Vector2Int.up;
                chessBoard.SetGrid(left, this);
                chessBoard.SetGrid(right, this);
                return true;
            }
            return false;
        }

        public bool PutToChessBoard(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(left) && chessBoard.GridEmpty(right))
            {
                chessBoard.SetGrid(left, this);
                chessBoard.SetGrid(right, this);
                return true;
            }
            return false;
        }
    }
    class Chess1x1 : IChess
    {
        Vector2Int center;
        public Chess1x1(string name, Vector2Int lowerLeft)
        {
            center = lowerLeft;
        }
        public bool MoveDown(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(center + Vector2Int.down))
            {
                chessBoard.SetGrid(center, null);
                center += Vector2Int.down;
                chessBoard.SetGrid(center, this);
                return true;
            }
            return false;
        }

        public bool MoveLeft(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(center + Vector2Int.left))
            {
                chessBoard.SetGrid(center, null);
                center += Vector2Int.left;
                chessBoard.SetGrid(center, this);
                return true;
            }
            return false;
        }

        public bool MoveRight(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(center + Vector2Int.right))
            {
                chessBoard.SetGrid(center, null);
                center += Vector2Int.right;
                chessBoard.SetGrid(center, this);
                return true;
            }
            return false;
        }

        public bool MoveUp(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(center + Vector2Int.up))
            {
                chessBoard.SetGrid(center, null);
                center += Vector2Int.up;
                chessBoard.SetGrid(center, this);
                return true;
            }
            return false;
        }

        public bool PutToChessBoard(ChessBoard chessBoard)
        {
            if (chessBoard.GridEmpty(center))
            {
                chessBoard.SetGrid(center, this);
                return true;
            }
            return false;
        }
    }
    public class ChessBoard
    {
        public static int unitLength = 1;
        public static Vector2Int dimension = new Vector2Int(4, 5);
        internal ChessBoard()
        {
            chesses = new IChess[dimension.x, dimension.y];
        }

        internal bool GridEmpty(Vector2Int grid)
        {
            //if out of bounds
            if (grid.x >= dimension.x || grid.y >= dimension.y || grid.x < 0 || grid.y < 0)
            {
                return false;
            }
            //grid is not empty
            if (chesses[grid.x, grid.y] != null)
            {
                return false;
            }
            return true;
        }
        internal void SetGrid(Vector2Int grid, IChess chess)
        {
            chesses[grid.x, grid.y] = chess;
        }
        internal IChess GetChess(Vector2Int grid)
        {
            return chesses[grid.x, grid.y];
        }
        IChess[,] chesses;
    }
}
