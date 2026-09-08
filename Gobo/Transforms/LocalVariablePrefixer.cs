using Gobo.SyntaxNodes;
using Gobo.SyntaxNodes.Gml;

namespace Gobo.Transforms;

/// <summary>
/// Gives local variables and parameters the leading underscore that marks them as local.
/// Runs before printing, so the output and the tree it is validated against agree.
/// </summary>
internal static class LocalVariablePrefixer
{
    public static void Run(GmlSyntaxNode ast, FormatOptions options)
    {
        var root = new Scope();
        Collect(ast, root, options);
        Apply(root);
    }

    private sealed class Scope
    {
        public List<Scope> Children { get; } = new();
        public Dictionary<string, List<Identifier>> Declarations { get; } = new();
        public List<Identifier> References { get; } = new();
        public HashSet<string> NamesInUse { get; } = new();

        public HashSet<string> Blocked { get; } = new();
    }

    private static void Collect(GmlSyntaxNode node, Scope scope, FormatOptions options)
    {
        if (node is FunctionDeclaration function)
        {
            var inner = new Scope();
            scope.Children.Add(inner);

            foreach (var parameter in function.Parameters.Children)
            {
                if (parameter is Parameter { Name: Identifier name })
                {
                    Declare(inner, name);
                }
            }

            Collect(function.Parameters, inner, options);

            // A parent constructor call reads this constructor's parameters.
            Collect(function.ConstructorParent, inner, options);

            Collect(function.Body, inner, options);
            return;
        }

        if (node is VariableDeclarationList declarationList && IsPrefixable(declarationList, options))
        {
            foreach (var declaration in declarationList.Declarations)
            {
                if (declaration is not VariableDeclarator { Id: Identifier id } declarator)
                {
                    continue;
                }

                // A static function is a method on the constructor's struct, callable by name
                // from anywhere, so renaming it would break its callers.
                if (
                    declarationList.Modifier == "static"
                    && declarator.Initializer is FunctionDeclaration
                )
                {
                    continue;
                }

                Declare(scope, id);
            }
        }

        if (node is Identifier identifier)
        {
            scope.NamesInUse.Add(identifier.Name);

            if (IsShorthandStructKey(identifier))
            {
                // '{ value }' names the key after the variable.
                scope.Blocked.Add(identifier.Name);
            }
            else if (IsRenameableReference(identifier))
            {
                scope.References.Add(identifier);
            }
        }

        foreach (var child in node.Children)
        {
            Collect(child, scope, options);
        }
    }

    private static bool IsPrefixable(VariableDeclarationList declarationList, FormatOptions options)
    {
        return declarationList.Modifier switch
        {
            "var" => options.PrefixLocalVariables,
            "static" => options.PrefixStaticVariables,
            _ => false
        };
    }

    private static void Declare(Scope scope, Identifier id)
    {
        if (!scope.Declarations.TryGetValue(id.Name, out var declarations))
        {
            declarations = new List<Identifier>();
            scope.Declarations[id.Name] = declarations;
        }

        declarations.Add(id);
    }

    private static void Apply(Scope scope)
    {
        var namesInUse = CollectNamesInUse(scope);
        var blocked = CollectBlockedNames(scope);
        var renames = new Dictionary<string, Rename>();

        foreach (var (name, declarations) in scope.Declarations)
        {
            var prefix = GetMissingPrefix(name, declarations);

            if (prefix is null || blocked.Contains(name))
            {
                continue;
            }

            var newName = prefix + name;

            if (namesInUse.Contains(newName))
            {
                continue;
            }

            renames[name] = new Rename(newName, declarations.Min(d => d.Span.Start));
        }

        if (renames.Count > 0)
        {
            ApplyRenames(scope, renames);
        }

        foreach (var child in scope.Children)
        {
            Apply(child);
        }
    }

    private static string? GetMissingPrefix(string name, List<Identifier> declarations)
    {
        if (name.StartsWith('_'))
        {
            return null;
        }

        return declarations[0].Parent is VariableDeclarator { Parent: VariableDeclarationList { Modifier: "static" } }
            ? "__"
            : "_";
    }

    /// <summary>
    /// A rename, and the position of the declaration that asked for it. Anything written
    /// earlier is a different variable.
    /// </summary>
    private readonly record struct Rename(string NewName, int DeclaredAt);

    private static void ApplyRenames(Scope scope, Dictionary<string, Rename> renames)
    {
        foreach (var (name, declarations) in scope.Declarations)
        {
            if (renames.TryGetValue(name, out var rename))
            {
                declarations.ForEach(declaration => declaration.Name = rename.NewName);
            }
        }

        RenameReferences(scope, renames);
    }

    private static void RenameReferences(Scope scope, Dictionary<string, Rename> renames)
    {
        foreach (var reference in scope.References)
        {
            if (
                renames.TryGetValue(reference.Name, out var rename)
                && reference.Span.Start >= rename.DeclaredAt
            )
            {
                reference.Name = rename.NewName;
            }
        }

        foreach (var child in scope.Children)
        {
            RenameReferences(child, renames);
        }
    }

    private static HashSet<string> CollectNamesInUse(Scope scope)
    {
        var result = new HashSet<string>(scope.NamesInUse);

        foreach (var child in scope.Children)
        {
            result.UnionWith(CollectNamesInUse(child));
        }

        return result;
    }

    /// <summary>
    /// Names redeclared in a nested function, plus the blocked names of every scope below.
    /// </summary>
    private static HashSet<string> CollectBlockedNames(Scope scope)
    {
        var result = new HashSet<string>(scope.Blocked);

        foreach (var child in scope.Children)
        {
            result.UnionWith(child.Declarations.Keys);
            result.UnionWith(CollectBlockedNames(child));
        }

        return result;
    }

    private static bool IsShorthandStructKey(Identifier identifier)
    {
        return identifier.Parent is StructProperty { Initializer.IsEmpty: true } property
            && property.Name == identifier;
    }

    private static bool IsRenameableReference(Identifier identifier)
    {
        switch (identifier.Parent)
        {
            case MemberDotExpression member:
                return member.Property != identifier;
            case StructProperty property:
                return property.Name != identifier;
            case FunctionDeclaration function:
                return function.Id != identifier;
            case ConstructorClause clause:
                return clause.Id != identifier;
            case EnumMember:
            case EnumDeclaration:
            case MacroDeclaration:
                return false;
            default:
                return true;
        }
    }
}
