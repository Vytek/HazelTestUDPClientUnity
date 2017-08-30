# HazelTestUDPClientUnity
Unity Test Client Hazel UDP 

This is a Networking Demo based on:

- https://github.com/DarkRiftNetworking/Hazel-Networking (MIT License)
- https://github.com/Vytek/HazelTest
- Stub from: https://forum.unity3d.com/threads/hazel-networking-open-source-rudp-tcp-library.409863/page-2#post-3006994 (Thanks JoeStrout)

Projects used:

- https://google.github.io/flatbuffers/ (Apache License, Version 2.0)
- http://exiin.com/blog/flatbuffers-for-unity-sample-code/
- https://github.com/neuecc/MessagePack-CSharp (MIT License)
- https://github.com/CaptainHillman/UnityTools (MIT Licence)
- https://github.com/HiddenMonk/Unity3DRuntimeTransformGizmo (MIT Licence)
- https://github.com/PimDeWitte/UnityMainThreadDispatcher (Apache License, Version 2.0)
- https://github.com/Deadcows/CustomLogger
- ScreenLogger Unity Assets: https://www.assetstore.unity3d.com/en/#!/content/49114

![Test Example](https://github.com/Vytek/HazelTestUDPClientUnity/blob/master/Images/2017-08-04%2010_29_34.gif)

## How to test

1. Clone this repo: [https://github.com/Vytek/HazelTest](https://github.com/Vytek/HazelTest)
2. Compile project using Monodevelop or Visual Studio 2015.
3. Start "https://github.com/Vytek/HazelTest/tree/master/HazelUDPTestSuperServer". For example: "mono HazelUDPtestSuperServer.exe".
4. Build and run Client One. WARNING: Its a default in Unity to pause the game when it's not in focus, you can change this in Edit -> Project Settings -> Player -> Resolution and Presentation -> Run In Background (see: https://forum.unity3d.com/threads/darkrift-fast-and-flexible-cross-platform-networking.320185/)
5. Run Unity Editor Project for Client Two.
6. Move cube on screen using runtime gizmo.

## Pull requests

 1. [Fork] the project, clone your fork, and configure the remotes.
 2. Create a new topic branch (from `master`) to contain your feature,
 chore, or fix.
 3. Commit your changes in logical units.
 4. Push your topic branch up to your fork.
 5. [Open a Pull Request] with a clear title and description.
