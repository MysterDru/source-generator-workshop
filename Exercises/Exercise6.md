## Exercise 6 | Optimize the Generator with Caching

Currently, our `TransformNode` method returns a `ValueTuple<>` with 3 values: 

- `string NamespaceName`
- `string ClassName`
- `string[] Properties`

It might not seem like it, but this return value will break the caching mechanism in the incremental generator pipelines. When detecting changes an incremental generator will compare prior results of the TransformNode against the current iteration. However, that comparison is done based on _value_ equality not _reference_ equality.

Our current return value doesn't have value equality with itself. It contains a collection and none of the default collection types in C# have value equality, only reference equality.

So, we need to create some custom models that can hold our transformation data and make it cachable.

| :exclamation:  Don't return SyntaxNode or ISymbol from TransformNode   |
|-----------------------------------------|
| Both types are large models and do not support value equality. Returning these types will break caching! |

### ClassToGenerate model

First, we'll create a new `record` type to hold our current structure. We use the record type as it comes with built in value equality. Otherwise, we would need to manually implement our equality checks. Typically this is best done by implementing `IEquatable<>`.

Add the following record class to the generator project, and replace the tuple types in the generator with this instead. 

You'll need to slightly modify the null handling in the `Where` and `Select` of the pipeline as well.

```csharp

namespace AutoProperty.Generator;

internal record ClassToGenerate
{
    public string NamespaceName { get; set; }
    
    public string ClassName { get; set; }
    
    public string[] Properties { get; set; }
}
```

We still have a problem with this record. It's using an array for `Properties`. We need to replace this data type with a collection that supports comparing the values of the collection intead of the reference pointers.

To simplify this, we'll borrow classes that have implemented by smart people in the .NET community. These come from [Andrew Lock](https://andrewlock.net/creating-a-source-generator-part-9-avoiding-performance-pitfalls-in-incremental-generators/#4-watch-out-for-collection-types).

Download these files and place them in a `Utils` folder in the Generator project.
- [EquatableArray`T](./files/EquatableArray.cs)
- [HashCode](./files/HashCode.cs)

Now, update `ClassToGenerate`:

```csharp
public EquatableArray<string> Properties { get; set; }
```

And it's property assignment in `TransformNode`:
```csharp
return new ClassToGenerate()
{
    NamespaceName = classSymbol.ContainingNamespace.ToDisplayString(),
    ClassName = classSymbol.Name,
    Properties = new EquatableArray<string>(unimplementedProperties)
};
```

If you've been keeping your unit test updated as we go, you should be able to run your tests and validate the generator is still creating the expected output.

### Offload Property Building

It is considered best practice to not return generated output from the transformer in the generator pipeline, such as the full property strings. Instead, the transformer should return only the necessary metadata needed for the generate method to create and build it's output.

Our implementation is pretty small and likely wouldn't cause large performance issues. But as you scale generators or do more complicated evaluation, it's a good habit to have.

So, let's make a new record to hold our property information.

```csharp
namespace AutoProperty.Generator;

internal record PropertyToGenerate
{
    public string Visibility { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public bool HasGet { get; set; }

    public bool HasSet { get; set; }

    public bool IsInit { get; set; }
}
```

And update the `ClassToGenerate` again.

```csharp
public EquatableArray<PropertyToGenerate> Properties { get; set; }
```

Then, we'll update the property selectors in `TransformNode`

```csharp
// Add the unimplemented properties
var unimplementedProperties = interfaceProperties
    .Where(i => !classProperties.Exists(c => c.Name == i.Name))
    .Select(i => new PropertyToGenerate()
    {
        Visibility = "public",
        Type = i.Type.ToDisplayString(),
        Name = i.Name,
        HasGet = i.GetMethod != null,
        HasSet = i.SetMethod != null,
        IsInit = i.SetMethod is {IsInitOnly: true},
    })
    .ToArray();

// Add the unimplemented properties in the return result
return new ClassToGenerate()
{
    NamespaceName = classSymbol.ContainingNamespace.ToDisplayString(),
    ClassName = classSymbol.Name,
    Properties = new EquatableArray<PropertyToGenerate>(unimplementedProperties)
};
```

And finally, move our builder logic to the `GenerateOutput` method.

```csharp
private static void GenerateOutput(SourceProductionContext context, ClassToGenerate classToGenerate)
{
    var formattedProperties = classToGenerate.Properties.Select(i =>
    {
        var getAccessor = i.HasGet ? "get;" : "";
        var setAccessor = i.HasSet ? "set;" : "";

        setAccessor = i.IsInit ? "init;" : setAccessor;

        return $"{i.Visibility} {i.Type} {i.Name} {{ {getAccessor} {setAccessor} }}";
    });

    var properties = string.Join("\n\n\t\t\t", formattedProperties);

    // remainder of method left out for brevity
```

Run those unit tests again, or build and insepct the generated files. You should see the same output as before. We've just modified some of the internal ordering of our logic.

## Wrapping Up

We have a fairly complete generator now. What happens if we want to use this outside of the local solution? 

In the [next exercise](./Exercise7.md), we'll take a look at how to package the generator assembly as a nuget pacakge.