using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.CLISelf.Entity
{
    [Serializable]
    public class ResponeMsg
    {
        public string Operation { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
