using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CompassCore.Model.Internal
{
    public static class IdSeed
    {
        private static int _seed = 0;

        public static int GetNext()
        {
            return Interlocked.Increment(ref _seed);
        }
    }
}
