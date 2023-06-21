<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="./assets/logo-dark.png"/>
    <source media="(prefers-color-scheme: light)" srcset="./assets/logo-light.png"/>
    <img alt="Sekai" src="./assets/logo-light.png"/>
  </picture>
</p>

<p align="center">Sekai is the framework that powers <a href="https://github.com/vignetteapp/vignette">Vignette</a>.</p>

<br/>

<p align="center">
  <img src="https://img.shields.io/github/stars/vignetteapp/sekai?style=flat-square"/>
  <img alt="GitHub" src="https://img.shields.io/github/license/vignetteapp/sekai?color=c850c1&style=flat-square">
  <img src="https://img.shields.io/discord/871618277258960896?logo=discord&color=5865f2&style=flat-square"/>
  <img src="https://img.shields.io/static/v1?label=website&message=vignetteapp.org&color=ea1a72&style=flat-square"/>
</p>
<p align="center">
  <img alt="GitHub Workflow Status" src="https://img.shields.io/github/actions/workflow/status/vignetteapp/sekai/test.yml?label=test&style=flat-square">
  <img alt="GitHub Workflow Status" src="https://img.shields.io/github/actions/workflow/status/vignetteapp/sekai/lint.yml?label=lint&style=flat-square">
  <img alt="Code Coverage" src="https://img.shields.io/codecov/c/gh/vignetteapp/sekai?style=flat-square">
</p>

## Introduction
Sekai is a fully abstracted graphics framework written under the .NET Runtime in C# inspired by other frameworks such as [LÖVE2D](https://github.com/love/love2d) and [osu! framework](https://github.com/ppy/osu-framework). Its core components such as rendering, audio, windowing, input, and even storage can easily be replaced to suit their requirements. The repository contains implementations that make use of GLFW, OpenGL, and OpenAL as a starting point.

Sekai is used to build [Vignette](https://github.com/vignetteapp/vignette) and is the choice of platform for Cosyne's graphical-related projects.

## Getting Started

### Building
Please make sure you meet the following prerequisistes:
- A desktop platform with .NET 7 or above installed.

### Examples
There are examples in the `./samples/` directory.

## License
Sekai is licensed under MIT. See the [the license](./LICENSE) file in the root of this repository for the full text.
