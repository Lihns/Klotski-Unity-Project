using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ChessMove
{
    readonly Transform chess;
    readonly float speed;
    readonly Vector3 startPos;
    readonly Vector3 direction;
    readonly AnimationCurve curve;
    float startTime;

    public ChessMove(Transform chess, AnimationCurve curve, Vector3 startPosition, Vector3 direction, float speed)
    {
        this.chess = chess;
        this.speed = speed;
        this.direction = direction;
        this.curve = curve;
        startPos = startPosition;
        startTime = -1;
    }
    public bool Move()
    {
        if (startTime<0)
        {
            startTime = Time.time;
        }
        float sampleTime = speed * (Time.time - startTime);
        if (sampleTime > 1)
        {
            return false;
        }
        float portion = curve.Evaluate(sampleTime);
        chess.position = startPos + portion * direction;
        return true;
    }
}

public class ChessBoardScript : MonoBehaviour
{
    public GameScript game;
    public Transform pointer;
    public Rect bounds;
    public float unitGridLength = 1;
    public float chessSpeed = 1;
    public AnimationCurve curve;

    Transform selectedChess;
    Vector3 chessDestination;
    Queue<ChessMove> moveQueue;

    private void Start()
    {
        moveQueue = new Queue<ChessMove>();
    }

    private void Update()
    {
        //left clicked
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedChess != null)
            {
                MoveChess();
            }
            else
            {
                Vector3 camPos = Camera.main.transform.position;
                Ray ray = new Ray(camPos, pointer.position - camPos);
                Physics.Raycast(ray, out RaycastHit hit, 10);
                //if hit chess
                if (hit.transform.CompareTag("Chess"))
                {
                    SelectChess(hit.transform);
                }
            }
        }
        //could move chess
        if (moveQueue.Count != 0)
        {
            ChessMove move = moveQueue.Peek();
            if (!move.Move())
                moveQueue.Dequeue();
        }

        //deselect chess
        if (Input.GetMouseButtonDown(1))
        {
            DeselectChess();
        }
    }
    public void DeselectChess()
    {
        selectedChess.GetComponent<Animator>().SetBool("selected", false);
        selectedChess = null;
    }

    public void SelectChess(Transform chess)
    {
        selectedChess = chess;
        chessDestination = selectedChess.position;
        selectedChess.GetComponent<Animator>().SetBool("selected", true);
    }


    public Vector3 GetAttractorPosition(Vector3 localPosition)
    {
        //move contrubuted by attractor
        //calculate offset to the origin
        Vector3 offset;
        if (selectedChess != null)
        {
            offset = localPosition - selectedChess.localPosition;
            offset.x = Mathf.Clamp(offset.x, -unitGridLength, unitGridLength);
            offset.z = Mathf.Clamp(offset.x, -unitGridLength, unitGridLength);
        }
        else
        {
            offset = localPosition;
            offset.x = Mathf.Clamp(offset.x, bounds.xMin, bounds.xMax);
            offset.z = Mathf.Clamp(offset.x, bounds.yMin, bounds.yMax);
        }

        offset /= unitGridLength;
        offset.x = Mathf.Round(offset.x);
        offset.z = Mathf.Round(offset.z);
        return offset;
    }

    public void MoveChess()
    {
        Vector3 chessToPointer = pointer.position - selectedChess.position;
        chessToPointer.y = 0;
        if (Mathf.Abs(chessToPointer.x) > Mathf.Abs(chessToPointer.z))
        {
            chessToPointer.x /= Mathf.Abs(chessToPointer.x);
            chessToPointer.z = 0;
        }
        else
        {
            chessToPointer.x = 0;
            chessToPointer.z /= Mathf.Abs(chessToPointer.z);
        }
        Vector2Int dir2D = new Vector2Int((int)chessToPointer.x, (int)chessToPointer.z);
        //if pointing right vertical/horizental to the chess
        //chess move with the pointer
        Direction dir;
        if (dir2D.y == 1)
        {
            dir = Direction.up;
        }
        else if (dir2D.y == -1)
        {
            dir = Direction.down;
        }
        else if (dir2D.x == -1)
        {
            dir = Direction.left;
        }
        else if (dir2D.x == 1)
        {
            dir = Direction.right;
        }
        else return;
        if (game.MoveChess(selectedChess.name, dir))
        {
            ChessMove move = new ChessMove(selectedChess, curve, chessDestination, chessToPointer, chessSpeed);
            chessDestination = chessDestination + chessToPointer;
            moveQueue.Enqueue(move);
        }

    }
}
