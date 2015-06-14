using CompassCore.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CompassCore.Parsing
{
    public class SolutionParser
    {
        private readonly Solution _solution;
        private GraphSpace _nodeSpace;

        private SolutionParser(Solution solution)
        {
            _solution = solution;
        }

        public static SolutionParser ParseSolution(string path)
        {
            var ws = MSBuildWorkspace.Create();
            var solution = ws.OpenSolutionAsync(path).Result;
            var result = new SolutionParser(solution);
            result.RunParsing(); //TODO async
            return result;
        }

        public IEnumerable<Vertex> GetVerticiesOfType(VertexType type)
        {
            return _nodeSpace.GetVerticesByProperty(PropertyKeys.Type, type);
        }

        private void RunParsing()
        {
            _nodeSpace = DoParse().Result;
        }

        private async Task<GraphSpace> DoParse()
        {
            var result = new GraphSpace();
            var solutionVertex = result.AddOrUpdateVertex(VertexDefinition.Create(_solution.Id.Id.ToString())
                .AddProp(PropertyKeys.Type, VertexType.Solution)
                .AddProp(PropertyKeys.Name, _solution.FilePath));

            foreach (var project in _solution.Projects)
            {
                Debug.Print("Parsing project: {0}", project.Name);
                var projectVertex = result.AddOrUpdateVertex(VertexDefinition.Create(project.Id.Id.ToString())
                .AddProp(PropertyKeys.Type, VertexType.Project)
                .AddProp(PropertyKeys.Name, project.Name));

                result.AddOrUpdateEdge(EdgeDefinition.Create("CONSISTSOF")
                    .FromVertex(solutionVertex).ToVertex(projectVertex));

                result.AddOrUpdateEdge(EdgeDefinition.Create("INCLUDEDIN")
                    .FromVertex(projectVertex).ToVertex(solutionVertex));

                foreach (var document in project.Documents)
                {
                    Debug.Print("Parsing document: {0}", document.Name);
                    if (document.SupportsSyntaxTree)
                    {
                        var tree = await document.GetSyntaxTreeAsync();
                        var semanticModel = await document.GetSemanticModelAsync();

                        var rootNode = await tree.GetRootAsync();

                        var walker = new CustomWalker(result, semanticModel, projectVertex);
                        walker.Visit(rootNode);
                    }
                }
            }
            return result;
        }
    }
}
