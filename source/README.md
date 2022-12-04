# Sekai

Sekai's source has been split into multiple folders and C# projects (`.csproj`), each representing a module of the engine. Engine components are split into tiers, with engine being the highest abstraction to the core, which is dealing with "low-level" calls.

## Projects
- **Sekai**
    - Implements core functionality.
- **Sekai.Testing**
    - Implements test functionality for NUnit.
- **Sekai.OpenGL**
    - Provides [**OpenGL**](https://github.com/dotnet/Silk.NET) graphics support.
- **Sekai.Forms**
    - Provides [**Windows Forms**](https://github.com/dotnet/winforms) windowing and input support.
    - **Note**: This requires Visual Studio and Windows with the appropriate workloads. See [this article](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/set-up-your-development-environment) for more details on how to set up your environment.
- **Sekai.SDL**
    - Provides [**SDL2**](https://github.com/dotnet/Silk.NET) windowing and input support.
