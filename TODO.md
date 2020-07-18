Update to monogame 3.8: https://community.monogame.net/t/monogame-3-8-prerelease-packages-are-up-on-nuget/12708/15 get rid of installer

# WIP
- Textures for models need more margin between island to avoid bleeding. Exhaust misses inside of the pipe
- Remove code that we're not going to use (especially in GameLogic project) and clean up code that we want to keep!

- Use network serialization from: https://revenantx.github.io/LiteNetLib/articles/netserializerusage.html
- If the game is running at <60fps the draw loop gets called less, but the elasped parameter is still 1/60s so the accelerometer (among other things) will freak out.
- The hierarchy (Parent component) works great in the Entity Window, however we don't do anything with it in the EntityMenu

- There are still a few components that use a position. However they usually use it in different way, should they use Pose? -> Yes because then we can use offset for these things
    - ShadowCastingLight
    - Sunlight
    - Waypoint    
    - Cameras??
- Replace icon system with a system that prints the number of the entity! Then remove all icons and clean up debug systems
- The split between opaque and transparent models makes it hard for other systems to reference all kinds (see UVAnimationSystem)
- The split between additive and transparent particles makes it hard for other systems to reference all kinds (see ReactionControlSystem)

# Future
## TODO
Threading:
- Make a ThreadSafe IComponentCollection that overrides the add and remove methods and stores the component to add or remove in a seperate list and syncs with the main thread once every frame, on that sync it allows the stuff to be added or removed. In this way  most threading stuff will work out OK as long as things are not too tightly coupled?

Multiplayer
- Make all stuff that is shared uniquely identifyable (UnitId?), send commands for a given tick (Like Move Unit 4 at Tick Now + 10). 
Have every client wait on each tick. Handle 1 command per tick per player, always handle player 1 first, then 2, ... Store commands in a list
so someone can try to reload and catch up if they desync/disconnect. Periodic state hash checks

- GameServer, hosted somewhere, should store public IP of every server and on what port they are listening? (How to communicate correct port 
if people are behind NAT?). Make game p2p?

- What if we create a server (that's either directly addressable locally, or via tcp/udp remotely) that manages all state changes
    - making the games simple viewers
    - how would that work with lock-step?
    - How can we even make sure the game stays speedy even if one computer lags
    - Check how factorio does it?

- What kind of path finding do we want? 
    - Grid
    - Graph
    - Reserve roads, try to go around when a road block is locked for a long time (keep track of seconds semi-locked per edge?)

- Job/Threading system for tasks that take more than one frame
    - Systems that are receive the processed output should have a thread-safe queue or something to process incoming events?

### Misc
- Make it cleaner to select or deselect render pipeline features
- Make it possible to tweak parameters used by components (like shadowmap resolution) at runtime

### Particles
- Particles need a lot more configurable parameters to look good
- Move Easings.cs to a separate projects

### Models
- Clean up code
- Make batch processing work like particles

### Projectors
- Maybe we can also project onto the Normal target to get interesting light effects from decals?
- Use the normal map as input to only cast onto surfaces facing the same way as the projector?
- Move ColorMap effect to projector system for both models and textures, removing the costs from shadow casting lights and sunlights
  only projecting colors when we want to. 

### General
- Find bottlenecks and optimize
- Find allocations and kill them

## Interesting
- Skybox: http://rbwhitaker.wikidot.com/skyboxes-1
- Water
- Terrain/tree/water generation, especially sand dunes/desert

## Possible Improvements
- The MiniEngine.Primitives project should be organized better as there is no coherence between classes in that project
- The FXAA algorithm could be improved, the sample distance is currently only goverened by the dot product of the UVs. I also tried perceived brightness differences, but that gave textures an ugly blur.
- A different sampling strategy for normal maps could reduce bright edges
- Treating diffuse textures as linear color space will improve the visual effect of a lot of algorithms
- The hierarchies of IViewPoint is a mess, figure out what kind of camera's we really need! 
    - ShadowCamera is special, but needs to be able to follow the normal pipeline for models (just like now)
    - Perspective Camera is almost always what we want
    - Orthographic Camera is never used, but it would be a cool effect, right now it is just not pluggable enough
    - In a lot of cases our vertex shader computes some camera matrices that we already have

## Known Issues
- All transparent models need to be rendered always to make sure that sunlights/directional lights projections work even when the model itself is not on screen
- SSAO: tiling/shimmiring/banding effect for flat surfaces visibile in light/blur render target