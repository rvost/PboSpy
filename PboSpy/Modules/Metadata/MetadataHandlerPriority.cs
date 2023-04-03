namespace PboSpy.Modules.Metadata;

public enum MetadataHandlerPriority
{
    Generic = 0,
    FormatGeneric = 2,
    FormatSpecific = 4,
    FormatAgnostic = 8,
    Highest = 16,
}
