// Stores the color, normal and depth information in three separate render targets
// to construct a geometry-buffer. Also samples the skybox for reflections

struct DeferredVertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
    float3 Binormal : BINORMAL0;
    float3 Tangent : TANGENT0;
};

struct DeferredVertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 Depth : TEXCOORD1;
    float4 ScreenPosition : TEXCOORD2;
    float3x3 tangentToWorld : TEXCOORD3;    
};

DeferredVertexShaderOutput DeferredMainVS(in DeferredVertexShaderInput input)
{
    DeferredVertexShaderOutput output = (DeferredVertexShaderOutput)0;

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

struct DeferredPixelShaderOutput
{
    float4 Color : COLOR0;
    float4 Normal : COLOR1;
    float Depth : COLOR2;
};


float4 SampleReflection(float4 screenPosition, float depth, float3 normal)
{
    float2 texCoord = ToTextureCoordinates(screenPosition.xy, screenPosition.w);
    float4 position = ReadWorldPosition(texCoord, depth, InverseViewProjection);

    float3 viewDirection = CameraPosition - position.xyz;

    float3 reflection = reflect(-normalize(viewDirection), normal);
    return texCUBE(skyboxSampler, normalize(reflection));    
}

DeferredPixelShaderOutput DeferredMainPS(DeferredVertexShaderOutput input)
{
    DeferredPixelShaderOutput output = (DeferredPixelShaderOutput)0;
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
    output.Color = SampleReflection(input.ScreenPosition, output.Depth, normal);

    return output;
}

technique Deferred
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL DeferredMainVS();
        PixelShader = compile PS_SHADERMODEL DeferredMainPS();
    }
}
