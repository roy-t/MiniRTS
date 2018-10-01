# Worklist
## TODO

- Clean up rendering pipeline, especially the DeferedRenderer class
- Make rendering effects more of a pipeline, giving each stage a separate class and tweakable order of events
- Move all effects to wrappers

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
