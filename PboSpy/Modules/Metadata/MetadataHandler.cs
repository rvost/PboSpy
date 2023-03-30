namespace PboSpy.Modules.Metadata;

public abstract class MetadataHandler : IMetadataHandler
{
    public IMetadataHandler Next { get; set; }

    public virtual IMetadata Handle(object obj, Dictionary<string, object> context) 
        => Next?.Handle(obj, context);
}
