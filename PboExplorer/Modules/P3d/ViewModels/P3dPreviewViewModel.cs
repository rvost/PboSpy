using BIS.P3D;
using PboExplorer.Models;
using PboExplorer.Modules.Preview.ViewModels;

namespace PboExplorer.Modules.P3d.ViewModels;

internal class P3dPreviewViewModel : PreviewViewModel
{
    public P3dPreviewViewModel(FileBase model, P3D p3d) : base(model)
    {
        Summary = new(p3d);

        LODs = p3d.LODs.Select(lod => new LodViewModel(lod)).ToList();
    }

    public P3dSummary Summary { get; set; }
    public IEnumerable<LodViewModel> LODs { get; set; }

    protected override void CanExecuteCopy(Command command)
        => command.Enabled = false;

    protected override Task ExecuteCopy(Command command)
        => throw new NotImplementedException();
}
