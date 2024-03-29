﻿using Epoche.MVVM.SourceGenerator.Models;
using Epoche.MVVM.SourceGenerator.Models.Attributes;

namespace Epoche.MVVM.SourceGenerator.Builders.Attributes;
static class WithFactoryAttributeModelBuilder
{
    public static void Build(OutputModel outputModel, ClassModel classModel, AttributeData attributeData)
    {
        var model = new WithFactoryAttributeModel
        {
            AttributeData = attributeData
        };

        foreach (var named in attributeData.NamedArguments)
        {
            switch (named.Key)
            {
                case "FactoryName":
                    model.FactoryName = named.Value.Value as string;
                    model.UseFactoryNameDefault = false;
                    break;
                case "FactoryAccessModifier":
                    model.FactoryAccessModifier = named.Value.Value as string;
                    break;
            }
        }

        if (!model.UseFactoryNameDefault && string.IsNullOrEmpty(model.FactoryName))
        {
            outputModel.Context.Report(Diagnostics.Warnings.NoFactoryInfo, attributeData.AttributeConstructor);
            return;
        }

        classModel.WithFactoryAttribute = model;
    }
}
