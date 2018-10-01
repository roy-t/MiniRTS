// Stores distances from the light source to the nearest visible object

struct SMVertexShaderInput
{
    float4 Position : POSITION0;
};

struct SMVertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Position2D : TEXCOORD0;
};

SMVertexShaderOutput SMMainVS(in SMVertexShaderInput input)
{
    SMVertexShaderOutput output = (SMVertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.Position2D = output.Position;

    return output;
}

struct SMPixelShaderOutput
{
    float4 Color : COLOR0;
};

SMPixelShaderOutput SMMainPS(SMVertexShaderOutput input)
{
    SMPixelShaderOutput output = (SMPixelShaderOutput)0;

    output.Color = input.Position2D.z / input.Position2D.w;

    return output;
}

technique ShadowMap
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL SMMainVS();
        PixelShader = compile PS_SHADERMODEL SMMainPS();
    }
}
