using CompassCore.Model.Internal;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompassCore.Model
{
    //Builder class
    public class VertexDefinition
    {
        private readonly string _uniqueId;
        private readonly Dictionary<string, object> _props = new Dictionary<string, object>();

        private VertexDefinition(string uniqueId)
        {
            _uniqueId = uniqueId;
        }

        public static VertexDefinition FromSyntaxNode(SyntaxNode node)
        {
            //nem is biztos, hogy kell
            throw new NotImplementedException();
        }

        public static VertexDefinition FromSemanticNode(ISymbol symbol)
        {
            var id = UniqueIdDerivator.Instance.GetUniqueId(symbol);
            return new VertexDefinition(id);
        }

        public static VertexDefinition Create(string uniqueId)
        {
            return new VertexDefinition(uniqueId);
        }

        public VertexDefinition AddProp(string key, object value)
        {
            _props.Add(key, value);
            return this;
        }

        internal string Id
        {
            get { return _uniqueId; }
        }

        internal Dictionary<string, object> Props
        {
            get { return _props; }
        }
    }
}
