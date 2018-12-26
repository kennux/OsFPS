# OsFPS
# What is this?

Open-source first person shooter engine for Unity3D.
Originally developed as game prototype, later on stripped of propreitary assets and refactored to use UnityTK.

This project is targeted at people who can read and write code, its not a game-ready drag&drop FPS engine (nor is there any end-user documentation)!
If you are looking for an easy to use drag&drop fps system i highly recommend the great uFPS unity asset.

Link: https://assetstore.unity.com/packages/templates/systems/ufps-ultimate-fps-2943

# How to use

You can use OsFPS as git submodule:

`git submodule init`  
`git submodule add https://github.com/kennux/OsFPS`

In order to sync the OsFPS submodule code to your unity project the bash script in Utility/update_osfsp.sh can be used.
OsFPS _REQUIRES_ the UnityTK library which can be used in a similar way.

You can find it here: https://github.com/kennux/UnityTK

However it is highly recommended to use the OsFPS unity project settings.
You can just clone the repository, copy the project, use it as your project and then use the submodule for pulling updates.

# What can i do with it?

Whatever you want essentially!
Use it for prototyping, fork it and contribute to development, use it for education.

It is up to you :>

# Features

- First person camera controller
- Simple inventory system
- Guns!
- Procedural animation system (for recoil, external forces, movement, idle, ....)
- Damage handling (Entity damage and physical forces)
- Interaction system with interactable objects
- A lot other small features

## Todo

There is quite a lot to do, there is room for lots of cool features!
I think the most important todos are:

- Melee weapons
- 3rd Person view
- AI Controllers
- Networking

# License

This project is released under MIT-License: https://tldrlegal.com/license/mit-license
