# Trarizon.Library.TaggedUnion

A source generator to generate tagged union. All generated types are partial.

Does not support generic and ref struct temporarily.

Requires [`System.Runtime.CompilerServices.Unsafe`](https://www.nuget.org/packages/System.Runtime.CompilerServices.Unsafe) for .Net Standard 2.0

## Easy guide

For code:
``` csharp
[UnionTag]
public enum SomeEnum
{
    A,
    [TagVariant<int, string>("Id", "Name")]
    B,
    [TagVariant<int, DateTime, SomeManagedStruct>]
    C,
}
```

will generate union like:

```
public partial struct SomeEnumUnion
{
    public SomeEnum Tag { get; }

    public SomeEnumUnion New_A();
    public bool TryGet_A();

    public SomeEnumUnion New_B(int Id, string Name);
    public bool TryGet_B(out int Id, out string Name);

    public SomeEnumUnion New_C(int Item1, DateTime Item2, SomeManagedStruct Item3);
    public bool TryGet_A(out int Item1, out DateTime Item2, out SomeManagedStruct Item3);
}
```

<details>
<summary>Actual generated code(simplified)</summary>

``` csharp
public partial struct SomeEnumUnion
{
    private object __obj0;
    private readonly SomeEnum __tag;
    private __Unmanageds __unmanageds;
    private SomeManagedStruct __managed0_0;
    
    private struct __Unmanageds
    {
        [FieldOffset(0)] int __0;
        [FieldOffset(0)] (int, DateTime) __1;
    }

    private SomeEnumUnion(SomeEnum tag) => __tag = tag;

#region A

    public static SomeEnumUnion New_A() => new(SomeEnum.A);_

    public bool TryGet_A()
    {
        if (this.__tag == SomeEnum.A)
            return true;
        else
            return false;
    }

#endregion

#region B

    public static SomeEnumUnion New_B(int Id, string Name)
    {
        var __res = new(SomeEnum.B);
        ref int __unmanaged_local = ref Unsafe.As<__Unmanageds, int>(ref this.__unmanageds);
        __unmanageds = Id;
        __res.__obj0 = Name;
        return __res;
    }

    public bool TryGet_B(out int Id, out string Name)
    {
        if (this.__tag == SomeEnum.C) {
            ref int __unmanaged_local = ref Unsafe.As<__Unmanageds, int>(ref this.__unmanageds);
            Id = __unmanaged_local;
            Name = Unsafe.As<string>(this.__obj0);
            return true;
        }
        else {
            // set parameters to default
            return false;
        }
    }

#endregion

#region C

    public static SomeEnumUnion New_C(int Item1, DateTime Item2, SomeManagedStruct Item3)
    {
        var __res = new(SomeEnum.C);
        ref (int, DateTime) __unmanaged_local = ref Unsafe.As<__Unmanageds, (int, DateTime)>(ref this.__unmanageds);
        __unmanaged_local.Item1 = Item1;
        __unmanaged_local.Item2 = Item2;
        __res.__managed0_0 = Item3;
        return __res;
    }

    public bool TryGet_C(out int Item1, out DateTime Item2, SomeManagedStruct Item3)
    {
        if (this.__tag == SomeEnum.C) {
            ref (int, DateTime) __unmanaged_local = ref Unsafe.As<__Unmanageds, (int, DateTime)>(ref this.__unmanageds);
            Item1 = __unmanaged_local.Item1;
            Item2 = __unmanaged_local.Item2;
            Item3 = this.__managed0_0;
            return true;
        }
        else {
            // set parameters to default
            return false;
        }
    }

#endregion
}
```

</details>

## Options

Can optional generate More methods:
- RefView : Requires .NET7+. Provide properties returns a view of this union
- IDisposable : Implement IDisposable, auto dispose if field implements IDisposable
- Match : Implements match method

<details>
<summary>RefView samples</summary>

For `SomeEnum.C`, generates

``` csharp
[UnscopedRef] public C_Ref => new(ref this);

public readonly ref partial struct C_View(ref SomeEnumUnion union)
{
    private readonly ref SomeEnumUnion _ref = union;

    public ref int Item1 => ref Unsafe.As<__Unmanageds, (int, DateTime)>(ref this.__ref.__unmanageds).Item1;

    public ref DateTime Item2 => ref Unsafe.As<__Unmanageds, (int, DateTime)>(ref this.__ref.__unmanageds).Item2;
        
    public ref SomeManagedStruct Item3 => this.__ref.__managed0_0;

    public void Deconstruct(out int Item1, out DateTime Item2, out SomeManagedStruct Item3)
    {
        Item1 = this.Item1;
        Item2 = this.Item2;
    }
}
```
you can use `union is { Tag: SomeEnum.C, C_Ref: (var item1, var item2, var item3) }` to get items.

</details>
