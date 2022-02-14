using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epoche.MVVM.ViewModels;
using Epoche.MVVM.SourceGenerator;
using Epoche.MVVM.Models;

namespace Epoche.MVVM.SourceGenerator.Dummy;

[UseSourceGen]
[WithFactory]
partial class SomeModel : ModelBase, IModelInitializer<int>
{
    public void Initialize(int dto) => throw new NotImplementedException();
}

[UseSourceGen]
partial class Test: ViewModelBase
{
    [Property]
    [FactoryInitialize(InitializeExpression = "4")]
    readonly SomeModel someModel;
}
