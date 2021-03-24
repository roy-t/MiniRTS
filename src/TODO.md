# TODO

- Unify geometry generation, let users pass in a vertex declaration and get the vertex and index buffer?

- Sunlight and Spotlight both use a shadow map, but the sampling is subtely different because one uses an array and the other not, can we unify this?

- Create a better editor tool for angles/yaw-pitch-roll

- Real paritioning/culling: https://www.slideshare.net/DICEStudio/culling-the-battlefield-data-oriented-design-in-practice

- https://github.com/prime31/Nez/blob/master/Nez.Portable/Utils/Tweens/Easing/Easing.cs ?



# Ideas

Make a scene with many asteroids (like 10 variations instanced in different orientations). Put some sort of 'fog' in between to simulate space dust. Take the thickness from this fog from a RT where I draw over black swatches when a rocket goes over it. Add sun rays?

# Participating Media
- Things to try
- Add depth/normal input to weigh bilinear upscale in ParticipatingMediaPostProcessEffect.fx
- Right now if a part of the media is in the shadow it is as if the media is less dense. This simulates dust particles in the air.
However for fog it would be better if this should just make the fog darker. Try how that looks?
- I never got upsamping to work in such a way that it takes more of the left or right pixel when its not in the center

# Particles
- Make shadows optional, per emitter
- The ParticleRenderer and GeometryRenderer are very much alike. But I don't like how the GeometryRenderer is tied to
poses now. Could it get the data from the spatial partitioner directly instead of via a property in camera? Alternatively a
pose could just be any visible entity and the renderers get the types they can render from it?