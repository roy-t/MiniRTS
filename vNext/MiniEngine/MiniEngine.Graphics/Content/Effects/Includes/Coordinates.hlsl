float2 ScreenToTexture(float2 screenPosition)
{
    return 0.5f * float2(screenPosition.x, -screenPosition.y) + 0.5f;
}
