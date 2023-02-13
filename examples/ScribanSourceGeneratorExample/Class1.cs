using System;

namespace ScribanSourceGeneratorExample
{
    using System.Text;

    [ScribanSourceGenerator.ClassMember("""
        {{ $x = ["a","abc","ABC","xyz"] -}}
        {{- for $i in 0..<$x.size ~}}
            public const string X{{ $i }} = "{{ $x[$i] }}";
        {{ end }}
        """)]
    [ScribanSourceGenerator.ClassMember("""
        {{- for $i in 0..5 ~}}
            public const int Y{{ $i }} = {{ $i }};
        {{ end }}
        """)]
    internal partial class ClassA
    {
    }

    [ScribanSourceGenerator.ClassMember("")]
    public partial class ClassB
    {
        [ScribanSourceGenerator.ClassMember("")]
        protected partial class Inner
        {
        }
    }

    [ScribanSourceGenerator.ClassMember("")]
    public partial struct StructA { }

    [ScribanSourceGenerator.ClassMember("")]
    public partial record R1 { }

    [ScribanSourceGenerator.ClassMember("")]
    public partial record class R2 { }

    [ScribanSourceGenerator.ClassMember("")]
    public partial record struct R3 { }

    [ScribanSourceGenerator.ClassMember("")]
    public partial record struct R<T1, T2>
        where T1 : struct
        where T2 : notnull
    { }
}
