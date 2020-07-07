using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    public GameObject pointer;

    Vector3 origin;
    // Start is called before the first frame update
    void Start()
    {
        origin = pointer.transform.position - transform.position; ;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camToPointer = pointer.transform.position - transform.position;
        Vector3 bias = camToPointer - origin;
        float len = bias.magnitude;
        float factor = 1 - 1 / (1 + len);
        bias.Normalize();
        bias *= factor;
        //print(bias);
        Vector3 dir = origin + bias;
        dir = Vector3.Lerp(transform.forward, dir, 0.1f);
        transform.LookAt(transform.position + dir);
    }
}
