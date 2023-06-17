# Project Structure

Sekai is structured where abstractions and concrete implementations as separate projects. This allows development to be flexible and allows use of these projects to be used outside of Sekai itself.

- **`Sekai`**
    - The main project.
- **`Sekai.Framework`**
    - The framework components of Sekai.
- **`Sekai.Audio`**
    - The audio subsystem abstraction.
- **`Sekai.Audio.OpenAL`**
    - The OpenAL backed audio subsystem.
- **`Sekai.Graphics`**
    - The graphics subsystem abstraction.
- **`Sekai.Graphics.OpenGL`**
    - The OpenGL backed graphics subsystem.
- **`Sekai.Platform`**
    - The platforming abstraction.
- **`Sekai.Platform.Desktop`**
    - The desktop platform implementation.
- **`Sekai.Platform.Headless`**
    - The headless platform implementation.