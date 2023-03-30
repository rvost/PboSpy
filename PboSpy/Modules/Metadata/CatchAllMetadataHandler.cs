namespace PboSpy.Modules.Metadata;

internal class CatchAllMetadataHandler: MetadataHandler
{
    public override IMetadata Handle(object obj, Dictionary<string, object> context) 
        => context.Any() ? new GenericMetadata(context) : null;
}
