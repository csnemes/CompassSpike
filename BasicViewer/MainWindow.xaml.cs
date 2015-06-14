using CompassCore;
using CompassCore.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BasicViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _selectedFile;
        private SolutionParser _parser;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            combo.ItemsSource = Enum.GetValues(typeof(VertexType)).Cast<VertexType>();
            combo.SelectedItem = VertexType.Solution;
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
                RunParsing();
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            DoRefresh();
        }

        private void DoRefresh()
        {
            var items = _parser.GetVerticiesOfType((VertexType)combo.SelectedItem);
            display.DisplayVertices(items);
        }

        private void RunParsing()
        {
            var parsingTask = Task.Factory.StartNew((inp) => SolutionParser.ParseSolution(_selectedFile), null,
                CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            parsingTask.ContinueWith((prevTask, state) =>
                {
                    _parser = prevTask.Result;
                    DoRefresh();
                }, 
                null, 
                CancellationToken.None, 
                TaskContinuationOptions.OnlyOnRanToCompletion, 
                TaskScheduler.FromCurrentSynchronizationContext() );
        }
    }
}
