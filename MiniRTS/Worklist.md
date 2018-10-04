# Worklist
## TODO

- Particles need a lot more configurable parameters to look good
    - Add a tint color to the shader so we can fade out almost dead particles!
- Move all effects to wrappers
- The code for the sunlight effect is still too complicated for what it does, especially the C# code is a mess.

## Interesting

- Particle effects
- Screen space ambient occlusion
- Reflections
- Water
- Terrain/tree/water generation


## Possible Improvements

- The FXAA algorithm could be improved, the sample distance is currently only goverened by the dot product of the UVs. I also tried perceived brightness differences, but that gave textures an ugly blur.

## Known Issues

- Sometimes transparency effects from the sunlight disappear when zooming in. Possibly due to the camera for that cascade not seeing the object anymore, even though backface culling and z-culling are disabled. This can usually be prevented by tweaking the cascade distances.

