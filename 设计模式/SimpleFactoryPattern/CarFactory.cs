using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{
    public static class CarFactory
    {

        public static Car GetCar(string carName)
        {
            if (carName.IndexOf("Benz") > -1)
            {
                return new Benz();
            }
            else
            {
                return new Bmw();
            }
        }

    }
}
