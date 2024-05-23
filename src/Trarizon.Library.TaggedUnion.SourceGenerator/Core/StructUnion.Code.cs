namespace Trarizon.Library.TaggedUnion.SourceGenerator.Core;
partial record StructUnion
{
    private string Code_GetUnmanagedTuple(string instance, Variant variant)
    {
        return $"global::{Literals.Unsafe_As_MethodName}<{InternalUnmanagedUnionFullQualifiedTypeName}, {variant.UnmanagedTupleTypeName}>(ref {instance}.__unmanageds)";
    }
}
