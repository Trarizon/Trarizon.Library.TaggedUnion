// See https://aka.ms/new-console-template for more information
using System.Numerics;
using System.Runtime.CompilerServices;
using Trarizon.Library.TaggedUnion.Attributes;


Console.WriteLine("Hello, World!");

var test = new Test();
test.Print();
Console.WriteLine() ;
ref Sub sub = ref Unsafe.As<object, Sub>(ref test._0);
sub = new Sub();
test.Print();

namespace Ns
{
    partial class A
    {
        [UnionTag("CustomNameUnion",
            Options = TaggedUnionGenerationOptions.RefView |
                TaggedUnionGenerationOptions.IDisposable |
                TaggedUnionGenerationOptions.Match |
                TaggedUnionGenerationOptions.EqualityComparison)]
        public enum UnionTag
        {
            A,
            [TagVariant<int, string>("Id", "Name")]
            B,
            [TagVariant<int, DateTime>]
            C,
        }

        partial struct CustomNameUnion
        {
        }
    }
}

partial class Program
{
    struct Test
    {
        public object _0;
        public object _1;
        public object _2;
        public object _3;
        public object _4;

        public void Print()
        {
            Console.WriteLine(_0 ?? "<null>");
            Console.WriteLine(_1 ?? "<null>");
            Console.WriteLine(_2 ?? "<null>");
            Console.WriteLine(_3 ?? "<null>");
            Console.WriteLine(_4 ?? "<null>");
        }
    }

    struct Sub()
    {
        public string A = "A";
        public List<int> L = new(5) { 5, 8, 6 };
        public int[] Ar = [1, 2, 3, 3, 8, 4, 5, 15, 4];
    }

}