using System.Collections;

namespace AutoProperty.Generator;

internal record ClassToGenerate
{
    public string NamespaceName { get; set; }
    
    public string ClassName { get; set; }
    
    public EquatableArray<string> Properties { get; set; }
}