# Sekai Sources

Sekai's source has been split into multiple folders and C# projects (.csproj), each representing a module of the engine. Engine components are split into tiers, with engine being the highest abstraction to the framework, which is dealing with "low-level" calls (with the exception of `extensions`, which are middleware for the engine, and usually reside in the same level as the framework).

## Folder and Projects Layout

Generally, the folder structure is as follows:

### framework
- **Sekai.Framework**
    - Implements core functionality.
- **Sekai.Framework.Testing**
    - Implements testing functionality.

### extensions
- **Sekai.Dummy**
    - Provides dummy windowing and graphics support.
- **Sekai.OpenAL**
    - Provides [**OpenAL**](https://github.com/dotnet/Silk.NET) audio support.
- **Sekai.OpenGL**
    - Provides [**OpenGL**](https://github.com/dotnet/Silk.NET) graphics support.
- **Sekai.SDL**
    - Provides [**SDL 2**](https://www.libsdl.org/) windowing and input support.
