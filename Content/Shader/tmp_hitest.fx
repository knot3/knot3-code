float4x4 WVP;
texture cubeTexture;
  
sampler TextureSampler = sampler_state
{
 texture = <cubeTexture>; 
 mipfilter = LINEAR;
 minfilter = LINEAR;
 magfilter = LINEAR;
};
 
struct InstancingVSinput
{
 float4 Position : POSITION0;
 float2 TexCoord : TEXCOORD0;
};
  
struct InstancingVSoutput
{
 float4 Position : POSITION0;
 float2 TexCoord : TEXCOORD0;
};
  
InstancingVSoutput InstancingVS(InstancingVSinput input, float4x4 instanceTransform : TEXCOORD1, float2 atlasCoord : TEXCOORD5)
{
 InstancingVSoutput output;
 float4 pos = input.Position;
 pos = mul(pos, transpose(instanceTransform));
 pos = mul(pos, WVP);
 output.Position = pos;
 output.TexCoord = float2((input.TexCoord.x / 2.0f) + (1.0f / 2.0f * atlasCoord.x), 
 (input.TexCoord.y / 2.0f) + (1.0f / 2.0f * atlasCoord.y));
 return output;
}
  
float4 InstancingPS(InstancingVSoutput input) : COLOR0
{
 return tex2D(TextureSampler, input.TexCoord);
}
  
technique Instancing
{
 pass Pass0
 {
 VertexShader = compile vs_3_0 InstancingVS();
 PixelShader = compile ps_3_0 InstancingPS();
 }
}