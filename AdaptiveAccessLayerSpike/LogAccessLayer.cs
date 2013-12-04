using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveAccessLayerSpike
{
    public abstract class LogAccessLayer : AdaptiveLayerBase
    {
        public override object Execute(Attribute methodAttribute, System.Reflection.MethodInfo methodInfo, object[] parameters)
        {
            throw new NotImplementedException("her.. logaccesslayer. --> " + Environment.NewLine + (string)parameters[0]);
        }
    }
}
