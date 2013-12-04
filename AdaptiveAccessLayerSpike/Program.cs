using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveAccessLayerSpike
{
    class Program
    {
        static void Main(string[] args)
        {
            ITestLogWriter testLogWriter = AdaptiveLayerFactory.CreateLogWriter<ITestLogWriter>();

            testLogWriter.Log("Did it log anything?");
        }
    }
}
