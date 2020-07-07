struct appdata
{
    float4 vertex : POSITION;
    int wave_index : SV_INSTANCEID;
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float2 vf_position : OUTPUT;
                //float2 vf_translation;
    float vf_bound_ratio : OUTPUT;
    nointerpolation int vf_wave_index : OUTPUT;
};

struct WaveData
{
    float rmin;
    float rmax;
    float t;
};

StructuredBuffer<WaveData> perWaveData; //¿¼ÂÇfloatarray
sampler2D waveHeightTexture;
float R_star = 0.2f;
float textureHeight;
float amplitude;

v2f vert(appdata v)
{
    v2f o;
    WaveData data = perWaveData[v.wave_index];
    float t = data.t;
    float r_star_max = data.rmax * R_star;
    float4 scaled = 2 * r_star_max * v.vertex;
    o.vf_position = 2 * v.vertex.xz;
    o.vf_wave_index = v.wave_index;
    o.vf_bound_ratio = data.rmin / data.rmax;
    o.vertex = UnityObjectToClipPos(scaled);
    return o;
}
/*
layout(location=0) in vec2 position;
layout(location=1) in vec2 translation;
layout(location=2) in vec3 wave_data;

uniform float R_star;

out vec2 vf_position;
flat out int vf_wave_index;
out float vf_bound_ratio;
out vec2 vf_translation;

uniform mat4 mvp_matrix;

// This shader draws a quad in screen-space and renders the wave height in the red color component
void main(void)
{
	// Determine size of wave quaad
	float t = wave_data.z;
	float r_star_max = wave_data[1] * R_star;

	vec2 p = 2 * r_star_max * position + translation;
	vf_position = 2 * position;
	vf_wave_index = gl_InstanceID;
	vf_bound_ratio = wave_data[0] / wave_data[1];
	// Transform quad to clip coordinates
	gl_Position = mvp_matrix * vec4(p.y, 0.0, p.x, 1.0);
	vf_translation = translation;
}
*/

float frag(v2f i) : SV_Target
{
    float2 tc = float2((length(i.vf_position) - i.vf_bound_ratio) / (1 - i.vf_bound_ratio),
		                (float(i.vf_wave_index) + 0.5f) / textureHeight);
    float height = amplitude * tex2D(waveHeightTexture, tc);
    return height;
}
/*
in vec2 vf_position;
flat in int vf_wave_index;
in float vf_bound_ratio;
in vec2 vf_translation;

uniform sampler2D wave_height_texture;

layout(location=0) out float wave_height;

void main(void)
{
	// Compute radius at current pixel
	vec2 tc = vec2((length(vf_position) - vf_bound_ratio) / (1 - vf_bound_ratio),
		(float(vf_wave_index) + 0.5f) / textureSize(wave_height_texture, 0).y);

	// Compute random amplitude
	vec2 rand_vec = fract(vf_translation*1000);
	float rand_num = 0.5 * (rand_vec.x + rand_vec.y);
	float rand_ampl = rand_num*rand_num + 0.01;

	// Do lookup in wave-height buffer
	wave_height = rand_ampl * texture(wave_height_texture, tc).r;
}
*/
// sample the texture