using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveAccessLayerSpike
{
    [AttributeUsage(AttributeTargets.Interface)]
    public abstract class AdaptiveLayerBase : Attribute
    {
        public object ExecuteImpl(MethodBase methodBase, object[] parameters)
        {
            return Execute(null, null, parameters);
        }

        public abstract object Execute(Attribute methodAttribute, MethodInfo methodInfo, object[] parameters);//, TAttributeData attributeData); 
    }
}
