// Helper functions

// Maps Screen coordinates [-1, 1] with y is up to texture coordinates [0, 1] with y is down
float2 ToTextureCoordinates(float2 screenCoordinates)
{
	return 0.5f * (float2(screenCoordinates.x, -screenCoordinates.y) + 1);
}