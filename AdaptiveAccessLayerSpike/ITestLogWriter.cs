using AdaptiveAccessLayerSpike.AAL;
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
        string Log(string message, int second);
    }

    /// <summary>
    /// This an example of the class that AdaptiveLayerFactory generates at runtime
    /// </summary>
    public class TestLogWriterImpl : LogContract, ITestLogWriter
    {
        public string Log(string message, int second)
        {
            object[] parameters = new object[2]; 
            parameters[0] = message;
            parameters[1] = second;
            return (string)base.ExecuteImpl(MethodBase.GetCurrentMethod(), parameters);
        }
    }   
}
