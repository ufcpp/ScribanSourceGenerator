using System;

namespace ScribanSourceGeneratorExample
{
    using System.Text;

    [ScribanSourceGeneretor.ClassMember("""
        {{ $x = ["a","abc","ABC","xyz"] -}}
        {{- for $i in 0..<$x.size ~}}
            public const string X{{ $i }} = "{{ $x[$i] }}";
        {{ end }}
        """)]
    [ScribanSourceGeneretor.ClassMember("""
        {{- for $i in 0..5 ~}}
            public const int Y{{ $i }} = {{ $i }};
        {{ end }}
        """)]
    internal partial class ClassA
    {
    }

    [ScribanSourceGeneretor.ClassMember("")]
    public partial class ClassB
    {
        [ScribanSourceGeneretor.ClassMember("")]
        protected partial class Inner
        {
        }
    }

    [ScribanSourceGeneretor.ClassMember("")]
    public partial struct StructA { }

    [ScribanSourceGeneretor.ClassMember("")]
    public partial record R1 { }

    [ScribanSourceGeneretor.ClassMember("")]
    public partial record class R2 { }

    [ScribanSourceGeneretor.ClassMember("")]
    public partial record struct R3 { }

    [ScribanSourceGeneretor.ClassMember("")]
    public partial record struct R<T1, T2>
        where T1 : struct
        where T2 : notnull
    { }
}
