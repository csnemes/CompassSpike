using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompassCore.Model
{
    public class EdgeDefinition
    {
        private string _name;
        private VertexRef _startNode;
        private VertexRef _endNode;

        private EdgeDefinition(string name)
        {
            _name = name;
        }

        public static EdgeDefinition Create(string name)
        {
            return new EdgeDefinition(name);
        }

        public EdgeDefinition FromVertex(VertexRef startNode)
        {
            _startNode = startNode;
            return this;
        }

        public EdgeDefinition ToVertex(VertexRef endNode)
        {
            _endNode = endNode;
            return this;
        }

        internal string Name
        {
            get { return _name; }
        }

        internal VertexRef Start
        {
            get { return _startNode; }
        }

        internal VertexRef End
        {
            get { return _endNode; }
        }
    }
}
