using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using CompassCore.Parsing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphX;
using QuickGraph;
using GraphX.Logic;
using CompassCore.Model;
using GraphX.GraphSharp.Algorithms.Layout.Simple.FDP;
using GraphX.GraphSharp.Algorithms.OverlapRemoval;

namespace VertexVisualizerViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _selectedFile;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".sln"; // Default file extension
            dlg.Filter = "Solution files (.sln)|*.sln"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                _selectedFile = dlg.FileName;
                selectedFile.Text = _selectedFile;
                DoRefresh();
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            DoRefresh();
        }

        private void DoRefresh()
        {
            var parsingTask = Task.Factory.StartNew((inp) => SolutionParser.ParseSolution(_selectedFile), null,
                CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            parsingTask.ContinueWith((prevTask, state) =>
            {
                var vertices = prevTask.Result.GetVerticiesOfType(CompassCore.VertexType.Class);

                var graph = new GraphExample();

                var i = 1;
                foreach(var vertex in vertices)
                {
                    graph.AddVertex(new DataVertex() { ID = i, Text = vertex.Id });
                    i++;
                }

                var vlist = graph.Vertices.ToList();

                foreach (var displayVertex in vlist)
                {
                    var originalVertex = vertices.First(item => item.Id == displayVertex.Text);
                    var navigationPossibilities = originalVertex.GetNavigationPossibilities();

                    foreach(var edge in navigationPossibilities)
                    {
                        var targetVertices = originalVertex.NavigateOn(edge);

                        foreach (var targetVertex in targetVertices)
                        {
                            var targetDisplayVertex = vlist.FirstOrDefault(item => item.Text == targetVertex.Id);

                            if (targetDisplayVertex != null)
                            {
                                graph.AddEdge(new DataEdge(displayVertex, targetDisplayVertex, 1)
                                { Text = edge });
                            }
                        }
                    }
                }

                var logicCore = BuildLogicCore(graph);

                gg_Area.LogicCore = logicCore;
                gg_Area.GenerateGraph(true);
            },
                null,
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        private GXLogicCoreExample BuildLogicCore(GraphExample graph)
        {
            var logicCore = new GXLogicCoreExample { Graph = graph };
            //This property sets layout algorithm that will be used to calculate vertices positions
            //Different algorithms uses different values and some of them uses edge Weight property.
            logicCore.DefaultLayoutAlgorithm = GraphX.LayoutAlgorithmTypeEnum.KK;
            //Now we can set optional parameters using AlgorithmFactory
            //NOTE: default parameters can be automatically created each time you change Default algorithms
            logicCore.DefaultLayoutAlgorithmParams =
                               logicCore.AlgorithmFactory.CreateLayoutParameters(GraphX.LayoutAlgorithmTypeEnum.KK);
            //Unfortunately to change algo parameters you need to specify params type which is different for every algorithm.
            ((KKLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).MaxIterations = 100;

            //This property sets vertex overlap removal algorithm.
            //Such algorithms help to arrange vertices in the layout so no one overlaps each other.
            logicCore.DefaultOverlapRemovalAlgorithm = GraphX.OverlapRemovalAlgorithmTypeEnum.FSA;
            //Setup optional params
            logicCore.DefaultOverlapRemovalAlgorithmParams =
                              logicCore.AlgorithmFactory.CreateOverlapRemovalParameters(GraphX.OverlapRemovalAlgorithmTypeEnum.FSA);
            ((OverlapRemovalParameters)logicCore.DefaultOverlapRemovalAlgorithmParams).HorizontalGap = 50;
            ((OverlapRemovalParameters)logicCore.DefaultOverlapRemovalAlgorithmParams).VerticalGap = 50;

            //This property sets edge routing algorithm that is used to build route paths according to algorithm logic.
            //For ex., SimpleER algorithm will try to set edge paths around vertices so no edge will intersect any vertex.
            logicCore.DefaultEdgeRoutingAlgorithm = GraphX.EdgeRoutingAlgorithmTypeEnum.SimpleER;

            //This property sets async algorithms computation so methods like: Area.RelayoutGraph() and Area.GenerateGraph()
            //will run async with the UI thread. Completion of the specified methods can be catched by corresponding events:
            //Area.RelayoutFinished and Area.GenerateGraphFinished.
            logicCore.AsyncAlgorithmCompute = false;

            return logicCore;
        }
    }

    public class GraphAreaExample : GraphArea<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>> { }

    //Graph data class
    public class GraphExample : BidirectionalGraph<DataVertex, DataEdge> { }

    //Logic core class
    public class GXLogicCoreExample : GXLogicCore<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>> { }

    //Vertex data object
    public class DataVertex : VertexBase
    {
        /// <summary>
        /// Some string property for example purposes
        /// </summary>
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    //Edge data object
    public class DataEdge : EdgeBase<DataVertex>
    {
        public DataEdge(DataVertex source, DataVertex target, double weight = 1)
            : base(source, target, weight)
        {
        }

        public DataEdge()
            : base(null, null, 1)
        {
        }

        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
