using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Text;

namespace ScribanSourceGenerator;

public class SyntaxNodeHelper
{
    /// <summary>
    /// Gets the hint path - <see cref="AppendMemberIndex"/> are appended to the path of the node.
    /// A drive letter is removed.
    /// </summary>
    /// <example>
    /// [node path]  [indexes] [hint path]
    /// name.cs             1        scriban/name-1.cs
    /// folder/sub/name.cs  1, 2, 3  scriban/folder/sub/name-1-2-3.cs
    /// C:/folder/name.cs   0, 0     scriban/folder/name-0-0.cs
    /// </example>
    public static string GetHintPath(MemberDeclarationSyntax node, StringBuilder buffer)
    {
        buffer.Clear();

        // I want to use AsSpan() if ending support for netstandard2.0.
        var path = node.GetLocation().GetMappedLineSpan().Path;

        var start = 0;
        var end = 0;

        // remove drive letter.
        if (path.IndexOf(':') is var i and >= 0) start = i + 1;

        // remove / at start position.
        if (start < path.Length && path[start] is '/' or '\\') start++;

        // remove extension.
        if (path.EndsWith(".cs")) end = 3;

        // "scriban/" + path without extension + indexes + ".cs"
        buffer.Append("scriban/");
        path = path.Substring(start, path.Length - end);
        buffer.Append(path);
        AppendMemberIndex(node, buffer);
        buffer.Append(".cs");

        return buffer.ToString();
    }

    /// <summary>
    /// Appends the index of the member in the parent.Members recursively.
    /// </summary>
    /// <example>
    /// using A;
    /// using B;
    /// 
    /// partial class X;  // 0
    /// partial class X;  // 1
    /// partial struct Y; // 2
    /// 
    /// namespace C // 3
    /// {
    ///     class Y; // 3-0
    /// }
    /// 
    /// namespace C // 4
    /// {
    ///     class Z // 4-0
    ///     {
    ///         public partial class A;  // 4-0-0
    ///         public partial struct B; // 4-0-1
    ///         public partial class A;  // 4-0-2
    ///     }
    /// }
    /// </example>
    private static void AppendMemberIndex(MemberDeclarationSyntax node, StringBuilder buffer)
    {
        var p = node.Parent;

        if (p is MemberDeclarationSyntax md) AppendMemberIndex(md, buffer);

        var members = p switch
        {
            CompilationUnitSyntax u => u.Members,
            TypeDeclarationSyntax t => t.Members,
            BaseNamespaceDeclarationSyntax n => n.Members,
            _ => [],
        };

        buffer.Append('-');
        buffer.Append(members.IndexOf(node));
    }

    public static int AppendDeclarations(StringBuilder sb, SyntaxNode? node)
    {
        if (node is null) return 0;

        var nest = AppendDeclarations(sb, node.Parent);

        switch (node)
        {
            case CompilationUnitSyntax c:
                AppendUsings(sb, c.Usings);
                return nest;
            case BaseNamespaceDeclarationSyntax ns:
                AppendNamespaceOpen(sb, ns);
                AppendUsings(sb, ns.Usings);
                return nest + 1;
            case TypeDeclarationSyntax t:
                AppendTypeOpen(sb, t);
                return nest + 1;
            default:
                return nest;
        }
    }

    private static void AppendTypeOpen(StringBuilder sb, TypeDeclarationSyntax t)
    {
        sb.Append("partial ");
        sb.Append(t.Keyword.Text);
        sb.Append(' ');

        if (t is RecordDeclarationSyntax { ClassOrStructKeyword: var k } && k != default)
        {
            sb.Append(k.Text);
            sb.Append(' ');
        }

        sb.Append(t.Identifier.Text);

        if (t.TypeParameterList is { } tl)
        {
            sb.Append('<');
            var first = true;
            foreach (var tp in tl.Parameters)
            {
                if (first) first = false;
                else sb.Append(", ");
                sb.Append(tp.Identifier.Text);
            }
            sb.Append('>');
        }
        sb.Append("""
             {

            """);
    }

    private static void AppendNamespaceOpen(StringBuilder sb, BaseNamespaceDeclarationSyntax ns)
    {
        sb.Append($$"""
        namespace {{ns.Name}} {

        """);
    }

    private static void AppendUsings(StringBuilder sb, SyntaxList<UsingDirectiveSyntax> usings)
    {
        foreach (var u in usings)
        {
            sb.Append(u.WithLeadingTrivia().ToFullString());
        }
    }

    public static void AppendClose(StringBuilder sb, int nest)
    {
        sb.Append('\n');

        for (int i = 0; i < nest; i++)
        {
            sb.Append('}');
        }
        sb.Append('\n');
    }
}
