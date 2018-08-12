// Helper functions

// Maps Screen coordinates [-1, 1] with y is up to texture coordinates [0, 1] with y is down
float2 ToTextureCoordinates(float2 screenCoordinates, float w)
{
	return 0.5f * (float2(screenCoordinates.x / w, -screenCoordinates.y / w) + 1);
}