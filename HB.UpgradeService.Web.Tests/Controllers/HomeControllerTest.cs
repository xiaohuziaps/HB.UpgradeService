using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HB.UpgradeService.Web;
using HB.UpgradeService.Web.Controllers;

namespace HB.UpgradeService.Web.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // 排列
            HomeController controller = new HomeController();

            // 操作
            ViewResult result = controller.Index() as ViewResult;

            // 断言
            Assert.AreEqual("欢迎使用 ASP.NET MVC!", result.ViewBag.Message);
        }

        [TestMethod]
        public void About()
        {
             
        }
    }
}
