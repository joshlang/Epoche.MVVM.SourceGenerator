using Epoche.MVVM.SourceGenerator.Models;
using Epoche.MVVM.SourceGenerator.Models.Attributes;

namespace Epoche.MVVM.SourceGenerator.Builders.Attributes;
static class CommandAttributeModelBuilder
{
    public static void Build(OutputModel outputModel, ClassModel classModel, MethodModel methodModel, AttributeData attributeData)
    {
        if (!classModel.IsMVVMViewModel)
        {
            outputModel.Context.Report(Diagnostics.Errors.NotViewModel, attributeData.AttributeConstructor);
            return;
        }

        var model = new CommandAttributeModel
        {
            AttributeData = attributeData
        };

        if (!attributeData.ConstructorArguments.IsDefaultOrEmpty)
        {
            model.Name = attributeData.ConstructorArguments[0].Value as string;
        }
        foreach (var named in attributeData.NamedArguments)
        {
            switch (named.Key)
            {
                case "Name":
                    model.Name = named.Value.Value as string;
                    break;
                case "AllowConcurrency":
                    model.AllowConcurrency = (bool?)named.Value.Value ?? false;
                    break;
                case "CanExecute":
                    model.CanExecute = named.Value.Value as string;
                    break;
                case "TaskName":
                    model.UseDefaultTaskName = false;
                    model.TaskName = named.Value.Value as string;
                    break;
            }
        }

        methodModel.CommandAttribute = model;
    }
}
