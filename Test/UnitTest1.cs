using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdaptiveAccessLayerSpike;

namespace Test
{
    [TestClass]
    public class AdaptiveLayerFactoryTest
    {
        [TestMethod]
        public void AdaptiveLayerFactory_CanCreateImplementationOfInterfaceT()
        {
            try
            {
                var result = AdaptiveLayerFactory.CreateLogWriter<ITestLogWriter>();
                result.Log("noget", 42);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("her.. logaccesslayer. --> " + Environment.NewLine + "noget-42", ex.Message);
                return;
            }
            Assert.Fail("didn't cast exception");
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class TestMethodContract : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Interface)]
        public class TestAccessLayer : AdaptiveLayerBase
        {
            public override object Execute(Attribute methodAttribute, System.Reflection.MethodInfo methodInfo, object[] parameters)
            {
                return parameters;
            }
        }

        [TestAccessLayer]
        public interface ITestInterface
        {
            [TestMethodContract]
            object[] TestMethod(string message, int number);
        }

        [TestMethod]
        public void CreateLogWriter_ShouldCreateImplementationWhichCanPassAlongTheParametersOfTheMethodAndReturnAnAnswer()
        {
            var testInterfaceImplementation = AdaptiveLayerFactory.CreateLogWriter<ITestInterface>();
            object[] result = testInterfaceImplementation.TestMethod("noget", 42);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("noget", result[0] as string);
            Assert.AreEqual(42, (int)result[1]);
        }
    }


}
