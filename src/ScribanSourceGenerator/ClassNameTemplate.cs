using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ScribanSourceGenerator;

public record ClassNameTemplate(TypeDeclarationSyntax Type, string[] Templates)
{
    public static ClassNameTemplate? Create(SemanticModel semanticModel, TypeDeclarationSyntax type)
    {
        var args = GetAttribute(semanticModel, type, "ScribanSourceGeneretor.ClassMemberAttribute").ToArray();
        if (args.Length == 0) return null;
        return new(type, args);
    }

    private static IEnumerable<string> GetAttribute(SemanticModel semanticModel, MemberDeclarationSyntax m, string attributeFullName)
    {
        foreach (var list in m.AttributeLists)
        {
            foreach (var a in list.Attributes)
            {
                if (semanticModel.GetSymbolInfo(a).Symbol is { ContainingType: var t }
                    && t.ToDisplayString() == attributeFullName)
                {
                    if (a.ArgumentList is not { } args) continue;
                    if (args.Arguments.FirstOrDefault(arg => arg.NameEquals is null) is not { } arg) continue;

                    var template = (string)semanticModel.GetConstantValue(arg.Expression).Value!;
                    yield return template;
                }
            }
        }
    }
}
