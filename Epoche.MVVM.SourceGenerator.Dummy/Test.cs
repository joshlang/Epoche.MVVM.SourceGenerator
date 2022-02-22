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
partial class Test<T, U>: ViewModelBase
{
    [Command]
    void Meow() { }
}
