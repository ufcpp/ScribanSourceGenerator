using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Text;

namespace ScribanSourceGenerator;

public class SyntaxNodeHelper
{
    public static string AppendName(StringBuilder buffer, SyntaxNode node)
    {
        buffer.Clear();

        appendTypeName(buffer, node);

        // file name + line number
        var span = node.GetLocation().GetMappedLineSpan();
        var filename = Path.GetFileNameWithoutExtension(span.Path);
        buffer.Append($"[{filename}]{span.StartLinePosition.Line}.cs");

        return buffer.ToString();

        static bool appendTypeName(StringBuilder sb, SyntaxNode? node)
        {
            if (node is BaseNamespaceDeclarationSyntax ns)
            {
                if (appendTypeName(sb, node.Parent)) sb.Append('_');

                sb.Append($"{ns.Name}");

                return true;
            }
            else if (node is TypeDeclarationSyntax t)
            {
                if (appendTypeName(sb, node.Parent)) sb.Append('_');

                sb.Append($"{t.Identifier}");

                if (t.TypeParameterList is { } tl)
                {
                    foreach (var t1 in tl.Parameters)
                    {
                        sb.Append($"`{t1.Identifier}");
                    }
                }

                return true;
            }

            return false;
        }

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
        for (int i = 0; i < nest; i++)
        {
            sb.Append('}');
        }
        sb.Append("""


            """);
    }
}
