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
        public static BE_USUARIO usuario { get; set; }

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
            if (Utils.isLogged()) return Redirect("/");

            if (ModelState.IsValid)
            {
                usuario = MembershipService.ValidateUser(model.UserName, model.Password);

                if (usuario != null)
                {
                    Session["userName"] = usuario.NOMBREUSUARIO;
                    Session["userValidated"] = false;
                    Session["firstPasswordAccess"] = false;
                    Session["userRol"] = usuario.CODIGOROL;
                    Session["userCode"] = usuario.CODIGO;

                    FormsService.SignIn(model.UserName);

                    if (!String.IsNullOrEmpty(returnUrl)) { return Redirect(returnUrl); }

                    else
                    {
                        if (Utils.ValidateFirstPassword(usuario.CONTRASENAACTUAL, usuario.CONTRASENAANTERIOR))
                        {
                            Session["userValidated"] = false;
                            Session["firstPasswordAccess"] = true;

                            return RedirectToAction("ChangePassword", "Home");
                        }
                        else
                        {
                            Session["userValidated"] = true;
                            Session["firstPasswordAccess"] = false;

                            SetAccessRol();

                            return RedirectToAction(pageRedirect(), "Home");
                        }

                    }
                }
                else
                    ModelState.AddModelError("", "El nombre de usuario o la contrase\u00F1a especificados son incorrectos.");
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
                    Session["userValidated"] = true;
                    Session["firstPasswordAccess"] = false;

                    SetAccessRol();

                    return RedirectToAction(pageRedirect(), "Home");
                }
                else
                {
                    ModelState.AddModelError("", "La contrase\u00F1a actual es incorrecta o la nueva contrase\u00F1a no es v\u00E1lida..");
                    Session["userName"] = null;
                }
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        private void SetAccessRol()
        {
            List<BE_ROL_ACCESO> accessRol = MembershipService.GetAccessRol(Int32.Parse(Session["userRol"].ToString()));

            Session["editarUsuario"] = false;
            Session["editarRol"] = false;
            Session["editarTarea"] = false;
            Session["editarEncuesta"] = false;
            Session["editarBeca"] = false;

            foreach (BE_ROL_ACCESO access in accessRol)
            {
                if (access.CODIGOACCESO == 1) { Session["editarUsuario"] = true; }

                if (access.CODIGOACCESO == 2) { Session["editarRol"] = true; }

                if (access.CODIGOACCESO == 3) { Session["editarTarea"] = true; }

                if (access.CODIGOACCESO == 4) { Session["editarEncuesta"] = true; }

                if (access.CODIGOACCESO == 5) { Session["editarBeca"] = true; }
            }
        }

        private String pageRedirect()
        {
            

            if (Convert.ToBoolean(Session["editarTarea"])) { return "Tareas"; }

            else if (Convert.ToBoolean(Session["editarBeca"])) { return "Becas"; }

            else if (Convert.ToBoolean(Session["editarEncuesta"])) { return "Encuestas"; }

            else if (Convert.ToBoolean(Session["editarUsuario"])) { return "Usuarios"; }

            else if (Convert.ToBoolean(Session["editarRol"])) { return "Roles"; }

            return null;
        }

        public ActionResult Usuarios()
        {
            Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
            Response.Cache.SetAllowResponseInBrowserHistory(false);
            Response.Cache.SetNoStore();
            if (!Utils.ValidateAccessUsuario()) { return RedirectToAction("Index", "Home"); }

            return View();
        }

        public ActionResult Roles()
        {
            Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
            Response.Cache.SetAllowResponseInBrowserHistory(false);
            Response.Cache.SetNoStore();
            if (!Utils.ValidateAccessRol()) { return RedirectToAction("Index", "Home"); }

            return View();
        }

        public ActionResult Tareas()
        {
            Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
            Response.Cache.SetAllowResponseInBrowserHistory(false);
            Response.Cache.SetNoStore();
            if (!Utils.ValidateAccessTarea()) { return RedirectToAction("Index", "Home"); }

            return View();
        }

        public ActionResult Encuestas()
        {
            if (!Utils.ValidateAccessEncuesta()) { return RedirectToAction("Index", "Home"); }

            return View();

        }

        public ActionResult Becas()
        {
            if (!Utils.ValidateAccessBeca()) { return RedirectToAction("Index", "Home"); }

            return View();
        }

        public ActionResult Encuesta()
        {
            return View();
        }

        public ActionResult BecaSolicitud()
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