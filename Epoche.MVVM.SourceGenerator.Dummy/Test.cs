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
    [Property(OnChange = nameof(YaySomethingChanged))]
    TBlarg something;

    [FactoryInitialize(InjectOnly = true)]
    [Property]    
    TBlarg? something2;

    [ChangedBy(nameof(something))]
    string? RandomThing { get; }

    [Command]
    void Meow() { }

    void YaySomethingChanged() { }
}

[UseSourceGen]
partial class Test2 : Test<SomeModel, int> { }

