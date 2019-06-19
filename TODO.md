# Worklist
## TODO

### Misc
- Make it cleaner to select or deselect render pipeline features
- Make it possible to tweak parameters used by components (like shadowmap resolution) at runtime
- Move all storage of components closer to their creators and give the EntityLinker hooks to get the info it needs
- Don't make factories responsible for deconstructing as they can now only deconstruct one type while we have a lot of dual types (additive/averaged particles, opaque/transparent models)

### Particles
- Make particles work again with shadows
- Particles need a lot more configurable parameters to look gooed
- Move Easings.cs to a separate projects

### Models
- Clean up code
- Make batch processing work like particles

### Projectors
- Maybe we can also project onto the Normal target to get interesting light effects from decals?
- Use the normal map as input to only cast onto surfaces facing the same way as the projector?
- Move ColorMap effect to projector system for both models and textures, removing the costs from shadow casting lights and sunlights
  only projecting colors when we want to. Maybe we can use this to make particles shadowy again?

### General
- Find bottlenecks and optimize
- Find allocations and kill them

## Interesting
- Reflections
- Water
- Terrain/tree/water generation

## Possible Improvements

- The MiniEngine.Primitives project should be organized better as there is no coherence between classes in that project
- The FXAA algorithm could be improved, the sample distance is currently only goverened by the dot product of the UVs. I also tried perceived brightness differences, but that gave textures an ugly blur.
- A different sampling strategy for normal maps could reduce bright edges
- Treating diffuse textures as linear color space will improve the visual effect of a lot of algorithms

## Known Issues
- Sometimes transparency effects from the sunlight disappear when zooming in. Possibly due to the camera for that cascade not seeing the object anymore, even though backface culling and z-culling are disabled. This can usually be prevented by tweaking the cascade distances.
- SSAO: tiling effect for flat surfaces that have no normal map