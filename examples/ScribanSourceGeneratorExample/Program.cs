// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
{ }

[ScribanSourceGenerator.ClassMember("""
    {{ $x = ["a","abc","ABC","xyz"] -}}
    {{- for $i in 0..<$x.size ~}}
        public const string X{{ $i }} = "{{ $x[$i] }}";
    {{ end ~}}
    // eof
    """)]
public partial class A
{

}
