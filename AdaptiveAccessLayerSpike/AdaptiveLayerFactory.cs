using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

namespace AdaptiveAccessLayerSpike
{
    public class AdaptiveLayerFactory
    {
        public static T CreateLogWriter<T>()
        {
            // Create dynamic assembly
            var aName = new AssemblyName("temp");
            var appDomain = System.Threading.Thread.GetDomain();
            var assemblyBuilder = appDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
            ModuleBuilder methodBuilder = assemblyBuilder.DefineDynamicModule(aName.Name);

            // Create type builder
            TypeBuilder typeBuilder = methodBuilder.DefineType(typeof(T).Name.TrimStart('I') + "Impl", TypeAttributes.Public | TypeAttributes.Class);
            typeBuilder.AddInterfaceImplementation(typeof(T));
            typeBuilder.SetParent(typeof(LogAccessLayer));



            var interfaceAddMethod = typeof(T).GetMethod("Log");

            var methBuilder = typeBuilder.DefineMethod("Log", MethodAttributes.Public |
                                MethodAttributes.Virtual |
                                MethodAttributes.Final |
                                MethodAttributes.NewSlot |
                                MethodAttributes.HideBySig);
            methBuilder.SetReturnType(interfaceAddMethod.ReturnType);
            var parameters = interfaceAddMethod
                .GetParameters()
                .Select(parameter => parameter.ParameterType)
                .ToArray();
            methBuilder.SetParameters(parameters);
            
            typeBuilder.DefineMethodOverride(methBuilder, interfaceAddMethod);

            var ilGenerator = methBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetCurrentMethod"));
            MethodInfo mInfo = typeof(LogAccessLayer).GetMethod("ExecuteImpl");
            ilGenerator.EmitCall(OpCodes.Call, mInfo, new[] { typeof(MethodBase) });
            ilGenerator.Emit(OpCodes.Pop);
            ilGenerator.Emit(OpCodes.Ret);
            methBuilder.SetParameters(new Type[] { typeof(string) });

            Type generatedType = typeBuilder.CreateType();
            return (T)Activator.CreateInstance(generatedType);
        }
    }
}
