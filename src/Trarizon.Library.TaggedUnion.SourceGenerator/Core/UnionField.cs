namespace Trarizon.Library.TaggedUnion.SourceGenerator.Core;
internal struct UnionField
{
    public UnionFieldKind Kind { get; }

    private UnionField(UnionFieldKind kind) => Kind = kind;

    private int _index;
    private int _index2;

    public readonly int Class_Index => _index;

    public readonly int Unmanaged_TupleItemIndex => _index;

    public readonly (int TypeIndex, int InstanceIndex) ManagedStruct => (_index, _index2);

    public static UnionField New_Class(int index)
    {
        return new UnionField(UnionFieldKind.Class) {
            _index = index,
        };
    }

    public static UnionField New_Unmanaged(int tupleItemIndex)
    {
        return new UnionField(UnionFieldKind.Unmanaged) {
            _index = tupleItemIndex,
        };
    }

    public static UnionField New_ManagedStruct(int typeIndex, int instanceIndex)
    {
        return new UnionField(UnionFieldKind.ManagedStruct) {
            _index = typeIndex,
            _index2 = instanceIndex,
        };
    }
}

enum UnionFieldKind
{
    Class,
    Unmanaged,
    ManagedStruct,
}