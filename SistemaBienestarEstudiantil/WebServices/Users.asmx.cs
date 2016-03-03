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
    /// Descripción breve de WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio Web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class Users : System.Web.Services.WebService
    {

        [WebMethod]
        public void getAllUser()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            Context.Response.Write(new JavaScriptSerializer().Serialize(db.USUARIOs.ToList()));
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void getAllActivedUser()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            Context.Response.Write(new JavaScriptSerializer().Serialize(db.USUARIOs.Where(u => u.ESTADOUSUARIO == true).ToList()));
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void getUserByCode(int code)
        {
            bienestarEntities db = new bienestarEntities();
            USUARIO usuario = db.USUARIOs.Single(u => u.CODIGOUSUARIO == code);
            Context.Response.Write(new JavaScriptSerializer().Serialize(usuario));
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void removeUserById(int id)
        {
            bienestarEntities db = new bienestarEntities();

            USUARIO usuario = db.USUARIOs.Single(u => u.CODIGOUSUARIO == id);
            db.USUARIOs.DeleteObject(usuario);
            db.SaveChanges();

            Context.Response.Write("ok");
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void inactiveUserById(int id)
        {
            bienestarEntities db = new bienestarEntities();

            USUARIO usuario = db.USUARIOs.Single(u => u.CODIGOUSUARIO == id);
            usuario.ESTADOUSUARIO = false;
            db.SaveChanges();

            writeResponse("ok");
        }

        [WebMethod]
        public void saveUserData(
            int userCode,
            String userName,
            String userCompleteName,
            String userIdentificationNumber,
            Boolean resetPassword)
        {
            bienestarEntities db = new bienestarEntities();

            USUARIO usuario = db.USUARIOs.Single(u => u.CODIGOUSUARIO == userCode);
            
            usuario.USUARIO1 = userName;
            usuario.NOMBREUSUARIO = userCompleteName;
            usuario.CEDULAUSUARIO = userIdentificationNumber;

            if (resetPassword) {
                usuario.CONTRASENAACTUAL = userIdentificationNumber;
                usuario.CONTRASENAANTERIOR = userIdentificationNumber;
            }

            db.SaveChanges();

            writeResponse("ok");
        }


        [WebMethod]
        public string prueba1()
        {
            return "prueba michel";
        }

        private void writeResponse(String response)
        {
            Context.Response.Write(response);
            Context.Response.Flush();
            Context.Response.End();
        }

    }
}
