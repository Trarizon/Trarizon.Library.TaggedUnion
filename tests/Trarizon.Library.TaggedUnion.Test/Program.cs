// See https://aka.ms/new-console-template for more information
using Trarizon.Library.TaggedUnion.Attributes;


Console.WriteLine("Hello, World!");
Ns.A.CustomNameUnion union = default;

namespace Ns
{
    partial class A
    {
        [UnionTag("CustomNameUnion",
            Options = TaggedUnionGenerationOptions.RefView|
                TaggedUnionGenerationOptions.IDisposable|
                TaggedUnionGenerationOptions.Match)]
        public enum UnionTag
        {
            A,
            [TagVariant<int, string>("Id", "Name")]
            B,
            [TagVariant<int, DateTime, ManagedStructSample>]
            C,
        }
    }
}

struct UnmanagedStructSample
{
    int _a;
    int _b;
    long _c;
}

struct ManagedStructSample
{
    object _obj;
    int _b;
}

class ReferenceTypeSample
{

}