﻿using CompassCore.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            var solution = ws.OpenSolutionAsync(path);
            var result = new SolutionParser(solution.Result);
            result.RunParsing(); //TODO async
            return result;
        }

        private void RunParsing()
        {
            _nodeSpace = DoParse().Result;
        }

        private async Task<GraphSpace> DoParse()
        {
            var result = new GraphSpace();
            foreach (var project in _solution.Projects)
            {
                Debug.Print("Parsing project: {0}", project.Name);
                foreach (var document in project.Documents)
                {
                    Debug.Print("Parsing document: {0}", document.Name);
                    if (document.SupportsSyntaxTree)
                    {
                        var tree = await document.GetSyntaxTreeAsync();
                        var semanticModel = await document.GetSemanticModelAsync();

                        var rootNode = await tree.GetRootAsync();

                        var walker = new CustomWalker(_nodeSpace, semanticModel);
                        walker.Visit(rootNode);
                    }
                }
            }
            return result;
        }
    }
}
