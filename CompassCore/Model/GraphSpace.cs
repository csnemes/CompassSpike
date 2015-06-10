using CompassCore.Model.Internal;
using System.Collections.Generic;

namespace CompassCore.Model
{
    public partial class GraphSpace
    {
        private readonly Dictionary<string, VertexRef> _keyIndex = new Dictionary<string, VertexRef>();

        private readonly Dictionary<VertexRef, Dictionary<string, object>> _propsForVertices = 
            new Dictionary<VertexRef, Dictionary<string, object>>();

        private readonly Dictionary<VertexRef, Dictionary<string, List<VertexRef>>> _edgesFromVertex = new Dictionary<VertexRef, Dictionary<string, List<VertexRef>>>();


        public VertexRef AddOrUpdateVertex(VertexDefinition node)
        {
            VertexRef targetVertex;
            if (!_keyIndex.TryGetValue(node.Id, out targetVertex))
            {
                targetVertex = new VertexRef(IdSeed.GetNext());
                _keyIndex.Add(node.Id, targetVertex);
                SetProperty(targetVertex, "$ID", node.Id);
            }

            foreach (var prop in node.Props)
            {
                SetProperty(targetVertex, prop.Key, prop.Value);
            }

            return targetVertex;
        }

        public Vertex GetVertex(VertexRef vRef)
        {
            Dictionary<string, object> props;
            if (_propsForVertices.TryGetValue(vRef, out props))
            {
                return new Vertex(props);
            }
            return null;
        }

        public void AddOrUpdateEdge(EdgeDefinition edgeDefinition)
        {
            Dictionary<string, List<VertexRef>> edges;
            if (!_edgesFromVertex.TryGetValue(edgeDefinition.Start, out edges))
            {
                edges = new Dictionary<string, List<VertexRef>>();
                _edgesFromVertex.Add(edgeDefinition.Start, edges);
            }

            List<VertexRef> targetVertices;
            if (!edges.TryGetValue(edgeDefinition.Name, out targetVertices))
            {
                targetVertices = new List<VertexRef>();
                edges.Add(edgeDefinition.Name, targetVertices);
            }

            if (!targetVertices.Contains(edgeDefinition.End))
            {
                targetVertices.Add(edgeDefinition.End);
            }
        }

        public Vertex GetVertexById(string id)
        {
            if (!_keyIndex.ContainsKey(id)) return null;
            return GetVertex(_keyIndex[id]);
        }

        public IEnumerable<Vertex> GetVerticesByFilter(Filter filter)
        {
            return null;
        }

        private void SetProperty(VertexRef id, string key, object value)
        {
            if (!_propsForVertices.ContainsKey(id))
            {
                _propsForVertices.Add(id, new Dictionary<string, object>());
            }
            _propsForVertices[id][key] = value;
        }

        private object GetProperty(VertexRef id, string key)
        {
            if (_propsForVertices.ContainsKey(id))
            {
                var props = _propsForVertices[id];
                if (props.ContainsKey(key))
                {
                    return props[key];
                }
            }

            return null;
        }

        private Dictionary<string, object> GetProperties(VertexRef id)
        {
            if (_propsForVertices.ContainsKey(id))
            {
                return _propsForVertices[id];
            }

            return null;
        }
    }
}
