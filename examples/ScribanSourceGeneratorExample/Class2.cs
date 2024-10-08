namespace ScribanSourceGeneratorExample;

[ScribanSourceGenerator.ClassMember("""
        {{ $x = ["a","abc","ABC","xyz"] -}}
        {{- for $i in 0..<$x.size ~}}
            public const string X{{ $i }} = "{{ $x[$i] }}";
        {{ end }}
        """)]
internal partial struct StructB;
