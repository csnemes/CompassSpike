using CompassCore.Parsing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompassCore.Tests
{
    [TestFixture]
    public class GeneralTests
    {
        [Test, Explicit]
        public void TestSpike()
        {
            var ws = MSBuildWorkspace.Create();
            var solution = ws.OpenSolutionAsync(@"c:\Temp\TibcoEAISimulator\src\TibcoEaiSimulator.sln").Result;

            var project = solution.Projects.First();

            foreach (var doc in project.Documents)
            {
                Debug.Print(doc.Name);
            }
        }

        [Test, Explicit]
        public void RunSmallSolutionParser()
        {
            var sp = SolutionParser.ParseSolution(@"c:\Temp\tracer\Tracer.sln");
        }

        [Test, Explicit]
        public void RunSolutionParser()
        {
            var sp = SolutionParser.ParseSolution(@"c:\Temp\TibcoEAISimulator\src\TibcoEaiSimulator.sln");
        }
    }
}
