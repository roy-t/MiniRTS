# TODO

- The Geometry, class PostProcessTriangle class, and SkyboxGeometry class, and how their draw call looks is extremely similar
    - How to make a class that unifies this, but at the same time doesn't confuse the vertex declaration?
    - Speed :D

- Unify geometry generation, let users pass in a vertex declaration and get the vertex and index buffer?

- Create a better multi-threaded render pipeline. Maybe have one worker thread that pushes an entire pipeline to a queue, and then have the workers handling those systems. Uses a semaphore or something to sync? 

- Sunlight and Spotlight both use a shadow map, but the sampling is subtely different because one uses an array and the other not, can we unify this?



# Gotchas
- If you inject a IEnumerable<IComponentContainer> you get everything 2 or 3 times because of how the injector 
registers all those types as ComponentContainer<>, IComponentContainer<> and IComponentContainer. 
If the class that gets this injected  is unware of this they might not work well. Fix it with a variance filter? How?
See: https://www.lightinject.net/#ienumerablet