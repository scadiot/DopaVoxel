#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix World;
matrix WorldViewProjection;
float3 SunOrientation;
float4 SunLightColor;
float4 ArtificialLightColor;
float4 AmbiantColor;
float3 CameraPos;
float FogStart;
float FogEnd;
float Opacity;
float4 FogColor;
Texture2D Texture1;

sampler TextureSampler1 = 
sampler_state
{
    Texture = <Texture1>;
    MipFilter = LINEAR;
    MinFilter = POINT;
    MagFilter = POINT;
};

struct VertexShaderInput
{
	float4 Position : SV_POSITION;
	float3 Normal : NORMAL0;
	float2 Uv : TEXCOORD0;
	float SunLight : COLOR0;
	float ArtificialLight : COLOR1;
	float OcclusionLight : COLOR2;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float3 Normal : NORMAL0;
	float2 Uv : TEXCOORD0;
	float SunLight : COLOR0;
	float ArtificialLight : COLOR1;
	float OcclusionLight : COLOR2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, WorldViewProjection);
	output.Normal = input.Normal;
	output.Uv = input.Uv;
	output.SunLight = input.SunLight;
	output.ArtificialLight = input.ArtificialLight;
	output.OcclusionLight = input.OcclusionLight;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	//float cosTheta = clamp( dot( input.Normal, SunOrientation), 0, 1);
	//float4 light = saturate((SunColor * cosTheta) + AmbiantColor);
	
	float4 ResultColor = Texture1.Sample(TextureSampler1, input.Uv) * saturate((ArtificialLightColor * input.ArtificialLight) + (SunLightColor * input.SunLight));
	ResultColor.a = Opacity;
	
	return ResultColor;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};