// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
{ }

[ScribanSourceGeneretor.ClassMember("""
    {{ $x = ["a","abc","ABC","xyz"] -}}
    {{- for $i in 0..<$x.size ~}}
        public const string X{{ $i }} = "{{ $x[$i] }}";
    {{ end }}
    """)]
public partial class A
{

}