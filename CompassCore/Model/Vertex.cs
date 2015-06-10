using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompassCore.Model
{
    public class Vertex
    {
        private Dictionary<string, object> _props;

        public Vertex(Dictionary<string, object> props)
        {
            _props = props;
        }

        public string Id
        {
            get { return (string)_props["$ID"]; }
        }

        public T GetByKey<T>(string key)
        {
            return (T)_props[key];
        }

        public bool HasKey(string key)
        {
            return _props.ContainsKey(key);
        }

        public bool TryGetByKey<T>(string key, out T value)
        {
            value = default(T);
            if (!_props.ContainsKey(key))
            {
                return false;
            }

            value = (T)_props[key];
            return true;
        }
    }
}
