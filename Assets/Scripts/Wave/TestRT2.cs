using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRT2 : MonoBehaviour
{
    public RenderTexture rt;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var texReadback = new Texture2D(rt.width, rt.height, TextureFormat.RFloat, false, true);
        Graphics.SetRenderTarget(rt);
        texReadback.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);
        Graphics.SetRenderTarget(null);
        texReadback.Apply();

        var data = texReadback.GetRawTextureData<float>();
        for (int i = 0; i < data.Length; i++)
        {
            float t = data[i];
            if (t < 1)
            {
                print("asd");
            }
        }
    }
}
