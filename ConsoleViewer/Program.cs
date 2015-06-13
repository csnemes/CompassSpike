using CompassCore.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            var sp = SolutionParser.ParseSolution(@"c:\Temp\tracer\Tracer.sln");
            var rootItems = sp.GetVerticiesOfType(CompassCore.VertexType.Class);
        }
    }
}
