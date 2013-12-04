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


            var interfaceClass = typeof(T);

            foreach (MethodInfo method in interfaceClass.GetMethods())
            {
                CreateMethodImplementation<T>(typeBuilder, method);
            }

            Type generatedType = typeBuilder.CreateType();
            return (T)Activator.CreateInstance(generatedType);
        }

        private static void CreateMethodImplementation<T>(TypeBuilder typeBuilder, MethodInfo interfaceMethod)
        {
            var methBuilder = typeBuilder.DefineMethod(interfaceMethod.Name, MethodAttributes.Public |
                                MethodAttributes.Virtual |
                                MethodAttributes.Final |
                                MethodAttributes.NewSlot |
                                MethodAttributes.HideBySig);
            methBuilder.SetReturnType(interfaceMethod.ReturnType);
            var parameters = interfaceMethod
                .GetParameters()
                .Select(parameter => parameter.ParameterType)
                .ToArray();
            methBuilder.SetParameters(parameters);
            methBuilder.InitLocals = true;

            typeBuilder.DefineMethodOverride(methBuilder, interfaceMethod);

            var ilGenerator = methBuilder.GetILGenerator();
            ilGenerator.DeclareLocal(typeof(object[]));
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldc_I4, parameters.Count());
            ilGenerator.Emit(OpCodes.Newarr, typeof(object));
            ilGenerator.Emit(OpCodes.Stloc_0);

            for (int i = 0; i < parameters.Count(); i++)
			{
                SaveParamInObjectArray(ilGenerator, parameters[i], i);
            }

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetCurrentMethod"));
            MethodInfo mInfo = typeof(LogAccessLayer).GetMethod("ExecuteImpl");
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.EmitCall(OpCodes.Call, mInfo, new[] { typeof(MethodBase), typeof(object[]) });
            ilGenerator.Emit(OpCodes.Pop);
            ilGenerator.Emit(OpCodes.Ret);
            methBuilder.SetParameters(new Type[] { typeof(string) });
        }

        private static void SaveParamInObjectArray(ILGenerator ilGenerator, Type parameter, int numParam)
        {
            if(parameter.IsValueType == false)
            {
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ldc_I4, numParam);
                ilGenerator.Emit(OpCodes.Ldarg, numParam+1);
                ilGenerator.Emit(OpCodes.Stelem_Ref);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ldc_I4, numParam);
                ilGenerator.Emit(OpCodes.Ldarg, numParam + 1);
                ilGenerator.Emit(OpCodes.Box, parameter);
                ilGenerator.Emit(OpCodes.Stelem_Ref);
            }
        }


    }
}
