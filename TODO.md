# Worklist
## TODO
Threading:
- Make a ThreadSafe IComponentCollection that overrides the add and remove methods and stores the component to add or remove in a seperate list and syncs with the main thread once every frame, on that sync it allows the stuff to be added or removed. In this way  most threading stuff will work out OK as long as things are not too tightly coupled?


- What if we create a server (that's either directly addressable locally, or via tcp/udp remotely) that manages all state changes
    - making the games simple viewers
    - how would that work with lock-step?
    - How can we even make sure the game stays speedy even if one computer lags
    - Check how factorio does it?

- What kind of path finding do we want? 
    - Grid
    - Graph
    - Reserve roads, try to go around when a road block is locked for a long time (keep track of seconds semi-locked per edge?)

- Job/Threading system for tasks that take more than one frame
    - Systems that are receive the processed output should have a thread-safe queue or something to process incoming events?

### Misc
- Make it cleaner to select or deselect render pipeline features
- Make it possible to tweak parameters used by components (like shadowmap resolution) at runtime


### Particles
- Particles need a lot more configurable parameters to look good
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
- SSAO: tiling/shimmiring/banding effect for flat surfaces visibile in light/blur render target