# TODO

- The Geometry, class PostProcessTriangle class, and SkyboxGeometry class, and how their draw call looks is extremely similar
    - How to make a class that unifies this, but at the same time doesn't confuse the vertex declaration?
    - Speed :D

- Unify geometry generation, let users pass in a vertex declaration and get the vertex and index buffer?

- Create a better multi-threaded render pipeline. Maybe have one worker thread that pushes an entire pipeline to a queue, and then have the workers handling those systems. Uses a semaphore or something to sync? 
