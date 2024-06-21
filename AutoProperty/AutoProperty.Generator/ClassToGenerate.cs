namespace AutoProperty.Generator;

public record ClassToGenerate
{
    public string NamespaceName { get; set; }
    
    public string ClassName { get; set; }
    
    public string[] Properties { get; set; }
}