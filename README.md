# HazelTestUDPClientUnity
Unity Test Client Hazel UDP 

This is a Networking Demo based on:

- https://github.com/DarkRiftNetworking/Hazel-Networking
- https://github.com/Vytek/HazelTest

Projects used:

- https://google.github.io/flatbuffers/
- https://github.com/neuecc/MessagePack-CSharp
- https://github.com/CaptainHillman/UnityTools
- https://github.com/PimDeWitte/UnityMainThreadDispatcher
- https://github.com/Deadcows/CustomLogger
- ScreenLogger Unity Assets: https://www.assetstore.unity3d.com/en/#!/content/49114

![Test Example](https://github.com/Vytek/HazelTestUDPClientUnity/blob/master/Images/2017-08-04%2010_29_34.gif)

## How to test

1. Clone this repo: [https://github.com/Vytek/HazelTest](https://github.com/Vytek/HazelTest)
2. Compile project using Monodevelop or Visual Studio 2015.
3. Start "https://github.com/Vytek/HazelTest/tree/master/HazelUDPTestSuperServer". For example: "mono HazelUDPtestSuperServer".
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