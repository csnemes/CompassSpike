using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompassCore.Model.Internal
{
    public class UniqueIdDerivator
    {
        public static UniqueIdDerivator Instance = new UniqueIdDerivator();

        public string GetUniqueId(SyntaxNode node)
        {
            if (node is ClassDeclarationSyntax)
            {
                return GetUniqueId((ClassDeclarationSyntax)node);
            }

            throw new ApplicationException(String.Format("Unknown node type {0}", node.GetType()));
        }

        public string GetUniqueId(ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol)
            {
                return GetUniqueId((INamedTypeSymbol)symbol);
            }

            throw new ApplicationException(String.Format("Unknown symbol type {0}", symbol.GetType()));
        }

        private string GetUniqueId(INamedTypeSymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        private string GetUniqueId(ClassDeclarationSyntax node)
        {
            throw new NotImplementedException();
        }
    }
}
