# Sekai Sources

Sekai's source has been split into multiple folders and C# projects (.csproj), each representing a module of the engine. Engine components are split into tiers, with engine being the highest abstraction to the framework, which is dealing with "low-level" calls (with the exception of `extensions`, which are middleware for the engine, and usually reside in the same level as the framework).

## Folder and Projects Layout

Generally, the folder structure is as follows:

### engine
- **Sekai.Engine**
    - Core game engine functionality.
- **Sekai.Engine.Resources**
    - Core game engine resources.
- **Sekai.Engine.Testing**
    - Provides extendability for headless testing.
### extensions
- **Sekai.Dummy**
    - Provides dummy windowing support.
- **Sekai.SDL**
    - Provides [**SDL 2**](https://www.libsdl.org/) windowing and input support.
- **Sekai.Veldrid**
    - Provides [**Veldrid**](https://github.com/mellinoe/veldrid) graphics support.
### framework
- **Sekai.Framework**
    - Implements various systems for core functionality such as dependency injection, logging, threading, and storage management.
- **Sekai.Framework.Audio**
    - Implements audio abstractions.
- **Sekai.Framework.Input**
    - Implements input abstractions.
- **Sekai.Framework.Graphics**
    - Implements graphics abstractions.
- **Sekai.Framework.Windowing**
    - Implements windowing abstractions.
