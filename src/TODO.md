# TODO

- Unify geometry generation, let users pass in a vertex declaration and get the vertex and index buffer?

- Sunlight and Spotlight both use a shadow map, but the sampling is subtely different because one uses an array and the other not, can we unify this?

- Create a better editor tool for angles/yaw-pitch-roll

- Real paritioning/culling: https://www.slideshare.net/DICEStudio/culling-the-battlefield-data-oriented-design-in-practice

# Ideas

Make a scene with many asteroids (like 10 variations instanced in different orientations). Put some sort of 'fog' in between to simulate space dust. Take the thickness from this fog from a RT where I draw over black swatches when a rocket goes over it. Add sun rays?

# Participating Media
- Things to try
- Maybe try a true fog formula instead of a lerp
- Add depth/normal input to weigh bilinear upscale in ParticipatingMediaPostProcessEffect.fx