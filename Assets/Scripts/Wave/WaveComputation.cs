using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


public class WaveComputation
{
    public readonly int max_num_waves, num_wave_samples;
    readonly int kernel;
    ComputeShader shader;
    public RenderTexture waveHeights;

    PrecomputeHelper helper;
    Texture2D kTexture, dVTexture;



    public WaveComputation(ComputeShader cs, Vector2 waveType,int maxWaveNum, int waveSampleNum)
    {
        max_num_waves = maxWaveNum;
        num_wave_samples = waveSampleNum;
        
        shader = cs;
        kernel = shader.FindKernel("CSMain");

        helper = new PrecomputeHelper(out kTexture,out dVTexture);

        waveHeights = new RenderTexture(num_wave_samples, max_num_waves, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
        waveHeights.enableRandomWrite = true;
        waveHeights.wrapMode = TextureWrapMode.Clamp;
        waveHeights.Create();

        //cbuffer
        shader.SetInt("num_wave_samples", num_wave_samples);
        shader.SetVector("V_bounds", new Vector2(helper.V_min, helper.V_max));
        shader.SetVector("wave_types", waveType);
        shader.SetFloat("g", PrecomputeHelper.g);
        shader.SetFloat("R_star", PrecomputeHelper.R_star);
        shader.SetFloat("nu", PrecomputeHelper.nu);
        shader.SetFloat("alpha", helper.alpha);
        shader.SetFloat("tau", helper.tau);

        //texture
        shader.SetTexture(kernel, "Result", waveHeights);
        shader.SetTexture(kernel, "_kTexture", kTexture);
        shader.SetTexture(kernel, "_dVTexture", dVTexture);
    }

    public void ComputeWave(ComputeBuffer sbuffer)
    {
        //have to update each computation
        shader.SetBuffer(kernel, "perWaveData", sbuffer);

        shader.Dispatch(kernel, num_wave_samples / 512, sbuffer.count, 1);
    }

    public RenderTexture GetWaveHeights()
    {
        return waveHeights;
    }

    public List<Vector4> ComputeWaveDatas(ICollection waves)
    {
        return helper.computeWaveData(waves);
    }
}

public class PrecomputeHelper
{
    public static readonly float WC_PI = 3.14159265359f;
    public static readonly float rho = 1.0f, g = 980.0f, R_star = 0.2f, It = 273.61f, tau_star = 74.0f, nu = 50e-3f;

    public float tau, W, W_star, alpha, beta;
    public float V_min, V_max;

    readonly int lookupLength = 2048;
    public PrecomputeHelper(out Texture2D kTexture, out Texture2D dVTexture)
    {
        // Init Constants (for unit conversion)
        tau = tau_star / (rho * g * R_star * R_star);
        W = It * 3 / (4 * WC_PI);
        W_star = W * Mathf.Sqrt(g * R_star);
        alpha = (3 * Mathf.Sqrt(g * R_star)) / (16 * R_star * W_star);
        beta = (3 * g) / (2 * R_star * R_star * R_star * W_star);
        precomputeDominantWavenumbers(out kTexture, out dVTexture);
    }

    public List<Vector4> computeWaveData(ICollection waves)
    {
        List<Vector4> datas = new List<Vector4>(waves.Count);
        // Update per-wave data such as time, min/max radius
        var fac = Mathf.Sqrt(g / R_star);
        foreach (Wave wave in waves)
        {
            var t_star = wave.t_star;

            var t_star_eff = (t_star * t_star + t_star + 0.15f) / (t_star + 1.0f);
            var t = t_star * fac;
            var t_eff = t_star_eff * fac;
            Vector4 data = new Vector4(t * V_min, t * V_min * 4.0f, t_eff);
            datas.Add(data);
        }
        return datas;
    }

    public void precomputeDominantWavenumbers(out Texture2D kLookup, out Texture2D dVLookup)
    {
        Func<float, float> lambda_V = k => V(k);
        Func<float, float> lambda_dV = k => dV(k);
        Func<float, float> lambda_ddV = k => ddV(k);

        var k_mid = newtonsMethod(lambda_V, lambda_ddV, 0f, 0.25f);
        V_max = 100.0f;
        V_min = V(k_mid);
        var k_lb = newtonsMethod(lambda_V, lambda_dV, V_max, 1e-5f);
        var k_ub = newtonsMethod(lambda_V, lambda_dV, V_max, 1e3f);

        int n = lookupLength;
        float interv = (V_max - V_min) / (float)n;
        float V_start = V_min + interv * 0.5f;

        var V_vals = new float[n];
        var k_vals = new Vector2[n];
        var dV_vals = new Vector2[n];
        for (int i = 0; i < n; i++)
        {
            var V_val = V_start + interv * i;
            var k1 = newtonsMethod(lambda_V, lambda_dV, V_val, k_lb);
            var frac = (1f + i) / (float)n;
            var k2 = newtonsMethod(lambda_V, lambda_dV, V_val, 0.01f * frac * k_ub + (1.0f - 0.01f * frac) * k_mid);
            V_vals[i] = V_val;
            k_vals[i] = new Vector2(k1, k2);
            dV_vals[i] = new Vector2(dV(k1), dV(k2));
        }

        kLookup = new Texture2D(lookupLength, 1, TextureFormat.RGFloat, false, true)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        dVLookup = new Texture2D(lookupLength, 1, TextureFormat.RGFloat, false, true)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        kLookup.SetPixelData(k_vals, 0);
        kLookup.Apply();
        dVLookup.SetPixelData(dV_vals, 0);
        dVLookup.Apply();
    }



    float newtonsMethod(Func<float, float> f, Func<float, float> df, float val, float p, float tol = 1e-10f, uint max_iter = 50)
    {
        var v = f(p) - val;
        var iter = 0u;
        while ((v > tol || v < -tol) && iter++ < max_iter)
        {
            var tmep = df(p);
            p = p - v / tmep;
            v = f(p) - val;
        }
        return p;
    }

    //float sigma(float k)
    //{
    //    return Mathf.Sqrt(k * (1.0f + tau * k * k));
    //}

    float V(float k)
    {
        return (1.0f + 3.0f * tau * k * k) / (2.0f * Mathf.Sqrt(k * (1.0f + tau * k * k)));
    }

    float dV(float k)
    {
        var ksq = k * k;
        var tmp = tau * ksq * k + k;
        return (3.0f * tau * ksq * (tau * ksq + 2) - 1.0f) / (4.0f * Mathf.Sqrt(tmp * tmp * tmp));
    }

    float ddV(float k)
    {
        var ksq = k * k;
        return -(3.0f * (ksq * tau - 1.0f) * (ksq * tau * (ksq * tau + 6) + 1)) / (8 * Mathf.Sqrt(Mathf.Pow(ksq * k * tau + k, 5.0f)));
    }
}