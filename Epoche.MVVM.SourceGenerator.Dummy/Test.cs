using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epoche.MVVM.ViewModels;
using Epoche.MVVM.SourceGenerator;
using Epoche.MVVM.Models;

namespace Epoche.MVVM.SourceGenerator.Dummy;

interface IMeow<T> { }

class SomeModel : ModelBase { }


[UseSourceGen]
abstract partial class Test<TBlarg, U>: ViewModelBase
    where TBlarg: ModelBase
{
    [FactoryInitialize]
    [Property]
    TBlarg something;

    [Command]
    void Meow() { }
}

[UseSourceGen]
partial class Test2 : Test<SomeModel, int> { }

