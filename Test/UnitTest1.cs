﻿using System;
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
                testLogerWriterImplementation.Log("noget", 42);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("her.. logaccesslayer. --> " + Environment.NewLine + "noget-42", ex.Message);
            }
        }
    }


}
