# Getting Started With Source Generators Workshop

This repository contains the hands-on exercises that will be used during the workshop session.

## Prerequisites

Before getting started, ensure the following is setup and installed on your development machine.

### IDE

One, or multiple, of the following:

*this workshop will be mostly agnostic of IDE and operating system.*

- Rider (with support for .NET 8)
  - Version 2024.1 or newer is preferred, but 2023.3 should work as well.

- Visual Studio 2022
  - Latest Version, 17.10 at time of writing.
  - Workloads:
    - Visual Studio Extension Development
      - .NET Compiler Platform SDK
      - DGML editor
      - https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/#installation-instructions---visual-studio-installer

- VS Code
  - Extensions:
    - [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)

- Visual Studio 2022

### SDKs and Tools

- .NET 8 SDK
  - Preferably version `8.0.302`, but any .net8 sdk version should be sufficient. 


## Exercises

The workshop is broken into smaller hands-on exercises to be completed. 

Work through each of the following when instructed, or work through them at your own pace.

- [Exercise 1 | Create Projects & Initial Files](Exercises/Exercise1.md)

- [Exercise 2 | Discover Classes & Add Output](Exercises/Exercise2.md)

- [Exercise 3 | Debugging The Generator](Exercises/Exercise3.md)

- [Exercise 4 | Generating a Partial Class](Exercises/Exercise4.md)

- [Exercise 5 | Working With the Semantic Model](Exercises/Exercise5.md)

- [Exercise 6 | Optimize the Generator with Caching](Exercises/Exercise6.md)

- [Exercise 7 | Packing the Generator](Exercises/Exercise7.md)

- [Exercise 8 | Final Optimizations](Exercises/Exercise8.md)