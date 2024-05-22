提供两种定义方法
- [UnionTag] enum：标记该enum为tag类型，在同namespace下生成union type
  - [TagVariant] enum field: 标记variant
  - !受C#语法限制，该方式无法使用泛型
- [TaggedUnion] struct: 标记为taggedunion类型，在同namespace下生成union tag enum
  - 编写一个static partial method，以参数定义taggedunion，使用ValueTuple<>定义单个或多个参数。该方法会被运行时删掉


Options
- UnionTypeKind: 默认Struct，有任何一个字段为ref struct时为RefStruct
  - Struct: 默认的TaggedUnion，struct始终有可能为default，因此tag enum不应提供有参的0值
  - Class: 使用class和type pattern match check
  - Record: 使用Record与type pattern match check
  - RefStruct：一个tag和一个ref byte  // 安全性存疑，暂定
- Pack: 用于指定[StructLayout]的Pack
- SequentialValueTypeLayout: 表示不同类型的struct不会共享内存中的位置
  - 考虑一下哪个作为首选项
  - 基本类型不会更改，所以可以保持共享
- CreatorAccessibility: Create方法或ctor的accessibility
  - 对Record无效
- AllowFieldAccess: bool: 对Struct有效，表示允许用户直接访问fields，这是unsafe操作 [暂定]
- // AggressiveMinimizingSize:试图不存储tag并通过字段获取tag
  - 因为获取tag时性能问题似乎不小所以取消了
  - 但是TryGetXXX里使用或许可以，暂定
- GenerationOption:
  - IDisposable: 如果有实现IDisposable的字段，实现
  - Match: 实现 Match(Action<> matcher) 和 Match<>(Func<> matcher, ...)

```
partial struct Union
{
    // 该结构内部可能存在大量Unsafe的cast操作，因此用户不应当访问这里任何一个字段，除了tag。
    // 如果对structlayout抱有疑惑，可以使用tuple包裹来实现
    // 如果使用tuple包裹，就可以和下文中的tuple采取同样的处理
    object __obj0;
    object __obj1;
    ...
    
    readonly UnionTag __tag;
    
    // 考虑外部库修改的话，这里就只有enum和基本类型能用了
    // 不考虑的话可以把所有unmanaged扔这里
    __UnmanagedUnion __unmanageds;
    
    // 外部库可能会修改struct结构，所以即使字段全是引用类型，managed struct也不能直接映射到objects
    // 不考虑的话对于纯引用类型的struct可以和objects重合
    ManagedA __managed0;
    ...
    // PS: unmanged generic constraint并不在运行时检查，也就是说如果偷偷替换dll的话是可以将managed struct
    // 传入unmanaged constraint的，此时简单运行似乎也不报错
    // 因此考虑忽略外部库修改的可能 [暂定]
    
    
    // ! 这部分实现会很复杂，可以考虑
    // 考虑valuetuple应该不会修改，当field为valuetuple时，我们或许可以从其中节省部分字段
    // 对于ref type和 unmanaged type，优先从这里寻找可分配空间
    // 我们不将这里tuple字段定义中的reference type转为object，仍然使用Unsafe在获取时进行转换
    (ValueTuple) __vtuple0;
    ...
    
    #pragma warning disable CS8618
    private Union(UnionTag tag) => __tag = tag;
    
    public UnionTag Tag => __tag;
    
    [UnscopedRef]
    public VariantVariant Variant => new VariantVariant(ref this);
    
    // accessibility可设置
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

    // 如果只有基本类型，我们可以直接计算出这个结构的尺寸。否则需要FieldOffset一下
    struct __UnmanagedUnion { } // 对齐所有Unmanaged struct
    
    // 没有字段时不生成这个结构
    ref struct VariantVariant(ref Union union)
    {
        private ref Union _unionRef = ref union;
        
        public ref Field => _unionRef._field;
        
        // 异步和迭代器中不能使用ref struct，因此提供Deconstruct很有用
        // ~~但是只有一个field时没用，此时不生成，用Field属性获取~~
        // 只有一个field也生成，如果需要用户自己调用
        public void Deconstruct(out field) => field = _unionRef._field;
    }
}
```
- 用户始终可以创建default(Union)，因此我们添加一个Diagnostic(INFO)，提示用户添加一个值为0的无字段enum值
对标记FlagAttr的enum报Diagnostic(INFO)

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

 整体设计和class version基本一样
```
abstract record Union
{
    sealed record Variant(fields) : Union {}
}
```

RefStruct
[tag|ref byte|ref byte|..]


 Struct的详细设计
```
int _maxRefTypeCount;
INamedTypeSymbol _tagSymbol;
//dict 使用草稿见Program.Partial.Do()
//managed struct的处理和unmanaged应该是基本一致的。但是managed struct不能获取tuple的sub tuple
//unmanaged的设计主要在于减少字段定义，对union size没有影响
Dictionary<ITypeSymbol, int Index> _unmanagedStructs;
Dictionary<ITypeSymbol, int Index> _managedStructs;
Dictionary<ITupleTypeSymbol, int> _managedTuples;
enum FieldValueSource
{
    ObjectInstances(int Index),
    UnmanagedUnion(string? VariantFieldName), // 为null时，使用Unsafe直接转换，
    ManagedStruct(string FieldName),
    ManagedTuple(string FieldName, string? TupleItemAccess), // 其他字段从managed tuple 借用时，TupleItemAccess有意义。通过$"this.{FieldName}.{TupleItemAccess}"访问字段
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
一个获取ManagedValueTuple字段定义数量的方法：
TupleTree: for (int, (int, string))
  - int
  - (int, string)
    - int
    - string

```
List<TupleTree> tuples // 这个字段共用
List<bool> marks // 每个variant有一个这个字段，用于临时标记tuple是否已经被用了
// 获取Varaint中所有tuple类型，并按tuple tree深度降序，
// 我们应该先处理深度更大的数，将可能的深度更小的数留给其他field类型
foreach (var tuple in Variant.OfManagedValueTuple().SortDesc(tupleTree => tupleTree.Depth)):
    if (tuples.OfNotMarked().Any(tpl.IsSubTreeOf(tuple))
        tuples[tpl] = tuple; // 如果已有tuple是新tuple的子串，替换该tuple
        marks[tpl] = true; // 标记该tuple已被占用
        此处还应标记原tuple在新tuple中的位置
    else (tuples.OfNotMarked().Any(tpl.isSuperTreeOf(tuple)))
        marks[tpl] = true;
        // 如果已有tuple包含新tuple，那就这样
    // 一轮处理后，就是把variant中的tuples，如果原来没有，那就加入，如果有，那就记为原来的
```
