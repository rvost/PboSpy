namespace PboSpy.Modules.Metadata;

public interface IMetadataHandler
{
    MetadataHandlerPriority Priority { get; }
    IMetadataHandler Next { get; set; }
    IMetadata Handle(object obj, Dictionary<string, object> context);
}
