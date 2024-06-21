namespace AutoProperty.Sample;

public interface IHasId
{
    public int Id { get; set; }
}

public interface IHasTitle
{
    public string Title { get; set; }
}

public interface IHasActiveFlag
{
    public bool IsActive { get; set; }
}

public interface IAuditMetadata
{
    DateTimeOffset LastUpdated { get; set; }

    bool IsEditable { get; }
}