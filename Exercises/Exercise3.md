## Exercise 3 | Debugging The Source Generator

As the complexity of the source generator increases, we'll want to debug
it's logic. 

Since the code of a source generator runs at compile time rather than runtime, debugging them is a bit more complex. There are 2 primary ways to accomplish this:

1) Create a project that is able to define syntax trees manually and execute the generator against them.

    - This is the approach that generator unit test projects take

    - You can also create a console "Runner" project to execute the generator and print the output.

2) Use Rider or Visual Studio 2022 to perform "live" debugging of the generator against a sample project.

### Unit Test / Runner Project

We're going to skip over this approach for now, and come back to it later.

### Live Debugging

Unfortunately, Visual Studio Code doesn't have the ability to perform live debugging of source generators (that I know of). So you'll need to use Rider of VS 2022 for this.

To configure Rider or Visual Studio 2022 to debug, add a `launchSettings.json` to the Properties folder of the **AutoProperty.Generator** project:

```json
{
	"$schema": "http://json.schemastore.org/launchsettings.json",
	"profiles": {
		"AutoProperty.Generator": {
			"commandName": "DebugRoslynComponent",
			"targetProject": "../AutoProperty.Sample/AutoProperty.Sample.csproj"
		}
	}
}
```

Also verify that `AutoProperty.Generator.csproj` has the following Property defined in a PropertyGroup

```xml
<!-- Special property for IDEs to help with debugging -->
<IsRoslynComponent>true</IsRoslynComponent>
```

The combination of these 2 things inform the IDE that the generator project can be debugged. The critical part of the profile is the `commandName` value of `DebugRoslynComponent`. Without this, the generator project doesn't have any entry point for the debugger to connect to. The `targetProject` indicates the project that should be used as the compilation source/context for the generator.

You may need to reload the solution after adding the profile for the IDE to register it. Once it's available, set `AutoProperty.Generator` as the startup project, set a breakpoint on the first line of the initialize method and start debugging.

![alt text](../images/debugger.png)

#### A Couple Notes For Debugging

- The incremental generator pipeline is similar to an IEnumerable, you need to register the output of the pipeline for the predicate and transform methods to be executed against the syntax tree.

- It is easy to unintentionally create an invalid debugging state. In this setup, for debugging to work the target project needs to compile. Live debugging of generators are a bit of a chicken and egg situation.

    Since the generator always runs apart of the  compilation, even before the debugger is attached, the generator may create invalid code that cause the target project to not compile. Since the target project can't compile the debugger can load the generator context.

    This issue can be worked around/through by using a Unit Test or Runner project with a manually created syntax tree.