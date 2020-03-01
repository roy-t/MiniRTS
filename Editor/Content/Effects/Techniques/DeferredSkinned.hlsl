// Stores the color, normal and depth information in three separate render targets
// to construct a geometry-buffer. Also samples the skybox for reflections

struct DeferredSkinnedVertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
    float3 Binormal : BINORMAL0;
    float3 Tangent : TANGENT0;
    uint4  Indices : BLENDINDICES0;
    float4 Weights : BLENDWEIGHT0;
};

struct DeferredSkinnedVertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 Depth : TEXCOORD1;
    float4 ScreenPosition : TEXCOORD2;
    float3x3 tangentToWorld : TEXCOORD3;    
    float4 Color : POSITION1;
};

float4x4 BoneTransforms[16];

void Skin(inout DeferredSkinnedVertexShaderInput vin)
{
    float4x4 skinning = 0;

    [unroll]
    for (int i = 0; i < 4; i++)
    {
        skinning += BoneTransforms[vin.Indices[i]] * vin.Weights[i];
    }

    vin.Position.xyz = mul(float4(vin.Position.xyz, 1), skinning);
    vin.Normal = mul(vin.Normal, (float3x3)skinning);
    vin.Binormal = mul(vin.Binormal, (float3x3)skinning);
    vin.Tangent = mul(vin.Tangent, (float3x3)skinning);
}


DeferredSkinnedVertexShaderOutput DeferredSkinnedMainVS(in DeferredSkinnedVertexShaderInput input)
{
    DeferredSkinnedVertexShaderOutput output = (DeferredSkinnedVertexShaderOutput)0;

    Skin(input);

    //float3 skinPosition = mul(float4(input.Position.xyz, 1), Foo).xyz;
    //float4 worldPosition = mul(float4(skinPosition, 1), World);
    float4 worldPosition = mul(float4(input.Position.xyz, 1), World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.TexCoord = input.TexCoord;
    output.Depth.x = output.Position.z;
    output.Depth.y = output.Position.w;

    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors
    output.tangentToWorld[0] = mul(float4(input.Tangent, 0), World).xyz;
    output.tangentToWorld[1] = mul(float4(input.Binormal, 0), World).xyz;
    output.tangentToWorld[2] = mul(float4(input.Normal, 0), World).xyz;

    output.ScreenPosition = output.Position;

    return output;
}

struct DeferredSkinnedPixelShaderOutput
{
    float4 Color : COLOR0;
    float4 Normal : COLOR1;
    float Depth : COLOR2;
};

DeferredSkinnedPixelShaderOutput DeferredSkinnedMainPS(DeferredSkinnedVertexShaderOutput input)
{
    DeferredSkinnedPixelShaderOutput output = (DeferredSkinnedPixelShaderOutput)0;
    float2 texCoord = input.TexCoord;

    float mask = tex2D(maskSampler, texCoord).r;
    clip(mask - 0.05f);

    // Diffuse        
    output.Color = tex2D(diffuseSampler, texCoord);
    clip(output.Color.a - 0.01f);


    // Normal   
    float3 normal = UnpackNormal(tex2D(normalSampler, texCoord).xyz);
    normal = normalize(mul(normal, input.tangentToWorld));
    output.Normal.rgb = PackNormal(normal);

    // Specular
    float specularPower = tex2D(specularSampler, texCoord).r;

    // Shininess is stored in textures with black is most shiney, and white is non-shiney
    // make 1.0f most shiney here   
    output.Normal.a = 1.0f - specularPower;

    output.Depth = (input.Depth.x / input.Depth.y);

    // Reflections
    float3 reflectionFactor = tex2D(reflectionSampler, texCoord).rgb;
    float l = saturate(length(reflectionFactor) > 0);
    if (l > 0)
    {
        // The more reflective this part is, the bigger part of its color will be the reflection color
        float3 reflection = SampleSkybox(skyboxSampler, CameraPosition, input.ScreenPosition, InverseViewProjection, output.Depth, normal).rgb;
        output.Color.rgb = lerp(output.Color.rgb, reflectionFactor * reflection, l);
    }

    return output;
}

technique DeferredSkinned
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL DeferredSkinnedMainVS();
        PixelShader = compile PS_SHADERMODEL DeferredSkinnedMainPS();
    }
}
