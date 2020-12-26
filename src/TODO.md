# TODO

- Unify geometry generation, let users pass in a vertex declaration and get the vertex and index buffer?

- Create a better multi-threaded render pipeline. Maybe have one worker thread that pushes an entire pipeline to a queue, and then have the workers handling those systems. Uses a semaphore or something to sync? 

- Sunlight and Spotlight both use a shadow map, but the sampling is subtely different because one uses an array and the other not, can we unify this?

# Idea

Make a scene with many asteroids (like 10 variations instanced in different orientations). Put some sort of 'fog' in between to simulate space dust. Take the thickness from this fog from a RT where I draw over black swatches when a rocket goes over it. At sun rays?


- Fog/Dust, Ravendarke at Monogame discord: https://www.slideshare.net/BenjaminGlatzel/volumetric-lighting-for-many-lights-in-lords-of-the-fallen


# WIP - Fog
- Can we add some scattering/*bloom*? <---- BLOOM might actually fake ray scattering a bit?

- Fog added 2 full screen render targets to the LBuffer, can we optimize this
- How can we give more parameters to tweak the fog?
- How can we have different parameters for different volumes (do we need that) at the same time..?
- Right now it only supports concave geometry as a volume?


- Any scene without a sunlight wont render all the way now OOPS :P

- Performance
    - Get rid of PCF -> without affecting looks?
    - Sample at lower resolution, then bilateral upscale    
       
- Artefacts
    - Dithering -> starting at slightly different offsets seems to mess with self shadowing
    - More samples
    - Blur/upscale?

- Weirdness
    - Although the shadowmap only applies darkness to the fog colour it still doesn't incorporate ambient light
    making the fog much darker than it should be. Maybe calculate the real ambient light at that position, 
    (could even colour the fog, since fog gets light from 360 degrees its one value for everything? 
    AKA average colour * strength of lightmap?)


- Things to try
    - Maybe try a true fog formula instead of a lerp
    - Make it possible to change the strength and colour