using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolLoadingTester
{
    internal class Program
    {
        static void Main()
        {
            //create an instance of a protocol library and test performance and bugs
            ProtocolBuffer eng = new ProtocolBuffer();
            foreach (var el in eng.Library)
            {
                Console.WriteLine(el.Item1);
            }
            Console.ReadLine();
        }
    }
}
