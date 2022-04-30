using Epoche.MVVM.Models;
using Epoche.MVVM.ViewModels;

namespace Epoche.MVVM.SourceGenerator.Dummy;

interface IMeow<T> { }

class SomeModel : ModelBase { }


[UseSourceGen]
[Inject(typeof(IMeow<string>), Name = "StringMeow")]
[Inject("IMeow<TBlarg>", Name = "BlargMeow")]
abstract partial class Test<TBlarg, U> : ViewModelBase
    where TBlarg : ModelBase
{
    [FactoryInitialize]
    [Property]
    TBlarg something;

    [Command]
    void Meow() { }
}

[UseSourceGen]
partial class Test2 : Test<SomeModel, int> { }

