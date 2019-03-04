using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            var car = CarFactory.GetCar("Benz");
            car.Drive();
        }
    }
}
