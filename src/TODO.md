# TODO

- Unify geometry generation, let users pass in a vertex declaration and get the vertex and index buffer?

- Sunlight and Spotlight both use a shadow map, but the sampling is subtely different because one uses an array and the other not, can we unify this?

- Real paritioning/culling: https://www.slideshare.net/DICEStudio/culling-the-battlefield-data-oriented-design-in-practice

- https://github.com/prime31/Nez/blob/master/Nez.Portable/Utils/Tweens/Easing/Easing.cs ?

- The state of a component only changes when the GUI does something with it. Easy to forget to update.. how to fix?

# Participating Media
- Right now if a part of the media is in the shadow it is as if the media is less dense. This simulates dust particles in the air.
However for fog it would be better if this should just make the fog darker. Try how that looks?

# Particles
- Greatly improve performance by using compute shaders? Also tricks to improve for   https://github.com/m-schuetz/compute_rasterizer
- Make shadows optional, per emitter?