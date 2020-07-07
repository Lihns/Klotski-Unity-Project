Shader "MyGame/Ripple"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #pragma target 4.5

            struct appdata
            {
                float4 vertex : POSITION;
                uint wave_index : SV_INSTANCEID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 vf_position : OUTPUT0;
                float vf_bound_ratio : OUTPUT1;
                float vf_wave_coord : OUTPUT2;
            };

            
            StructuredBuffer<float4> perWaveData;
            sampler2D waveHeightTexture;
            float R_star;
            float textureHeight;
            float amplitude;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                float rmin=perWaveData[v.wave_index].x;
                float rmax=perWaveData[v.wave_index].y;
                float r_star_max = rmax * R_star;
                float4 scaled = 2 * r_star_max * v.vertex;
                //float4 scaled = 2 * v.vertex;

                o.vf_wave_coord = ((float)v.wave_index+0.5f)/textureHeight;
                o.vf_position = 2 * v.vertex.xz;
                o.vf_bound_ratio = rmin / rmax;
                o.vertex = UnityObjectToClipPos(scaled);
                return o;
            }

            float frag(v2f i) : SV_Target
            {
                float2 tc = float2(
                                    (length(i.vf_position) - i.vf_bound_ratio) / (1 - i.vf_bound_ratio),
		                            i.vf_wave_coord
                                    );
                float height = amplitude * tex2D(waveHeightTexture, tc);
                //if(abs(height)>0) return 0;
                //else return height;
                return clamp(height,-1000,1000);
            }
            ENDCG
        }
    }
}
