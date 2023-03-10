using BIS.P3D;

namespace PboSpy.Modules.P3d.ViewModels;

internal class LodViewModel : PropertyChangedBase
{
    public LodViewModel(ILevelOfDetail lod)
    {
        Resolution = lod.Resolution;
        Faces = lod.FaceCount;
        Vertexes = lod.VertexCount;
        HashId = lod.GetModelHashId().ToString();

        NamedProperties = lod.NamedProperties.OrderBy(p => p.Item1);
        NamedSelections = lod.NamedSelections.OrderBy(m => m.Name)
            .Select<INamedSelection, Tuple<string, string, string>>(s => new(s.Name, s.Material, s.Texture));

        Textures = lod.GetTextures();
        Materials = lod.GetMaterials();
    }

    public float Resolution { get; }

    public int Faces { get; }

    public uint Vertexes { get; }

    public string HashId { get; }

    public IEnumerable<Tuple<string, string>> NamedProperties { get; }

    public IEnumerable<Tuple<string, string, string>> NamedSelections { get; }

    public IEnumerable<string> Textures { get; }

    public IEnumerable<string> Materials { get; }

}
