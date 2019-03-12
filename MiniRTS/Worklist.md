# Worklist
## TODO

### Misc
- Allow scenes to share code or inherit
- Make it cleaner to select or deselect render pipeline features
- Make it possible to tweak parameters used by components (like shadowmap resolution) at runtime

### Particles
- Split emitters into transparent and additive emitters, add additive blending
- Make particles work again with shadows
- Particles need a lot more configurable parameters to look gooed
- Move Easings.cs to a separate projects
- Figure out if we can use a geometry shader and just send points to the GPU

### Models
- Clean up code
- Make batch processing work like particles

### UI
- Separate UI code into parts, have this reflect in the UI rendering and serialization code
- Clean-up all code that generates UI sliders
- Give components more control over the kind of UI controls they get (infinite sliders, ranges.. etc..)

### General
- Find bottlenecks and optimize

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

## Known Issues in dependencies
- The IMGUI.net NuGet package does not automatically copy cimgui.dll to the output directory on build. See https://github.com/mellinoe/ImGui.NET/issues/83 because of this I've added it the the UI project (as a link). But this might break when updating the library! Or when redistributing the engine
