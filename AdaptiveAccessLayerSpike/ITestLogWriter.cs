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
        void Log(string message);
    }

    public class TestLogWriterImpl : LogAccessLayer, ITestLogWriter
    {
        public void Log(string message)
        {
            object[] parameters = new object[1]; 
            parameters[0] = message;
            base.ExecuteImpl(MethodBase.GetCurrentMethod(), parameters);
        }
    }   
}
