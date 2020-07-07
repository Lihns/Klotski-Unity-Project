using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRT : MonoBehaviour
{
    public RenderTexture texture;
    
    IEnumerator OnPostRender()
    {
        yield return new WaitForEndOfFrame();
        Graphics.DrawTexture(new Rect(0,0,texture.width,texture.height), texture);
    }
}
