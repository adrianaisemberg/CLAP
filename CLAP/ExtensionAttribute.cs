//
// Since downgrading from .NET 4.0 to 2.0,
// System.Runtime.CompilerServices.ExtensionAttribute is no longer available.
// This fake one is good enough to allow the use of extension methods.
//

namespace System.Runtime.CompilerServices
{
    internal class ExtensionAttribute : Attribute
    {
    }
}
