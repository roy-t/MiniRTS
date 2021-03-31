# TODO

- Unify geometry generation, let users pass in a vertex declaration and get the vertex and index buffer?

- Sunlight and Spotlight both use a shadow map, but the sampling is subtely different because one uses an array and the other not, can we unify this?

- Create a better editor tool for angles/yaw-pitch-roll

- Real paritioning/culling: https://www.slideshare.net/DICEStudio/culling-the-battlefield-data-oriented-design-in-practice

- https://github.com/prime31/Nez/blob/master/Nez.Portable/Utils/Tweens/Easing/Easing.cs ?


# Components
Are often also useful without an entity (for mathy stuff / reuse)
- What if we seperate them completely and just have a Component<T> type? or auto generate them?


# Participating Media
- Right now if a part of the media is in the shadow it is as if the media is less dense. This simulates dust particles in the air.
However for fog it would be better if this should just make the fog darker. Try how that looks?

# Particles
- Make shadows optional, per emitter
- The ParticleRenderer and GeometryRenderer are very much alike. But I don't like how the GeometryRenderer is tied to
poses now. Could it get the data from the spatial partitioner directly instead of via a property in camera? Alternatively a
pose could just be any visible entity and the renderers get the types they can render from it?

- Add a physics component that computes velocity and acceleration like in vOld
- Use the physics component to give particles an initial velocity via a new texture that is only updated when age is approx 0