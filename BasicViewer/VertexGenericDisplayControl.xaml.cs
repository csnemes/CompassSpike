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
                tree.Items.Add(CreateVertexNode(vertex));
            }
        }

        private void TreeViewItemExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;
            if (tvi != null)
            {
                if (tvi.Items != null && tvi.Items.Count > 0) return; //already filled

                //ket nodefajta lehet vertex vagy a navigációs cucc
                if (tvi.Tag.GetType() == typeof(Vertex))
                {
                    var vertex = (Vertex)tvi.Tag;
                    var navs = vertex.GetNavigationPossibilities();
                    foreach (var nav in navs)
                    {
                        tvi.Items.Add(CreateLinkNode(nav));
                    }
                }
                else
                {
                    var navType = (string)tvi.Tag;
                    var parentVertex = (Vertex)((TreeViewItem)tvi.Parent).Tag;
                    var childs = parentVertex.NavigateOn(navType);

                    foreach (var vertex in childs)
                    {
                        tvi.Items.Add(CreateVertexNode(vertex));
                    }
                }

            }
        }

        private TreeViewItem CreateLinkNode(string linkType)
        {
            var newItem = new TreeViewItem();
            newItem.Header = linkType;
            newItem.Tag = linkType;
            newItem.Expanded += new RoutedEventHandler(TreeViewItemExpanded);
            return newItem;
        }

        private TreeViewItem CreateVertexNode(Vertex vertex)
        {
            var item = new TreeViewItem();
            item.Header = vertex.Id;
            item.Expanded += new RoutedEventHandler(TreeViewItemExpanded);
            item.Tag = vertex;
            return item;
        }
    }
}

