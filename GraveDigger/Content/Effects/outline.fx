#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

float4 OutlineColor = float4(1, 1, 1, 1);
float2 TextureSize;

sampler TextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 pixelSize = 1.0 / TextureSize;
    
    float centerAlpha = tex2D(TextureSampler, input.TexCoord).a;
    
    float neighbourAlpha = 0.0;
    
    neighbourAlpha = max(
            neighbourAlpha,
            tex2D(TextureSampler, input.TexCoord + float2(pixelSize.x, 0)).a
        );
        
    neighbourAlpha = max(
            neighbourAlpha,
            tex2D(TextureSampler, input.TexCoord + float2(-pixelSize.x, 0)).a
        );
        
    neighbourAlpha = max(
            neighbourAlpha,
            tex2D(TextureSampler, input.TexCoord + float2(0, pixelSize.y)).a
        );
        
    neighbourAlpha = max(
                neighbourAlpha,
                tex2D(TextureSampler, input.TexCoord + float2(0, -pixelSize.y)).a
            );
    
    
    if (centerAlpha < 0.1 && neighbourAlpha > 0.1)
    {    
        return float4(
            OutlineColor.rgb,
            OutlineColor.a
        );
    }
    return float4(0, 0, 0, 0);
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}