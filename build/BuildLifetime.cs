using System.IO;
using Cake.Core;
using Cake.Frosting;

namespace build;

public class BuildLifetime : FrostingLifetime<BuildContext>
{
    public override void Setup(BuildContext context, ISetupContext info) { }

    public override void Teardown(BuildContext context, ITeardownContext info)
    {
        CleanStubbedLibs(context);
    }

    private static void CleanStubbedLibs(BuildContext context)
    {
        foreach (var reference in context.References)
        {
            var stubbedRef = context.StubbedLibsDir / reference;
            if (File.Exists(stubbedRef))
                File.Delete(stubbedRef);
        }
    }
}