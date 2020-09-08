# MiniRTS
The name of this repository is MiniRTS, because when I started out I wanted to make an RTS game, inspired by games like Total Annihilation,  Supreme Commander: Forged Alliance, or Age of Empires (1, 2). But I got stuck in making a game engine! Which is what this repository is now focussed on.

# MiniEngine

MiniEngine is a work-in-progress rendering engine that I'm using as a personal research project. Using MiniEngine I explore common rendering techniques. My main interest here is how those techniques are combined, as many papers, examples, and tutorials showcase techniques in isolation, and it is not always obvious how multiple techniques should work together.

This contents of this repository is constantly subject to change, and I would not recommend using MiniEngine as an engine for your own projects. However, I do think my work here can be useful of you're trying to figure out how a technique works. MiniEngine currently includes the following effects.

In 2019 I gave a presentation on real-time rendering where I talk about techniques used in this engine. See: https://youtu.be/l9Mx67fCr5I. 

# General Architecture

MiniEngine is built around a rendering technique called [deferred shading](https://en.wikipedia.org/wiki/Deferred_shading), also called deferred rendering. See `GBuffer.cs` for a description of the buffers in use. Transparency is support and is achieved by rendering multiple passes. One pass for all opaque objects and then one pass for each group of non-overlapping transparent objects. This means that even multiple overlapping semi-transparent objects are supported. Using deferred shading means that all effects mentioned below are only computed in screen space, and are, at most, computed for every pixel on screen

MiniEngine also uses a pipelined approach, where every type of effect is a step in the pipeline towards the final image. This pipeline is combined with an [Entity-Component System](https://en.wikipedia.org/wiki/Entity_component_system). Any object in the game is an entity, which has no properties. Factories are used to assign components to an entity. And system are used to process all components of a single type, being completely ignorant about what entity they belong to. This keeps every system, and component, extremely simple. And avoids large and complicated hierarchies.

# Effects

## Lights
All lights use a slightly modified version of the [Phong reflection model](https://en.wikipedia.org/wiki/Phong_reflection_model). Models are contain a diffuse, normal, and specular texture, which is taken into account in the Phong shading model. 

### Ambient light
Both a fixed ambient light and [Screen Space Ambient Occlusion](https://en.wikipedia.org/wiki/Screen_space_ambient_occlusion) are supported. the SSAO implementation uses [Simplex Noise](https://en.wikipedia.org/wiki/Simplex_noise) to reduce artefacts.

### Directional light
Light coming from one direction infinitely far away.

### Point light
Point light source with attenuation.

### Shadow casting light
A spotlight that uses [shadow mapping](https://en.wikipedia.org/wiki/Shadow_mapping) to cast shadows. The shadow maps are filtered using [Percentage Closer Filtering](https://developer.nvidia.com/gpugems/GPUGems/gpugems_ch11.html).

### Sunlight
A directional light source infinitely far away that uses [Cascaded Shadow Mapping](https://developer.download.nvidia.com/SDK/10.5/opengl/src/cascaded_shadow_maps/doc/cascaded_shadow_maps.pdf) to cast shadows. The CSMs are filtered using PCF.

## Particles
Both additive and transparent particles are supported. All transparent particles are rendered in one pass (with two steps) using [Weighted, Blended Order-Independent Transparency](http://casual-effects.blogspot.com/2015/03/implemented-weighted-blended-order.html).

## Projectors
Projects support projecting textures onto the environment. You can even use a texture that is generated everyframe using a separate render pipeline, to create portal like effects.

# Requirements
- Windows 10 64 bit
- Visual Studio 2019
- .Net Core 3.1
- MonoGame 3.8.0.1469-develop ([see](http://www.monogame.net/downloads/))

Install the monogame nuget source and the monogame content pipeline tools
```
dotnet nuget add source http://teamcity.monogame.net/guestAuth/app/nuget/feed/_Root/default/v3/index.json --name monogame-develop
dotnet tool update -g dotnet-mgcb --version 3.8.0.1469-develop
dotnet tool update -g dotnet-mgcb-editor --version 3.8.0.1469-develop
```

Simply open the `.sln` file, compile and run the `Editor` project and you should be able to play around. Note that there are some pre-populated scenes that you can view, they are available via the `file` menu.
