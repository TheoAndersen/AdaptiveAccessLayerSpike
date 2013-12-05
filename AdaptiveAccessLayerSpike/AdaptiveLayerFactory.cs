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
            Type adaptiveLayerType = GetAdaptiveImplementationType<T>();
            typeBuilder.SetParent(adaptiveLayerType);


            var interfaceClass = typeof(T);

            foreach (MethodInfo method in interfaceClass.GetMethods())
            {
                CreateMethodImplementation<T>(typeBuilder, method, adaptiveLayerType);
            }

            Type generatedType = typeBuilder.CreateType();
            return (T)Activator.CreateInstance(generatedType);
        }

        private static Type GetAdaptiveImplementationType<T>()
        {
            var customAttributes = typeof(T).CustomAttributes;
            var baseLayerAttribute = customAttributes.SingleOrDefault(a => a.AttributeType.BaseType.Equals(typeof(AdaptiveLayerBase)));
            if (baseLayerAttribute == null)
            {
                throw new Exception("The interface: " + typeof(T).Name + " doesn't have an interface attribute that inherits from AdaptiveLayerBase");
            }
            return baseLayerAttribute.AttributeType;
        }

        private static void CreateMethodImplementation<T>(TypeBuilder typeBuilder, MethodInfo interfaceMethod, Type adaptiveAccessType)
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
            CreateObjectArrayForParameters(parameters, ilGenerator);
            SaveParametersInObjectArray(parameters, ilGenerator);
            CallExecuteImplWithMethodBaseAndParameters(ilGenerator, adaptiveAccessType);

            if (interfaceMethod.ReturnType.Equals(typeof(void)))
            {
                ilGenerator.Emit(OpCodes.Castclass, interfaceMethod.ReturnType);
                ilGenerator.Emit(OpCodes.Stloc_1);
                Label targetInstruction = ilGenerator.DefineLabel();
                ilGenerator.Emit(OpCodes.Br_S, targetInstruction);
                ilGenerator.MarkLabel(targetInstruction);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Nop);
            }
             
            ilGenerator.Emit(OpCodes.Ret);
        }

        private static void CreateObjectArrayForParameters(Type[] parameters, ILGenerator ilGenerator)
        {
            ilGenerator.DeclareLocal(typeof(object[]));
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Ldc_I4, parameters.Count());
            ilGenerator.Emit(OpCodes.Newarr, typeof(object));
            ilGenerator.Emit(OpCodes.Stloc_0);
        }

        private static void SaveParametersInObjectArray(Type[] parameters, ILGenerator ilGenerator)
        {
            for (int i = 0; i < parameters.Count(); i++)
            {
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ldc_I4, i);
                ilGenerator.Emit(OpCodes.Ldarg, i + 1);

                if (parameters[i].IsValueType)
                {
                    ilGenerator.Emit(OpCodes.Box, parameters[i]);
                }

                ilGenerator.Emit(OpCodes.Stelem_Ref);
            }
        }

        private static void CallExecuteImplWithMethodBaseAndParameters(ILGenerator ilGenerator, Type adaptiveAccessType)
        {
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetCurrentMethod"));
            MethodInfo mInfo = adaptiveAccessType.GetMethod("ExecuteImpl");
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.EmitCall(OpCodes.Call, mInfo, new[] { typeof(MethodBase), typeof(object[]) });
        }
    }
}
