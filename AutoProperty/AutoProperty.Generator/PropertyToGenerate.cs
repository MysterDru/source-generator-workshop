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