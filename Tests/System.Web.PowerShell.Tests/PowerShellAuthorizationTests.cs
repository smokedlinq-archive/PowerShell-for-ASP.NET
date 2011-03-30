using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Web.PowerShell.Tests
{
    [TestClass]
    public class PowerShellAuthorizationTests
    {
        [TestMethod]
        public void should_return_false()
        {
            Assert.AreEqual(false, new MockController().IsAuthorized(x => x.AlwaysFailAuthorization()));
        }

        [TestMethod]
        public void should_return_true()
        {
            Assert.AreEqual(true, new MockController().IsAuthorized(x => x.AlwaysAllowAuthoriation()));
        }

        [TestMethod]
        public void should_return_true_if_iheartpowershell_is_true()
        {
            Assert.AreEqual(true, new MockController().IsAuthorized(x => x.AllowOnlyIfIHeartPowerShellIsTrue(true)));
        }

        [TestMethod]
        public void should_return_false_if_iheartpowershell_is_false()
        {
            Assert.AreEqual(false, new MockController().IsAuthorized(x => x.AllowOnlyIfIHeartPowerShellIsTrue(false)));
        }

        [TestMethod]
        public void should_return_false_if_isanonymous_is_true()
        {
            Assert.AreEqual(false, new MockController().IsAuthorized(x => x.ActionAuthorizationByResourceScript(true)));
        }

        class MockController : Controller
        {
            [PowerShellAuthorization.Script("$false")]
            public ActionResult AlwaysFailAuthorization()
            {
                throw new NotImplementedException();
            }

            [PowerShellAuthorization.Script("$true")]
            public ActionResult AlwaysAllowAuthoriation()
            {
                throw new NotImplementedException();
            }

            [PowerShellAuthorization.Script("param($RouteValues) process { [bool]$RouteValues.iheartpowershell }")]
            public ActionResult AllowOnlyIfIHeartPowerShellIsTrue(bool iheartpowershell)
            {
                throw new NotImplementedException();
            }

            [PowerShellAuthorization.Resource(typeof(PowerShellScripts), "AuthorizeMvcAction")]
            public ActionResult ActionAuthorizationByResourceScript(bool isAnonymous)
            {
                throw new NotImplementedException();
            }
        }
    }
}
