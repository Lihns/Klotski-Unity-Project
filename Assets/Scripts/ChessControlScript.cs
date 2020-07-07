using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessControlScript : MonoBehaviour
{
    public void ChessMoveTo(GameObject chess, Vector3 destination)
    {
        if (chess.CompareTag("Chess"))
        {
            chess.transform.position = destination;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
