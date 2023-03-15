namespace PboSpy.Modules.Signatures.Views;

internal interface IHexEditorView
{
    void SubmitChanges();
    void SubmitChanges(string path);
}
