using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SistemaBienestarEstudiantil;
using SistemaBienestarEstudiantil.Controllers;

namespace SistemaBienestarEstudiantil.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Disponer
            HomeController controller = new HomeController();

            // Actuar
            ViewResult result = controller.Index() as ViewResult;

        }

        [TestMethod]
        public void About()
        {
            // Disponer
            HomeController controller = new HomeController();
        }
    }
}
