�ṩ���ֶ��巽��
- [UnionTag] enum����Ǹ�enumΪtag���ͣ���ͬnamespace������union type
  - [TagVariant] enum field: ���variant
  - !��C#�﷨���ƣ��÷�ʽ�޷�ʹ�÷���
- [TaggedUnion] struct: ���Ϊtaggedunion���ͣ���ͬnamespace������union tag enum
  - ��дһ��static partial method���Բ�������taggedunion��ʹ��ValueTuple<>���嵥�������������÷����ᱻ����ʱɾ��


Options
- UnionTypeKind: Ĭ��Struct�����κ�һ���ֶ�Ϊref structʱΪRefStruct
  - Struct: Ĭ�ϵ�TaggedUnion��structʼ���п���Ϊdefault�����tag enum��Ӧ�ṩ�вε�0ֵ
  - Class: ʹ��class��type pattern match check
  - Record: ʹ��Record��type pattern match check
  - RefStruct��һ��tag��һ��ref byte  // ��ȫ�Դ��ɣ��ݶ�
- Pack: ����ָ��[StructLayout]��Pack
- SequentialValueTypeLayout: ��ʾ��ͬ���͵�struct���Ṳ���ڴ��е�λ��
  - ����һ���ĸ���Ϊ��ѡ��
  - �������Ͳ�����ģ����Կ��Ա��ֹ���
- CreatorAccessibility: Create������ctor��accessibility
  - ��Record��Ч
- AllowFieldAccess: bool: ��Struct��Ч����ʾ�����û�ֱ�ӷ���fields������unsafe���� [�ݶ�]
- // AggressiveMinimizingSize:��ͼ���洢tag��ͨ���ֶλ�ȡtag
  - ��Ϊ��ȡtagʱ���������ƺ���С����ȡ����
  - ����TryGetXXX��ʹ�û�����ԣ��ݶ�
- GenerationOption:
  - IDisposable: �����ʵ��IDisposable���ֶΣ�ʵ��
  - Match: ʵ�� Match(Action<> matcher) �� Match<>(Func<> matcher, ...)

```
partial struct Union
{
    // �ýṹ�ڲ����ܴ��ڴ���Unsafe��cast����������û���Ӧ�����������κ�һ���ֶΣ�����tag��
    // �����structlayout�����ɻ󣬿���ʹ��tuple������ʵ��
    // ���ʹ��tuple�������Ϳ��Ժ������е�tuple��ȡͬ���Ĵ���
    object __obj0;
    object __obj1;
    ...
    
    readonly UnionTag __tag;
    
    // �����ⲿ���޸ĵĻ��������ֻ��enum�ͻ�������������
    // �����ǵĻ����԰�����unmanaged������
    __UnmanagedUnion __unmanageds;
    
    // �ⲿ����ܻ��޸�struct�ṹ�����Լ�ʹ�ֶ�ȫ���������ͣ�managed structҲ����ֱ��ӳ�䵽objects
    // �����ǵĻ����ڴ��������͵�struct���Ժ�objects�غ�
    ManagedA __managed0;
    ...
    // PS: unmanged generic constraint����������ʱ��飬Ҳ����˵���͵͵�滻dll�Ļ��ǿ��Խ�managed struct
    // ����unmanaged constraint�ģ���ʱ�������ƺ�Ҳ������
    // ��˿��Ǻ����ⲿ���޸ĵĿ��� [�ݶ�]
    
    
    // ! �ⲿ��ʵ�ֻ�ܸ��ӣ����Կ���
    // ����valuetupleӦ�ò����޸ģ���fieldΪvaluetupleʱ�����ǻ�����Դ����н�ʡ�����ֶ�
    // ����ref type�� unmanaged type�����ȴ�����Ѱ�ҿɷ���ռ�
    // ���ǲ�������tuple�ֶζ����е�reference typeתΪobject����Ȼʹ��Unsafe�ڻ�ȡʱ����ת��
    (ValueTuple) __vtuple0;
    ...
    
    #pragma warning disable CS8618
    private Union(UnionTag tag) => __tag = tag;
    
    public UnionTag Tag => __tag;
    
    [UnscopedRef]
    public VariantVariant Variant => new VariantVariant(ref this);
    
    // accessibility������
    public static Union CreateVariant(field) => new(UnionTag.Variant) { __field = field, };
    
    public bool TryGetVariant(out field)
    {
        if (__tag is UnionTag.Variant) {
            field = _field;
            return true;
        }
        else {
            field = default!;
            return false;
        }
    }

    // ���ֻ�л������ͣ����ǿ���ֱ�Ӽ��������ṹ�ĳߴ硣������ҪFieldOffsetһ��
    struct __UnmanagedUnion { } // ��������Unmanaged struct
    
    // û���ֶ�ʱ����������ṹ
    ref struct VariantVariant(ref Union union)
    {
        private ref Union _unionRef = ref union;
        
        public ref Field => _unionRef._field;
        
        // �첽�͵������в���ʹ��ref struct������ṩDeconstruct������
        // ~~����ֻ��һ��fieldʱû�ã���ʱ�����ɣ���Field���Ի�ȡ~~
        // ֻ��һ��fieldҲ���ɣ������Ҫ�û��Լ�����
        public void Deconstruct(out field) => field = _unionRef._field;
    }
}
```
- �û�ʼ�տ��Դ���default(Union)������������һ��Diagnostic(INFO)����ʾ�û����һ��ֵΪ0�����ֶ�enumֵ
�Ա��FlagAttr��enum��Diagnostic(INFO)

```
abstract partial class Union
{
    public bool TryAs<T>(out T value) where T : Union { }
    public T? As<T> where T : Union { }

    sealed partial class Variant(field) : Union
    {
        public Field; // yes we use public field rather than property
        
        // util methods:
        // Equals, Deconstruct
    }
}
```

 ������ƺ�class version����һ��
```
abstract record Union
{
    sealed record Variant(fields) : Union {}
}
```

RefStruct
[tag|ref byte|ref byte|..]


 Struct����ϸ���
```
int _maxRefTypeCount;
INamedTypeSymbol _tagSymbol;
//dict ʹ�òݸ��Program.Partial.Do()
//managed struct�Ĵ����unmanagedӦ���ǻ���һ�µġ�����managed struct���ܻ�ȡtuple��sub tuple
//unmanaged�������Ҫ���ڼ����ֶζ��壬��union sizeû��Ӱ��
Dictionary<ITypeSymbol, int Index> _unmanagedStructs;
Dictionary<ITypeSymbol, int Index> _managedStructs;
Dictionary<ITupleTypeSymbol, int> _managedTuples;
enum FieldValueSource
{
    ObjectInstances(int Index),
    UnmanagedUnion(string? VariantFieldName), // Ϊnullʱ��ʹ��Unsafeֱ��ת����
    ManagedStruct(string FieldName),
    ManagedTuple(string FieldName, string? TupleItemAccess), // �����ֶδ�managed tuple ����ʱ��TupleItemAccess�����塣ͨ��$"this.{FieldName}.{TupleItemAccess}"�����ֶ�
}

enum FieldType
{
    RefType(Type Type),
    Unmanaged(Type Type, int Size),
    RefOnlyManagedStruct(Type type, int RefCount),
    ManagedStruct(Type type),
    ManagedValueTuple(Type[] Types),
}
```
һ����ȡManagedValueTuple�ֶζ��������ķ�����
TupleTree: for (int, (int, string))
  - int
  - (int, string)
    - int
    - string

```
List<TupleTree> tuples // ����ֶι���
List<bool> marks // ÿ��variant��һ������ֶΣ�������ʱ���tuple�Ƿ��Ѿ�������
// ��ȡVaraint������tuple���ͣ�����tuple tree��Ƚ���
// ����Ӧ���ȴ�����ȸ�������������ܵ���ȸ�С������������field����
foreach (var tuple in Variant.OfManagedValueTuple().SortDesc(tupleTree => tupleTree.Depth)):
    if (tuples.OfNotMarked().Any(tpl.IsSubTreeOf(tuple))
        tuples[tpl] = tuple; // �������tuple����tuple���Ӵ����滻��tuple
        marks[tpl] = true; // ��Ǹ�tuple�ѱ�ռ��
        �˴���Ӧ���ԭtuple����tuple�е�λ��
    else (tuples.OfNotMarked().Any(tpl.isSuperTreeOf(tuple)))
        marks[tpl] = true;
        // �������tuple������tuple���Ǿ�����
    // һ�ִ���󣬾��ǰ�variant�е�tuples�����ԭ��û�У��Ǿͼ��룬����У��Ǿͼ�Ϊԭ����
```
