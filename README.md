# FlexSharpUnity
An implementation of Nvidia Flex in Unity!
![space, with fluid](https://github.com/coolcatcoder/FlexSharpUnity/blob/main/Images(github)/space_scene.png)

## Intro:
This library should allow easy developement in Unity using NvFlex in both dots and non-dots environments! (dots currently not done yet)

## Current State:
Currently this library is early in development, so far this is what has been done:
- fluids
- solver parameters
- basic shape collisions (cube and sphere, no mesh support currently)
- infinite planes (occasionally causes crashes though)
- basic rendering (can render as spheres to a unity particle system currently)

## How To Use:

For the most basic example all you need is a flex container with planes setup for your particles to collide with, and then an emitter to emit fluid particles!
I would highly recommend downloading the official NvFlex demo app and choosing the solver parameters based off of scenes from there (also check their manual, it is very helpful).

## Screenshots of Demo Scenes:

![rainbow pond](https://github.com/coolcatcoder/FlexSharpUnity/blob/main/Images(github)/pond_scene_rainbow.png)
