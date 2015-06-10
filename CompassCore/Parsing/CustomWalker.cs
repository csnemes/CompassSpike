using CompassCore.Model;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

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

            var simfo = _semanticModel.GetDeclaredSymbol(node);

            var vertexDef = VertexDefinition.FromSemanticNode(simfo)
                .AddProp(Keys.Name, simfo.Name)
                .AddProp(Keys.OriginalNode, node);

            var vertex = _nodeSpace.AddOrUpdateVertex(vertexDef);

            var parentVertexDef = VertexDefinition.FromSemanticNode(simfo.BaseType)
                .AddProp(Keys.Name, simfo.BaseType.Name);

            var parentVertex =_nodeSpace.AddOrUpdateVertex(parentVertexDef);

            _nodeSpace.AddOrUpdateEdge(EdgeDefinition.Create("Child").FromVertex(parentVertex).ToVertex(vertex));
            _nodeSpace.AddOrUpdateEdge(EdgeDefinition.Create("Parent").FromVertex(vertex).ToVertex(parentVertex));
        }
    }
}
