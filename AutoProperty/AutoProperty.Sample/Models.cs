namespace AutoProperty.Sample;

public partial class Author : IHasId, IHasActiveFlag, IAuditMetadata
{
    public required string Name { get; set; }
}

public partial class Book : IHasId, IHasTitle, IHasActiveFlag, IAuditMetadata
{
    public required string Title { get; set; }

    public required string Author { get; set; }
}