## Exercise 7 | Packing the Generator

We've built and used our source generator locally in the solution, but typically you'll want to distribute the analyzer as a nuget package to be used by other solutions & projects.

### Create the Package Configuration

SDK style projects make it really easy to build and package a nuget.

Add a new `<PropertyGroup>` and `<ItemGroup>` section to `AutoProperty.Generator.csproj`

```xml
<PropertyGroup>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <IncludeBuildOutput>false</IncludeBuildOutput>
    <Version>1.0.0</Version>
    <AssemblyVersion>$(Version).0</AssemblyVersion>
    <AssemblyFileVersion>$(Version).0</AssemblyFileVersion>
    <Product>AutoProperty</Product>
    <Description>Contains a C# Source Generator auto implement interface properties.</Description>
    
    <PackageOutputPath>$(MSBuildProjectDirectory)\..\build</PackageOutputPath>
</PropertyGroup>

<ItemGroup>
    <!-- Package the generator in the analyzer directory of the nuget package -->
    <None Include="$(OutputPath)\$(AssemblyName).dll"
          Pack="true"
          PackagePath="analyzers/dotnet/cs" Visible="false" />
  </ItemGroup>
```

These properties are pretty standard for creating a nuget package from any class library. We've added a few extra:

`GeneratePackageOnBuild` and `PackageOutputPath` will drop the `nupkg` into a build folder of the solution directory everytime the generator is built.

Additionally, we need to specify that the dll for the generator project is packaged to the `analyzers` folder for the package. Normally, dlls are placed in the libs folder. But Nuget requires roslyn components to be in the analyzers folder of the package.

Build the generator project, and you should see a `AutoProperty.Generator.1.0.0.nupkg` file in `$(SolutionDirectory)\build` folder.

### Testing the Nuget

If you want to test the installation of your nuget we need to register the local folder as a nuget source. Create a `Nuget.config` in root of the solution directory and add the build folder as a local package source:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <clear />
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
        <!-- use build folder as a local package source -->
        <add key="Local" value="build" />
    </packageSources>
</configuration>
```

Next create a new project `AutoProperty.NugetTest.csproj` and add a package reference to our new package:

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net8.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
    </PropertyGroup>
    
    <ItemGroup>
        <PackageReference Include="AutoProperty.Generator" Version="1.0.0">
            <PrivateAssets>all</PrivateAssets>
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageReference>
    </ItemGroup>
</Project>
```

The PrivateAssets and IncludeAssets nodes for the package reference are standard setup for roslyn package references. They indicate that the package should be added as an analyzer; referencing the `analyzer` folder of the package.

Next, you can add a class and an interface as in prior tasks to trigger the generator.

```csharp
namespace AutoProperty.NugetTest;

public interface IModel
{
    public int Id { get; set; }
}

public partial class Models : IModel
{
    
}
```

### Wrapping Up

Success! We've create an incremental generator that auto implements our entity and dto properties. 
