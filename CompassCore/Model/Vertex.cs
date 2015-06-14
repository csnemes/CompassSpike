using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompassCore.Model
{
    public class Vertex
    {
        private readonly VertexRef _vertexRef;
        private readonly GraphSpace _graphSpace;

        public Vertex(GraphSpace graphSpace, VertexRef vertexRef)
        {
            _graphSpace = graphSpace;
            _vertexRef = vertexRef;
        }

        public Dictionary<string, object> Props
        {
            get
            {
                return _graphSpace.GetProperties(_vertexRef);
            }
        }

        public string Id
        {
            get { return (string)Props["$ID"]; }
        }

        public T GetByKey<T>(string key)
        {
            return (T)Props[key];
        }

        public bool HasKey(string key)
        {
            return Props.ContainsKey(key);
        }

        public bool TryGetByKey<T>(string key, out T value)
        {
            value = default(T);
            if (!Props.ContainsKey(key))
            {
                return false;
            }

            value = (T)Props[key];
            return true;
        }

        public bool CanNavigateOn(string edgeName)
        {
            return _graphSpace.GetOutgoingEdgesFrom(_vertexRef).ContainsKey(edgeName);
        }

        public IEnumerable<string> GetNavigationPossibilities()
        {
            return _graphSpace.GetOutgoingEdgesFrom(_vertexRef).Select(item => item.Key);
        }

        public IEnumerable<Vertex> NavigateOn(string edgeName)
        {
            IEnumerable<Vertex> result;
            if (TryNavigateOn(edgeName, out result))
            {
                return result;
            }
            throw new ApplicationException(string.Format("Edge named {0} does not exist!", edgeName));
        }

        public bool TryNavigateOn(string edgeName, out IEnumerable<Vertex> result)
        {
            result = null;
            List<VertexRef> refResults;
            if (_graphSpace.GetOutgoingEdgesFrom(_vertexRef).TryGetValue(edgeName, out refResults))
            {
                result = refResults.Select(r => _graphSpace.GetVertex(r));
                return true;
            }
            return false;
        }

    }
}
