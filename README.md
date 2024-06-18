# Getting Started With Source Generators | KCDC 2024

This readme file contains the excercices and code snippets that will be used during the "lab" portions of the workshop session.

## Prerequisites

Before getting started, ensure the following is setup and installed on your development machine.

### IDE

One, or multiple, of the following:

*this workshop will be mostly agnostic of IDE and operating system.*

- Rider

- VS Code
  - Extensions:
    - [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)

- Visual Studio 2022

### SDKs and Tools

- .NET 8 SDK

## Hands-On 1 | Create Projects

For the first hand-on section, create a new solution file called `AutoProperty.sln`. Once created, the following projects:

### AutoProperty.csproj

Add a new class library project called `AutoProperty.csproj` that targets the `netstandard2.0` framework.  This is the project that
will contain the code for our source generator. 

Once created, delete all auto created `.cs` files and update the `.csproj` file contents to be the following:

```xml
<Project Sdk="Microsoft.NET.Sdk">
	<PropertyGroup>
		<TargetFramework>netstandard2.0</TargetFramework>
		<IsPackable>false</IsPackable>
		<LangVersion>latest</LangVersion>

		<EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
		<IsRoslynComponent>true</IsRoslynComponent>
	</PropertyGroup>

	<ItemGroup>
		<PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="3.3.4">
			<PrivateAssets>all</PrivateAssets>
			<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
		</PackageReference>
		<PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.8.0" />
		<PackageReference Include="Microsoft.CodeAnalysis.CSharp.Workspaces" Version="4.8.0" />
		<PackageReference Include="Microsoft.Net.Compilers.Toolset" Version="4.8.0">
		  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
		  <PrivateAssets>all</PrivateAssets>
		</PackageReference>
	</ItemGroup>
</Project>
```

### AutoProperty.Runner.csproj

Add a new console application project called `AutoProperty.csproj` that targets the `net8.0` framework.  This project will facilicate
executing our source generator against a sample set of code. It enables us to execute the generator on demand and work around some inconsitencies
between differnt .net IDEs and supported operating systems.

Once created, delete all auto created `.cs` files and update the `.csproj` file contents to be the following:

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net8.0</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <ProjectReference Include="..\AutoProperty\AutoProperty.csproj" />
    </ItemGroup>

    <ItemGroup>
      <PackageReference Include="Microsoft.CodeAnalysis.Common" Version="4.8.0" />
    </ItemGroup>

</Project>
```