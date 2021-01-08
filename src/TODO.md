# TODO

- Unify geometry generation, let users pass in a vertex declaration and get the vertex and index buffer?

- Create a better multi-threaded render pipeline. Maybe have one worker thread that pushes an entire pipeline to a queue, and then have the workers handling those systems. Uses a semaphore or something to sync? 

- Sunlight and Spotlight both use a shadow map, but the sampling is subtely different because one uses an array and the other not, can we unify this?

- Rework UI, make editors for every primitive, store in a file the path to the primitive (so component.property.value....) and the range it should have. If an unknown property comes up try to find the closest match

- Use ImGui.PushID("A"); and PopID to be able to use shorter names for labels without imgui thinking its the same thing!

# Ideas

Make a scene with many asteroids (like 10 variations instanced in different orientations). Put some sort of 'fog' in between to simulate space dust. Take the thickness from this fog from a RT where I draw over black swatches when a rocket goes over it. Add sun rays?

# Participating Media
- Things to try
- Get rid of PCF -> without affecting looks?
- Maybe try a true fog formula instead of a lerp
- Add depth/normal input to weigh bilinear upscale in ParticipatingMediaPostProcessEffect.fx
- Experiment with sample count