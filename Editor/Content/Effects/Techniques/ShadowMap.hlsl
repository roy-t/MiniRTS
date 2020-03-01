// Stores distances from the light source to the nearest visible object

struct SMVertexShaderInput
{
    float4 Position : POSITION0;
};

struct SMSkinnedVertexShaderInput
{
    float4 Position : POSITION0;
    uint4  Indices : BLENDINDICES0;
    float4 Weights : BLENDWEIGHT0;
};

struct SMVertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Position2D : TEXCOORD0;
};

void Skin(inout SMSkinnedVertexShaderInput vin)
{
    float4x4 skinning = 0;

    [unroll]
    for (int i = 0; i < 4; i++)
    {
        skinning += BoneTransforms[vin.Indices[i]] * vin.Weights[i];
    }

    vin.Position.xyz = mul(float4(vin.Position.xyz, 1), skinning).xyz;
}

SMVertexShaderOutput SMMainVS(in SMVertexShaderInput input)
{
    SMVertexShaderOutput output = (SMVertexShaderOutput)0;

    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.Position2D = output.Position;

    return output;
}

SMVertexShaderOutput SMSkinnedMainVS(in SMSkinnedVertexShaderInput input)
{
    SMVertexShaderInput output = (SMVertexShaderInput)0;

    Skin(input);

    output.Position = input.Position;

    return SMMainVS(output);
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

technique ShadowMapSkinned
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL SMSkinnedMainVS();
        PixelShader = compile PS_SHADERMODEL SMMainPS();
    }
}
