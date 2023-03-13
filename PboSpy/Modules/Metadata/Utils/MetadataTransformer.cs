using Microsoft.Extensions.Logging;
using PboSpy.Interfaces;
using PboSpy.Models;
using PboSpy.Modules.Metadata.Models;

namespace PboSpy.Modules.Metadata.Utils;

internal class MetadataTransformer : ITreeItemTransformer<Task<IMetadata>>
{
    private readonly ILogger<MetadataTransformer> _logger;

    public MetadataTransformer(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<MetadataTransformer>();
    }

    public Task<IMetadata> Transform(PboDirectory entry)
        => Task.FromResult<IMetadata>(new PboDirectoryMeatdata(entry));

    public Task<IMetadata> Transform(PboEntry entry)
    {
        return Task.Run(() =>
        {
            IMetadata metadata = null;
            try
            {
                metadata = entry.Extension switch
                {
                    ".paa" or ".pac" => new PaaEntryMetadata(entry),
                    ".rvmat" or ".sqm" => new ConfigEntryMetadata(entry),
                    ".wrp" => new WrpEntryMetadata(entry),
                    ".p3d" => new P3dEntryMetadata(entry),
                    _ => new PboEntryMetadata(entry),
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when creating metadata for PBO entry {entry}", entry);
            }
            return metadata;
        });
    }
    public Task<IMetadata> Transform(PboFile entry)
        => Task.FromResult<IMetadata>(new PboMetadata(entry));

    public Task<IMetadata> Transform(PhysicalFile entry)
        => Task.FromResult<IMetadata>(new PhysicalFileMetadata(entry));

    public Task<IMetadata> Transform(PhysicalDirectory entry)
        => Task.FromResult<IMetadata>(new PhysicalDirectoryMetadata(entry));

    public Task<IMetadata> Transform(ConfigClassItem entry)
        => Task.FromResult<IMetadata>(new DictionaryPropertyGridAdapter<string, object>(entry.Properties));

}
