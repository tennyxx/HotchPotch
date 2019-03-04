using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.CLISelf.Entity
{
    [Serializable]
    public class RequestMsg
    {
        public string Operation { get; set; }
        public Object[] Parameters { get; set; }
    }


}
