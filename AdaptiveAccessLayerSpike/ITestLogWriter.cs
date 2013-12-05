using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveAccessLayerSpike
{
    [LogContract]
    public interface ITestLogWriter
    {
        [LogEntryContract]
        void Log(string message, int second);
    }

    public class TestLogWriterImpl : LogContract, ITestLogWriter
    {
        public void Log(string message, int second)
        {
            object[] parameters = new object[2]; 
            parameters[0] = message;
            parameters[1] = second;
            base.ExecuteImpl(MethodBase.GetCurrentMethod(), parameters);
        }
    }   
}
