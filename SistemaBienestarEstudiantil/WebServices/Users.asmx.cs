using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using SistemaBienestarEstudiantil.Models;

namespace SistemaBienestarEstudiantil.WebServices
{
    /// <summary>
    /// Descripción breve de Users
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio Web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class Users : System.Web.Services.WebService
    {
        private void writeResponse(String response)
        {
            Context.Response.Write(response);
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void getAllUser()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            writeResponse(new JavaScriptSerializer().Serialize(db.BE_USUARIO.ToList()));
        }

        [WebMethod]
        public void getAllActivedUser()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();

            Context.Response.Write(new JavaScriptSerializer().Serialize(db.BE_USUARIO.Where(u => u.ESTADO == true).ToList()));

            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void getUserByCode(int code)
        {
            bienestarEntities db = new bienestarEntities();

            BE_USUARIO usuario = db.BE_USUARIO.Single(u => u.CODIGO == code);

            writeResponse(new JavaScriptSerializer().Serialize(usuario));
        }

        [WebMethod]
        public void removeUserById(int id)
        {
            bienestarEntities db = new bienestarEntities();

            BE_USUARIO usuario = db.BE_USUARIO.Single(u => u.CODIGO == id);

            db.BE_USUARIO.DeleteObject(usuario);
            db.SaveChanges();

            Context.Response.Write("ok");
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void inactiveUserById(int id)
        {
            bienestarEntities db = new bienestarEntities();

            BE_USUARIO usuario = db.BE_USUARIO.Single(u => u.CODIGO == id);

            usuario.ESTADO = false;

            db.SaveChanges();

            writeResponse(new JavaScriptSerializer().Serialize(usuario));
        }

        [WebMethod]
        public void saveUserData(BE_USUARIO user, Boolean resetPassword)
        {
            bienestarEntities db = new bienestarEntities();

            BE_USUARIO usuario = db.BE_USUARIO.Single(u => u.CODIGO == user.CODIGO);

            usuario.NOMBREUSUARIO = user.NOMBREUSUARIO;
            usuario.NOMBRECOMPLETO = user.NOMBRECOMPLETO;
            usuario.CEDULA = user.CEDULA;
            usuario.CORREO = user.CORREO;
            usuario.ESTADO = user.ESTADO;
            usuario.CODIGOROL = user.CODIGOROL;

            if (resetPassword)
            {
                usuario.CONTRASENAACTUAL = user.CEDULA;
                usuario.CONTRASENAANTERIOR = user.CEDULA;
            }

            db.SaveChanges();

            writeResponse(new JavaScriptSerializer().Serialize(usuario));
        }

        [WebMethod]
        public void addNewUser(BE_USUARIO newUser)
        {
            bienestarEntities db = new bienestarEntities();
            newUser.CONTRASENAACTUAL = newUser.CEDULA;
            newUser.CONTRASENAANTERIOR = newUser.CEDULA;
            db.BE_USUARIO.AddObject(newUser);
            db.SaveChanges();
            writeResponse(new JavaScriptSerializer().Serialize(newUser));
        }

        [WebMethod]
        public void countUserWithCedula(string cedula)
        {
            bienestarEntities db = new bienestarEntities();
            int cantidad = db.BE_USUARIO.Where(u => cedula != null && u.CEDULA == cedula).Count();
            writeResponse("{\"cantidad\":" + cantidad + "}");
        }
    }
}
