# DopaVoxel

This Github repository contains the source code for a 3D engine that displays a world made up of cubes, similar to Minecraft. I developed this project in 2014 using C# and the Monogame library.

The world in this engine is infinite and is dynamically generated as the player moves through it. Unlike the original Minecraft game, the terrain is not generated using Perlin noise, but rather by combining different height maps.

The world is divided into chunks of 16x16x256 blocks, which are generated in a separate thread. I attempted to implement a lighting system, but the subject proved to be more complex than I anticipated.

Overall, I am fairly satisfied with the results given the amount of time I spent on the project.

![DopaVoxel](DopaVoxel.jpg?raw=true)
