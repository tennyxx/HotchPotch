using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{
    class Benz : Car
    {
        public override void Drive()
        {
            Console.WriteLine("Benz Drive");
            Console.ReadKey();
        }
    }
}
