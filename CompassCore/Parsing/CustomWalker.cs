using CompassCore.Model;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using System;

namespace CompassCore.Parsing
{
    public class CustomWalker : CSharpSyntaxWalker
    {
        private readonly GraphSpace _nodeSpace;
        private readonly SemanticModel _semanticModel;

        public CustomWalker(GraphSpace nodeSpace, SemanticModel semanticModel)
        {
            _nodeSpace = nodeSpace;
            _semanticModel = semanticModel;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            Debug.Print(node.Identifier.Text);

            var symbol = _semanticModel.GetDeclaredSymbol(node);

            var classVertexDef = VertexDefinition.FromSemanticNode(symbol)
                .AddProp(PropertyKeys.Name, symbol.Name)
                .AddProp(PropertyKeys.OriginalNode, node)
                .AddProp(PropertyKeys.Type, VertexType.Class);

            var classVertex = _nodeSpace.AddOrUpdateVertex(classVertexDef);

            var parentClassVertexDef = VertexDefinition.FromSemanticNode(symbol.BaseType)
                .AddProp(PropertyKeys.Name, symbol.BaseType.Name);

            var parentClassVertex =_nodeSpace.AddOrUpdateVertex(parentClassVertexDef);

            _nodeSpace.AddOrUpdateEdge(EdgeDefinition.Create(EdgeKeys.Child).FromVertex(parentClassVertex).ToVertex(classVertex));
            _nodeSpace.AddOrUpdateEdge(EdgeDefinition.Create(EdgeKeys.Parent).FromVertex(classVertex).ToVertex(parentClassVertex));

            AddInterfaceVertexesAndEdges(symbol, classVertex);
            AddNamespaceVertexesAndEdges(symbol, classVertex);

            base.VisitClassDeclaration(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            Debug.Print(node.Identifier.Text);

            var symbol = _semanticModel.GetDeclaredSymbol(node);

            var interfaceVertexDef = VertexDefinition.FromSemanticNode(symbol)
                .AddProp(PropertyKeys.Name, symbol.Name)
                .AddProp(PropertyKeys.OriginalNode, node)
                .AddProp(PropertyKeys.Type, VertexType.Interface);

            var interfaceVertex = _nodeSpace.AddOrUpdateVertex(interfaceVertexDef);

            AddNamespaceVertexesAndEdges(symbol, interfaceVertex);

            base.VisitInterfaceDeclaration(node);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            Debug.Print(node.Name.ToString());

            var symbol = _semanticModel.GetDeclaredSymbol(node);

            var namespaceVertexDef = VertexDefinition.FromSemanticNode(symbol)
                .AddProp(PropertyKeys.Name, symbol.Name)
                .AddProp(PropertyKeys.OriginalNode, node)
                .AddProp(PropertyKeys.Type, VertexType.NameSpace);

            _nodeSpace.AddOrUpdateVertex(namespaceVertexDef);

            base.VisitNamespaceDeclaration(node);
        }

        private void AddNamespaceVertexesAndEdges(INamedTypeSymbol symbol, VertexRef vertex)
        {
            var namespaceSymbol = symbol.ContainingNamespace;

            var namespaceVertexDef = VertexDefinition.FromSemanticNode(namespaceSymbol)
                .AddProp(PropertyKeys.Name, namespaceSymbol.Name);

            var namespaceVertex = _nodeSpace.AddOrUpdateVertex(namespaceVertexDef);

            _nodeSpace.AddOrUpdateEdge(EdgeDefinition.Create(EdgeKeys.ContainedBy).FromVertex(vertex).ToVertex(namespaceVertex));
            _nodeSpace.AddOrUpdateEdge(EdgeDefinition.Create(EdgeKeys.Contain).FromVertex(namespaceVertex).ToVertex(vertex));
        }

        private void AddInterfaceVertexesAndEdges(INamedTypeSymbol symbol, VertexRef vertex)
        {
            foreach (var interfaceSymbol in symbol.Interfaces)
            {
                var interfaceVertexDef = VertexDefinition.FromSemanticNode(interfaceSymbol)
                    .AddProp(PropertyKeys.Name, symbol.Name);

                var interfaceVertex = _nodeSpace.AddOrUpdateVertex(interfaceVertexDef);

                _nodeSpace.AddOrUpdateEdge(EdgeDefinition.Create(EdgeKeys.Implementation).FromVertex(vertex).ToVertex(interfaceVertex));
                _nodeSpace.AddOrUpdateEdge(EdgeDefinition.Create(EdgeKeys.Definition).FromVertex(interfaceVertex).ToVertex(vertex));
            }
        }
    }
}
