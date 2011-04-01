using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Web.PowerShell.Tests
{
    [TestClass]
    public class PowerShellDependencyResolverTests
    {
        [TestMethod]
        public void resolver_returns_ArgumentException_when_Exception_is_registered_type()
        {
            var resolver = new PowerShellDependencyResolver();

            resolver.Register<Exception>(HttpPowerShellCommand.FromScript("New-Object System.ArgumentException"));

            DependencyResolver.SetResolver(resolver);

            Assert.IsInstanceOfType(DependencyResolver.Current.GetService<Exception>(), typeof(ArgumentException));
        }

        [TestMethod]
        public void resolver_returns_int_array_when_int_is_registered_type_and_getservices_called()
        {
            var resolver = new PowerShellDependencyResolver();

            resolver.Register<int>(HttpPowerShellCommand.FromScript("1..5"));

            DependencyResolver.SetResolver(resolver);

            Assert.AreEqual(15, DependencyResolver.Current.GetServices<int>().Sum());
        }
    }
}
