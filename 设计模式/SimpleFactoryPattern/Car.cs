using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{
    public abstract class Car
    {
        public string CarName { get; set; }
        public abstract void Drive();
    }
}
