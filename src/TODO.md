# TODO

- The Geometry, class PostProcessTriangle class, and SkyboxGeometry class, and how their draw call looks is extremely similar
    - How to make a class that unifies this, but at the same time doesn't confuse the vertex declaration?
    - Speed :D

- Unify geometry generation, let users pass in a vertex declaration and get the components back, or maybe just the vertex and index buffer?



# Bug
- Crash when switching scenes multiple times! Either some component doesn't get removed or 
something gets disposed too soo, rething scene loading/unloading!
