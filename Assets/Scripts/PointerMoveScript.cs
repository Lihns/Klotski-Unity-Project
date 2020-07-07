using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PointerMoveScript : MonoBehaviour
{
    public float sensitivity = 0.01f;
    public float lerpFactor = 0.1f;
    public float attractFactor = 0.1f;
    public float captureThreshold = 0.5f;

    public ChessBoardScript chessBoard;

    Vector3 destination;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        destination = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //move direction contributed by mouse
        var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 pointerMov = new Vector3(mouseMovement.x, 0, mouseMovement.y) * sensitivity;
        destination += pointerMov;

        ////move contributed by attractor
        //Vector3 attractorPosition = chessBoard.GetRelativeGridCenterPosition(transform.localPosition);
        //Vector3 pointerToAttractor = attractorPosition - transform.localPosition;
        //pointerToAttractor.y = 0;
        //Vector3 attractMov = pointerToAttractor * attractFactor;
        //attractMov = Vector3.Lerp(attractMov, Vector3.zero, pointerToAttractor.magnitude + 1 - captureThreshold);
        //destination += attractMov;

        //lerp and apply the transform
        transform.localPosition = Vector3.Lerp(transform.localPosition, destination, lerpFactor);


        //move chess
        //
    }
}
