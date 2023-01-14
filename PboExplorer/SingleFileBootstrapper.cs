using System.Reflection;

namespace PboExplorer;

public class SingleFileBootstrapper : Gemini.AppBootstrapper
{
    public override bool IsPublishSingleFileHandled => true;

    protected override IEnumerable<Assembly> PublishSingleFileBypassAssemblies
    {
        get
        {
            yield return Assembly.GetAssembly(typeof(Gemini.AppBootstrapper));
            //yield return Assembly.GetAssembly(typeof(Gemini.Modules.CodeCompiler.ICodeCompiler));
            yield return Assembly.GetAssembly(typeof(Gemini.Modules.CodeEditor.ILanguageDefinition));
            //yield return Assembly.GetAssembly(typeof(Gemini.Modules.ErrorList.IErrorList));
            //yield return Assembly.GetAssembly(typeof(Gemini.Modules.GraphEditor.Module));
            //yield return Assembly.GetAssembly(typeof(Gemini.Modules.Inspector.IInspectorTool));
            //yield return Assembly.GetAssembly(typeof(Gemini.Modules.Output.IOutput));
            yield return Assembly.GetAssembly(typeof(Gemini.Modules.PropertyGrid.IPropertyGrid));
            // add more assemblies with exports as needed here
        }
    }
};
