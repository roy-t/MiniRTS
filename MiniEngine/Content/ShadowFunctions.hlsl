// From: http://dev.theomader.com/cascaded-shadow-mapping-3/

#define NumSplits 4

// Bias to prevent shadow acne
static const float bias = 0.01f;

float4x4 ShadowTransform[NumSplits];
float4 TileBounds[NumSplits];

texture ShadowMap;
sampler shadowSampler = sampler_state
{
    Texture = (ShadowMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
};

struct ShadowData
{
    float4 TexCoords_0_1;
    float4 TexCoords_2_3;
    float4 LightSpaceDepth;
};

// compute shadow parameters (tex coords and depth) 
// for a given world space position
ShadowData GetShadowData(float4 worldPosition)
{
    ShadowData result = (ShadowData)0;
    float4 texCoords[NumSplits];
    float lightSpaceDepth[NumSplits];
 
    for( int i=0; i<NumSplits; ++i )
    {
        float4 lightSpacePosition = mul(worldPosition, ShadowTransform[i]);
        texCoords[i] = lightSpacePosition / lightSpacePosition.w;
        lightSpaceDepth[i] = texCoords[i].z;
    }
 
    result.TexCoords_0_1 = float4(texCoords[0].xy, texCoords[1].xy);
    result.TexCoords_2_3 = float4(texCoords[2].xy, texCoords[3].xy);
    result.LightSpaceDepth = float4( lightSpaceDepth[0], 
                                     lightSpaceDepth[1], 
                                     lightSpaceDepth[2], 
                                     lightSpaceDepth[3] );
 
    return result;
}

struct ShadowSplitInfo
{
    float2 TexCoords;
    float  LightSpaceDepth;
    int    SplitIndex;
};

// find split index, texcoords and light space depth for given shadow data
ShadowSplitInfo GetSplitInfo(ShadowData shadowData)
{   
    float2 shadowTexCoords[NumSplits] = 
    {
        shadowData.TexCoords_0_1.xy, 
        shadowData.TexCoords_0_1.zw,
        shadowData.TexCoords_2_3.xy,
        shadowData.TexCoords_2_3.zw
    };
 
    float lightSpaceDepth[NumSplits] = 
    {
        shadowData.LightSpaceDepth.x,
        shadowData.LightSpaceDepth.y,
        shadowData.LightSpaceDepth.z,
        shadowData.LightSpaceDepth.w,
    };
     
    for(int splitIndex=0; splitIndex < NumSplits; splitIndex++)
    {
        if(shadowTexCoords[splitIndex].x >= TileBounds[splitIndex].x && 
           shadowTexCoords[splitIndex].x <= TileBounds[splitIndex].y && 
           shadowTexCoords[splitIndex].y >= TileBounds[splitIndex].z && 
           shadowTexCoords[splitIndex].y <= TileBounds[splitIndex].w)
        {
            ShadowSplitInfo result;
            result.TexCoords = shadowTexCoords[splitIndex];
            result.LightSpaceDepth = lightSpaceDepth[splitIndex];
            result.SplitIndex = splitIndex;
 
            return result;
        }
    }
 
    ShadowSplitInfo result = { float2(0,0), 0, NumSplits };
    return result;
}

// compute shadow factor: 0 if in shadow, 1 if not
float GetShadowFactor(ShadowData shadowData)
{
    ShadowSplitInfo splitInfo = GetSplitInfo( shadowData );
    float storedDepth = tex2Dlod( shadowSampler, float4( splitInfo.TexCoords, 0, 0)).r;
 
    return ((splitInfo.LightSpaceDepth - bias) <  storedDepth);
}