# MiniRTS
The name of this repository is MiniRTS, because when I started out I wanted to make an RTS game, inspired by games like Total Annihilation,  Supreme Commander: Forged Alliance, or Age of Empires (1, 2). But I got stuck in making a game engine! Which is what this repository is now focused on.

# MiniEngine

This repository actually contains two (work-in-progress) rendering engines. In the end of 2017 I started on the first version, which now resides in the `./vOld/` folder. The vOld engine is a standard deferred rendering engine, using a Blinn-Phong rendering model. It contains a lot of different light and particle primitives and the editor is quite feature rich. 

In 2019 I gave a presentation on real-time rendering where I talk about techniques used in this engine. See: https://youtu.be/l9Mx67fCr5I. I see this talk as the 'completion' of the `vOld` engine. After that talk I've not added much engine features, but I started experimenting with different game mechanics. 

 In 2020 I realized that adding new features to this engine was becoming quite the burden. Which isn't a surprise if you realize this was my first really big game engine project. So I started a new version, the current version, which lives in the `./src/` in September 2020. With the following goals:

- Adding a new system should be no more work than writing that system
- Multi-threaded core (but single threaded rendering)
- A proper physically based rendering model
- A correct approach to gamma (linear work-flow)
- HDR ready

With a few light primitives already there it seems that the I'm able accomplish these goals. And though the editor still lacks a lot of features the basics for something great are there in the engine. 

![Screenshot of the editor](docs/_images/window.png "MiniEngine")

# Requirements
- Windows 10 64 bit
- Visual Studio 2019
- .Net 5.0

Simply open the `.sln` file, compile and run the `MiniEngine.Editor` project and you should be able to play around. Note that there are some pre-populated scenes that you can view, they are available via the `scene` menu.

# Technologies

MiniRTS uses: 
- [MonoGame](https://www.monogame.net/)
