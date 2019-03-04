using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Face.CLISelf.Common
{
    [Serializable]
    public class MsgResult
    {
        public bool Success { get; set; }

        public string Msg { get; set; }

        public object Result { get; set; }
    }
}