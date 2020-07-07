using UnityEngine;
using System.Collections;
using System.IO;
using System.Data;

public class NegativeEXRTest : MonoBehaviour
{
    public int resolutionX = 16;
    public int resolutionY = 16;
    public TextureFormat tex2DFormat = TextureFormat.RGBAHalf;
    public RenderTextureFormat rtFormat = RenderTextureFormat.ARGBHalf;
    public Texture2D.EXRFlags exrFlags = Texture2D.EXRFlags.None;

    void Start()
    {
        // Create an array of colors that has RGB values from -1.0 to 1.0
        Color[] colors = new Color[resolutionX * resolutionY];
        int numPixels = resolutionX * resolutionY;
        for (int i = 0; i < numPixels; i++)
        {
            float a = ((float)i / (float)(numPixels - 1)) * 2f - 1f;
            colors[i] = new Color(a, a, a, 1f);
        }

        // Create Texture2D and set pixels colors
        var tex = new Texture2D(resolutionX, resolutionY, tex2DFormat, false, true);
        tex.SetPixels(colors, 0);
        tex.Apply();

        // Create RenderTexture
        RenderTexture rt = new RenderTexture(resolutionX, resolutionY, 0, rtFormat, RenderTextureReadWrite.Linear);
        rt.Create();

        // Copy Texture2D to RenderTexture
        // It would be faster to use CopyTexture, but Blit() works by rendering the source texture into the destination
        // render texture with a simple unlit shader.
        Graphics.Blit(tex, rt);

        // Read RenderTexture contents into a new Texture2D using ReadPixels
        var texReadback = new Texture2D(resolutionX, resolutionY, tex2DFormat, false, true);
        Graphics.SetRenderTarget(rt);
        texReadback.ReadPixels(new Rect(0, 0, resolutionX, resolutionY), 0, 0, false);
        Graphics.SetRenderTarget(null);
        texReadback.Apply();

        var data = texReadback.GetRawTextureData<float>();
        for (int i = 0; i < data.Length; i++)
        {
            float t = data[i];
        }

        // Destroy texture objects
        Object.DestroyImmediate(tex);
        Object.DestroyImmediate(texReadback);
        Object.DestroyImmediate(rt);
    }
}
