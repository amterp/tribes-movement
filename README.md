# Tribes Movement Demo

A little project/demo replicating the movement from the Tribes games.

Includes:
- Jetpacking.
- Skiing.
- Propelling yourself by shooting explosive projectiles at the ground behind you (to be implemented).
- Terrain generation.

You just sorta fly and ski around at high velocities!

## Terrain Generation

The terrain generation is a modified version of what Sebastian Lague implements in his "Landmass Generation" tutorial series - modified in ways I think makes for slightly cleaner designs and implementations!

It works via generating multiple layers (aka "octaves") of Perlin Noise and sampling a map of those noise values to determine the heights for a terrain. You can see such a noise map below in black and white - black gets interpreted as lower altitude, while white gets interpreted as high altitude.

We then apply colors based on this height map, making lowest altitude areas be oceans or lakes, and highest altitude areas be the peaks of mountains.

Lastly, we generate a mesh from the height map, where the height of each vertex corresponds to the value sampled at its location from the height map, in addition so some additional height mapping to make natural-looking terrain.

The terrain generates infinitely in chunks. This has been multithreaded for increased performance, and we also have implemented automatic level of detail models for the meshes based on the chunks' distance from the player.

We manually calculate mesh normals in orders to ensure smooth lighting between chunks.

## Tribes-style Character Controller

Since the terrain generates a mesh, Unity makes it quite easy to generate a collision mesh at the same time. This allows us to implement a Rigidbody character controller for the player to fly around like they would in Tribes! The skiing has also been implemented and works by raycasting downwards from the player. If we hit the terrain, we take the terrain normal we're given and derive a vector downwards parallel to the slope of the terrain, scaling correctly with gravity in accordance with the laws of physics.

## Media

### Terrain Generation

Raw noise:

<img src="./media/2d-terrain-noise.png" width="500">

Colored in:

<img src="./media/2d-terrain-color.png" width="500">

Gif Demo:

<img src="./media/2d-terrain.gif" width="500">

### Tribes-style Character Controller

GIF from the 2021-10-20 build:

<img src="./media/tribes-demo.gif" width="500">
