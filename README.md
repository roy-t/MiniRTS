![Screenshot of the editor](docs/_images/logo.jpg "MiniEngine")

# MiniRTS
The name of this repository is MiniRTS, because when I started out I wanted to make an RTS game, inspired by games like Total Annihilation,  Supreme Commander: Forged Alliance, or Age of Empires (1, 2). But I got stuck in making a game engine! Which is what this repository is now focused on.

# Goal
Creating a game engine is a lot of work. So I do not aim to make a competitor to the popular game engines, like Unity, Unreal, Source, Godot, or Lumberyard. Instead I try to make this game engine an educational resource. Both for myself, and anybody else that's interested in how game engines work. There are many websites dedicated to explaining techniques that you find in games. But there are only few that show you how all these techniques can work together inside a game engine. To me this feels like a crucial bit of information that is missing. So I hope to help fill this gap with this project.

I'm trying to fulfill this goal with the source code in this repository and the documentation on https://roy-t.github.io/MiniRTS/. The source code will show you exactly how I've implemented a certain technique. The documentation pages will give you a high level overview of how a technique works. 

To make the goals for this project explicit:

> The primary goal of this repository, and the supplementing documentation, is to learn techniques used in game engines and to teach those to anybody who is interested.

I've defined the following sub-goals to help me accomplish my main goal.

## Sub-goals
> 1. Use established techniques

I do not write about the most cutting edge techniques, but lag behind by approximately 5 years. Techniques that are a few years older are generally better understood and better documented. Older techniques have also shown that they have merits and here to stay. All the techniques used in MiniEngine are still used in the most state-of-the-art game engines so still relevant.

> 2. Value clarity over performance

In commercial game engines code is often optimized to get that last percentage of extra performance. Unfortunately these optimizations often to lead to obfuscation of what that piece of code is trying to accomplish. In this project I implement the same algorithms. But when implementing them I will try to write clear code instead of fast code, when both is not possible.

> 3. Make my life as easy as possible

When I implement a technique I want to focus purely on that technique and worry as little as possible about integrating it into the game engine. In the previous version of MiniEngine I often had to write a lot of boilerplate code to integrate a new technique. This boilerplate code does not contribute to my main or sub-goals so is effectively waste. In the latest version of MiniEngine I use techniques like source-generators and dependency injection to make my life as easy as possible.

# Current state
MiniEngine is an ongoing project, I've got a lot of plans for new features, and at the same time I still have to port a few old features from the old version. 

## What's implemented?
- An Entity/Component/System (ECS) with queries based on an entity's component signature and state (new, unchanged, changed, removed)
- Physically Based Rendering using a linear, HDR compatible, workflow
- Several light primitives: point, spot, image
- Filtered shadows using PCF
- Tone mapping

## What's on the roadmap?
- Start on documentation
- Add all light primitives that are in vOld (directional, sun)
- Cascaded shadow maps (already implemented in vOld)
- FXAA (already implemented in vOld)
- Particles (already implemented in vOld)

# Running MiniEngine

1. Make sure you've installed the following software and have a DirectX 12 compatible GPU.
> - Windows 10 64 bit
> - Visual Studio 2019
> - .Net 5.0

2. Clone the project and its submodules using the following command

```
git clone --recurse-submodules https://github.com/roy-t/MiniRTS.git
```

3. Open the `/src/MiniEngine.vNext.sln` file, set the `MiniEngine.Editor` project as the start-up project, and press run. You should be able to play around. Note that there are some pre-populated scenes that you can view, they are available via the `scene` menu.

# History
This repository actually contains two game engines. In the end of 2017 I started on the first version, which now resides in the `./vOld/` folder. The vOld engine is a standard deferred rendering engine, using a Blinn-Phong rendering model. It contains a lot of different light and particle primitives and the editor is quite feature rich. 

In 2019 I gave a presentation on real-time rendering where I talk about techniques used in this engine. See: https://youtu.be/l9Mx67fCr5I. I see this talk as the 'completion' of the `vOld` engine. After that talk I've not added much engine features, but I started experimenting with different game mechanics. 

In 2020 I realized that adding new features to this engine was becoming quite the burden. Which isn't a surprise if you realize this was my first really big game engine project. So I started a new version which you can find in the `./src/` folder. 

# Standing on the shoulders of giants
MiniEngine uses several libraries. Especially the [MonoGame](https://www.monogame.net/) library allows me to focus on the code that I want to write (and show). MiniEngine also uses a fork of [DearImGui](https://github.com/ocornut/imgui) for its UI, [LightInject](https://www.lightinject.net) for dependency injection and [Serilog](https://www.serilog.net) as a logging abstraction.

While writing MiniEngine I consulted hundreds of blogs to figure out how to implement a technique. Without these blog posts I would have never even attempted to start writing my own game engine. To highlight a few.

- [Riemers.net](https://github.com/simondarksidej/XNAGameStudio/wiki/RiemersArchiveOverview) extensive examples of basic rendering techniques, archived by Simon Jackson.
- [LearnOpenGL](https://learnopengl.com/PBR/Theory), the articles on Physically Based Rendering are extremely well done
- [Tim Jones](http://timjones.io/blog/archive/2015/04/13/cascaded-shadow-maps-sample-for-monogame), especially his article and source code for cascaded shadow maps
- [Bevy](https://bevyengine.org/news/introducing-bevy/) for their focus on being 'the most ergonomic ECS in existence'
- [Coding Adventures](https://www.youtube.com/watch?v=r_It_X7v-1E&list=PLFt_AvWsXl0ehjAfLFsp1PGaatzAwo0uK) great video series by Sebastian Lague
- Ziggyware, ConkerJo, CatalinZima and all the other great blogs about XNA that have are now defunct

I also get a lot of my inspiration from books, like the [Real Time Rendering](http://www.realtimerendering.com/) book and Eric Lengyel's [Foundations of Game Engine Development](http://terathon.com/blog/) series.



