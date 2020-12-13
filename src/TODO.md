# TODO

- The Geometry, class PostProcessTriangle class, and SkyboxGeometry class, and how their draw call looks is extremely similar
    - How to make a class that unifies this, but at the same time doesn't confuse the vertex declaration?
    - Speed :D

- I need a name for mesh data that doesn't clash with MonoGame, geometry is too broad

- PointLight and Spotlight shader share 90% of their code, refactor!
- PointLight uses a full screen triangle, but an actual frustum would work much better. Check the irradiance function to see how large it should be! (Or check the alternative irradiance function from learnopengl)
    - See how this was done for SpotLights and replicate the effect

- Replace 'Diffuse' with 'Albedo'

# Wishlist

- Unify geometry generation, let users pass in a vertex declaration and get the components back? (Too early?)
- Write a document explaining design decisions and how everything works? Possibly on GitHub pages?
