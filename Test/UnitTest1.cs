using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdaptiveAccessLayerSpike;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void AdaptiveLayerFactory_CanCreateImplementationOfInterfaceT()
        {
            try
            {
                ITestLogWriter testLogerWriterImplementation = AdaptiveLayerFactory.CreateLogWriter<ITestLogWriter>();
                testLogerWriterImplementation.Log("noget");
            }
            catch (Exception ex)
            {
                Assert.Fail("Could not create implementation " + ex);
            }
        }
    }


}
