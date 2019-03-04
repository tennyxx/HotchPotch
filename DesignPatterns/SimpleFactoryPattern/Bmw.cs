using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{
    public class Bmw : Car
    {
        public override void Drive()
        {
            Console.WriteLine("Bmw Drive");
            Console.ReadKey();
        }
    }
}
