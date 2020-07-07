using Packages.Rider.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Wave
{
    readonly static float t_star_max = 10.0f;
    public float t_star;
    public Vector3 position;

    public Wave()
    {
        position = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
    }
    public Wave(Vector3 position)
    {
        this.position = position; 
    }
    public bool isFinished()
    {
        return t_star > t_star_max;
    }
}

public class RainRenderer : MonoBehaviour
{
    public float ripplePerSecond = 1.0f;
    public float scale = 10.0f;
    public float amplitude = 1.0f;
    public Vector2 waveType = new Vector2(1, 1);
    public int numberOfWaves = 16;
    public int numberOfSamples = 1024;
    public ComputeShader computeShader = null;
    public Material rippleMaterial = null;
    public Mesh quad = null;
    public Camera topDownCam = null;
    public RenderTexture waveHeights = null;

    Queue<Wave> waveQueue;
    WaveComputation computation;
    ComputeBuffer sbuffer;

    // Start is called before the first frame update
    void Start()
    {
        computation = new WaveComputation(computeShader, waveType,numberOfWaves,numberOfSamples);
        waveHeights = computation.GetWaveHeights();
        waveQueue = new Queue<Wave>();
    }

    private void OnDestroy()
    {
        sbuffer.Release();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    Physics.Raycast(ray, out hit,100);
        //    if (waveQueue.Count >= computation.max_num_waves)
        //    {
        //        waveQueue.Dequeue();
        //    }
        //    waveQueue.Enqueue(new Wave(hit.point));
        //}

        if (Random.Range(0.0f,1.0f)< ripplePerSecond * Time.deltaTime)
        {
            if (waveQueue.Count >= computation.max_num_waves)
            {
                waveQueue.Dequeue();
            }
            waveQueue.Enqueue(new Wave());
        }

        foreach (Wave wave in waveQueue)
        {
            wave.t_star += Time.deltaTime;
        }
        while (waveQueue.Count > 0)
        {
            var wave = waveQueue.Peek();
            if (wave.isFinished()) waveQueue.Dequeue();
            else break;
        }
        if (waveQueue.Count > 0)
        {
            var waveDatas = computation.ComputeWaveDatas(waveQueue);
            
            if (sbuffer != null) sbuffer.Release();
            sbuffer = new ComputeBuffer(waveDatas.Count, Marshal.SizeOf<Vector4>(), ComputeBufferType.Structured);
            sbuffer.SetData(waveDatas);

            computation.ComputeWave(sbuffer);
            RenderRippleQuads(waveHeights);
        }
    }
    void RenderRippleQuads(Texture texture)
    {
        rippleMaterial.SetTexture("waveHeightTexture", texture);
        rippleMaterial.SetFloat("R_star", PrecomputeHelper.R_star);
        rippleMaterial.SetFloat("textureHeight", texture.height);
        rippleMaterial.SetFloat("amplitude", amplitude);
        rippleMaterial.SetBuffer("perWaveData", sbuffer);

        List<Matrix4x4> matrix4X4s = new List<Matrix4x4>(waveQueue.Count);
        foreach (Wave wave in waveQueue)
        {
            matrix4X4s.Add(Matrix4x4.Translate(wave.position*scale));
        }

        Graphics.DrawMeshInstanced(quad, 0, rippleMaterial, matrix4X4s, null, ShadowCastingMode.Off, false,1,topDownCam);
    }
}
