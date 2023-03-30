namespace PboSpy.Modules.Metadata;

public interface IMetadataHandler
{
    IMetadataHandler Next { get; set; }
    IMetadata Handle(object obj, Dictionary<string, object> context);
}
