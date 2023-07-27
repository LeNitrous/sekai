# Project Structure

Sekai is structured where abstractions and concrete implementations as separate projects. This allows development to be flexible and allows use of these projects to be used outside of Sekai itself.

- **`Sekai`**
    - The main project.
- **`Sekai.GLFW`**
    - The GLFW backed windowing subsystem.
- **`Sekai.OpenAL`**
    - The OpenAL backed audio subsystem.
- **`Sekai.OpenGL`**
    - The OpenGL backed graphics subsystem.