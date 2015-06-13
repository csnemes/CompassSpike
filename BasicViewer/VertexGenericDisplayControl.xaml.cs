using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using CompassCore.Model;

namespace BasicViewer
{
    /// <summary>
    /// Interaction logic for VertexGenericDisplayControl.xaml
    /// </summary>
    public partial class VertexGenericDisplayControl : UserControl
    {
        public VertexGenericDisplayControl()
        {
            InitializeComponent();
        }

        internal void DisplayVertices(IEnumerable<Vertex> vertices)
        {
            tree.Items.Clear();
            foreach (var vertex in vertices)
            {
                var item = new TreeViewItem();
                item.Header = vertex.Id;
                tree.Items.Add(item);
            }
        }
    }
}
