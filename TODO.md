# Worklist
## TODO

### Misc
- Make it cleaner to select or deselect render pipeline features
- Make it possible to tweak parameters used by components (like shadowmap resolution) at runtime


### Particles
- Particles need a lot more configurable parameters to look gooed
- Move Easings.cs to a separate projects

### Models
- Clean up code
- Make batch processing work like particles

### Projectors
- Maybe we can also project onto the Normal target to get interesting light effects from decals?
- Use the normal map as input to only cast onto surfaces facing the same way as the projector?
- Move ColorMap effect to projector system for both models and textures, removing the costs from shadow casting lights and sunlights
  only projecting colors when we want to. 

### General
- Find bottlenecks and optimize
- Find allocations and kill them

## Interesting
- Skybox: http://rbwhitaker.wikidot.com/skyboxes-1
- Water
- Terrain/tree/water generation

## Possible Improvements
- The MiniEngine.Primitives project should be organized better as there is no coherence between classes in that project
- The FXAA algorithm could be improved, the sample distance is currently only goverened by the dot product of the UVs. I also tried perceived brightness differences, but that gave textures an ugly blur.
- A different sampling strategy for normal maps could reduce bright edges
- Treating diffuse textures as linear color space will improve the visual effect of a lot of algorithms
- The hierarchies of IViewPoint is a mess, figure out what kind of camera's we really need! 
    - ShadowCamera is special, but needs to be able to follow the normal pipeline for models (just like now)
    - Perspective Camera is almost always what we want
    - Orthographic Camera is never used, but it would be a cool effect, right now it is just not pluggable enough
    - In a lot of cases our vertex shader computes some camera matrices that we already have

## Known Issues
- Sometimes transparency effects from the sunlight disappear when zooming in. Possibly due to the camera for that cascade not seeing the object anymore, even though backface culling and z-culling are disabled. This can usually be prevented by tweaking the cascade distances.
- SSAO: tiling effect for flat surfaces that have no normal map