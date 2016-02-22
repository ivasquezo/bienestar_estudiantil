using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaBienestarEstudiantil.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["Message"] = "ASP.NET MVC";
            ViewData["Permiso"] = "true"; 
            
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Usuarios()
        {
            return View();
        }

        public ActionResult Roles()
        {
            return View();
        }

        public ActionResult Datos()
        {
            return View();
        }

        public ActionResult Tareas()
        {
            return View();
        }

        static public String prueba() {
            return "michel te ama";
        }

    }
}
