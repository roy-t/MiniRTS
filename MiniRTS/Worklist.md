# Worklist
## TODO


- The RenderPipeline was changed for a few pipeline projects change them for all (see ModelRenderPipeline for samples)!  

- Particles need a lot more configurable parameters to look good
    - Add a tint color to the shader so we can fade out almost dead particles!
- The code for the sunlight effect is still too complicated for what it does, can I partition the code differently?

## Interesting

- Particle effects via a new method that weighs by depth so we can do it in 1 pass
- Screen space ambient occlusion
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
- Particles are rendered last in the shadow map state, might 'shadow' on a stained glass window that is in front of it.
- Shadowey particles appear on the roof (-z culling?)
- Particles are really expensive, as they are drawn for every shadow casting light, and every cascade. Let's first try to add spatial partitioning to reduce the draws, then make the draw calls more efficient

