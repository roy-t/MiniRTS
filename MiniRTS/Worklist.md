# Worklist
## TODO
- Add icons for where a projector/light/... is

### Misc
- Allow scenes to share code or inherit
- Make it cleaner to select or deselect render pipeline features
- Make it possible to tweak parameters used by components (like shadowmap resolution) at runtime
- Move all storage of components closer to their creators and give the EntityLinker hooks to get the info it needs
- Move render states to effects since they are usually tightly coupled?

### Particles
- Make particles work again with shadows
- Particles need a lot more configurable parameters to look gooed
- Move Easings.cs to a separate projects

### Models
- Clean up code
- Make batch processing work like particles

### Projectors
- Maybe we can also project onto the Normal target to get interesting light effects from decals?
- Move ColorMap effect to projector system for both models and textures, removing the costs from shadow casting lights and sunlights
  only projecting colors when we want to

### General
- Find bottlenecks and optimize
- Add outlines for cameras, and lights (that often use cameras internally).

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
- Projectors also project on the back of objects, maybe check that via the normal map so it wont happen? Not a real problem for now

## Known Issues in dependencies
- The IMGUI.net NuGet package does not automatically copy cimgui.dll to the output directory on build. See https://github.com/mellinoe/ImGui.NET/issues/83 because of this I've added it the the UI project (as a link). But this might break when updating the library! Or when redistributing the engine
