using System;
using Infrastrutura;
using Infrastrutura.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCustomerService
{
    [TestClass]
    public class TesteContratoDoCustomer
    {
        private ICustomerService contrato;
        [TestMethod]
        public void Test_RegisterNewCustomer()
        {
            //  Prepare
            contrato = new CustomerService(null);
            //  Execute
            Guid id=Guid.NewGuid();
            var r1 = contrato.RegisterNewCustomer(new Customer(){CustomerId = id, Name = "TesteDO", NIF = "TesteDO", Email = "TesteDO", Country = "PT", UserName = "TesteDO", PassHash = "TesteDO", Logged = false});
            var r2 = contrato.RegisterNewCustomer(new Customer(){ CustomerId = id, Name = "TesteDO", NIF = "TesteDO", Email = "TesteDO", Country = "PT", UserName = "TesteDO", PassHash = "TesteDO", Logged = false});
            var s1 = contrato.SelectCustomer(id);
            var s2 = contrato.SelectCustomer("DO", "DO");
            var s3 = contrato.SelectCustomer("TesteDO", "TesteDO");
            var r3 = contrato.DeleteCustomer(id);
            //  Assert
            Assert.AreEqual(1, r1);
            Assert.AreEqual(-1, r2);
            Assert.IsNotNull(s1);
            Assert.IsNull(s2);
            Assert.IsNotNull(s3);
            Assert.AreEqual(1, r3);
        }
    }
}
