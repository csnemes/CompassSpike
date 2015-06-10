using CompassCore.Model.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompassCore.Model
{
    public struct VertexRef
    {
        private readonly int _id;

        public VertexRef(int id)
        {
            _id = id;
        }
    }
}
