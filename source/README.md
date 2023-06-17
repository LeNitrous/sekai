# Project Structure

Sekai is structured where abstractions and concrete implementations as separate projects. This allows development to be flexible and allows use of these projects to be used outside of Sekai itself.

- **`Sekai`**
    - The main project.
- **`Sekai.Framework`**
    - The framework components of Sekai.
- **`Sekai.Framework.Audio`**
    - The audio subsystem abstraction.
- **`Sekai.Framework.Audio.OpenAL`**
    - The OpenAL backed audio subsystem.
- **`Sekai.Framework.Graphics`**
    - The graphics subsystem abstraction.
- **`Sekai.Framework.Graphics.OpenGL`**
    - The OpenGL backed graphics subsystem.
- **`Sekai.Framework.Platform`**
    - The platforming abstraction.
- **`Sekai.Framework.Platform.Desktop`**
    - The desktop platform implementation.
- **`Sekai.Framework.Platform.Headless`**
    - The headless platform implementation.