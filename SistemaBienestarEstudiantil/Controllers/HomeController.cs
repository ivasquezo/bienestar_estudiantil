using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemaBienestarEstudiantil.Models;
using System.Web.Routing;

namespace SistemaBienestarEstudiantil.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public static String Permiso { get; set; }
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        public ActionResult Index()
        {
            Permiso = "true";
            ViewData["Permiso"] = Permiso;

            return View();
        }

        [HttpPost]
        public ActionResult Index(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "El nombre de usuario o la contraseña especificados son incorrectos.");
                }
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Usuarios()
        {
            ViewData["Permiso"] = Permiso;

            return View();
        }

        public ActionResult Roles()
        {
            ViewData["Permiso"] = Permiso;

            return View();
        }

        public ActionResult Datos()
        {
            ViewData["Permiso"] = Permiso;

            return View();
        }

        public ActionResult Tareas()
        {
            ViewData["Permiso"] = Permiso;

            return View();
        }

        public ActionResult LogOn()
        {
            ViewData["Permiso"] = Permiso;

            return View();
        }
    }
}
