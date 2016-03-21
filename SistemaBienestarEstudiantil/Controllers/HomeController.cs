using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemaBienestarEstudiantil.Models;
using System.Web.Routing;
using SistemaBienestarEstudiantil.Class;

namespace SistemaBienestarEstudiantil.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }
        public static USUARIO usuario { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                usuario = MembershipService.ValidateUser(model.UserName, model.Password);

                if (usuario != null)
                {
                    Session["userName"] = usuario.NOMBREUSUARIO;
                    Session["usuarioValidado"] = false;
                    Session["firstPasswordAccess"] = false;

                    FormsService.SignIn(model.UserName);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        if (Utils.ValidateFirstPassword(usuario.CONTRASENAACTUAL, usuario.CONTRASENAANTERIOR))
                        {
                            Session["usuarioValidado"] = false;
                            Session["firstPasswordAccess"] = true;
                            return RedirectToAction("ChangePassword", "Home");
                        }
                        else
                        {
                            Session["usuarioValidado"] = true;
                            Session["firstPasswordAccess"] = false;
                            return RedirectToAction("Tareas", "Home");
                        }
                        
                    }
                }
                else
                    ModelState.AddModelError("", "El nombre de usuario o la contraseña especificados son incorrectos.");
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (Utils.ValidateConfirmPassword(usuario.CONTRASENAACTUAL, model.OldPassword, model.NewPassword, model.ConfirmPassword))
                {
                    MembershipService.ChangePassword(usuario.CODIGO, model.NewPassword);
                    Session["usuarioValidado"] = true;
                    Session["firstPasswordAccess"] = false;
                    return RedirectToAction("Tareas", "Home");
                }
                else
                    ModelState.AddModelError("", "La contraseña actual es incorrecta o la nueva contraseña no es válida..");
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        public ActionResult Usuarios()
        {
            if (!Class.Utils.validateAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Roles()
        {
            if (!Class.Utils.validateAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Datos()
        {
            if (!Class.Utils.validateAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Tareas()
        {
            if (!Utils.validateAccess())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Encuestas()
        {
            return View();
            /*if (!Utils.validateAccess())
            {
                return View();
            }
            return RedirectToAction("Index", "Home");*/
        }

        public ActionResult Encuesta()
        {
            return View();
        }

        public ActionResult LogOn()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            FormsService.SignOut();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        

    }
}
