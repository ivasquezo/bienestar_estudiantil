using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using SistemaBienestarEstudiantil.Models;
using SistemaBienestarEstudiantil.Class;

namespace SistemaBienestarEstudiantil.WebServices
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio Web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la linea siguiente. 
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
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                List<BE_USUARIO> users = db.BE_USUARIO.ToList();

                if (users != null && users.Count > 0)
                    response = new Response(true, "", "", "", users);
                else
                    response = new Response(false, "info", "Informaci\u00F3n", "No se han encontrado usuarios registrados", null);
            }
            catch (Exception e)
            {
                response = new Response(false, "error", "Error", "Error al obtener los usuarios", e);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void countUserWithUserName(String userName)
        {
            bienestarEntities db = new bienestarEntities();
            int cantidad = db.BE_USUARIO.Where(u => userName != null && u.NOMBREUSUARIO == userName).Count();
            writeResponse("{\"cantidad\":" + cantidad + "}");
        }

        [WebMethod]
        public void countUserWithCedula(string cedula)
        {
            bienestarEntities db = new bienestarEntities();
            int cantidad = db.BE_USUARIO.Where(u => cedula != null && u.CEDULA == cedula).Count();
            writeResponse("{\"cantidad\":" + cantidad + "}");
        }

        [WebMethod(EnableSession = true)]
        public void saveUserData(BE_USUARIO user, Boolean resetPassword)
        {
            Response response = new Response(false, "", "", "No tiene acceso", null);

            if (Utils.haveAccessTo(Utils.MODULOUSUARIO))
            {
                bienestarEntities db = new bienestarEntities();

                try
                {
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

                    response = new Response(true, "info", "Actualizar", "Usuario actualizado correctamente", usuario);
                }
                catch (InvalidOperationException)
                {
                    response = new Response(false, "error", "Error", "Error al obtener los datos para actualizar el usuario", null);
                    writeResponse(new JavaScriptSerializer().Serialize(response));
                }
                catch (Exception)
                {
                    response = new Response(false, "error", "Error", "Error al actualizar el usuario", null);
                    writeResponse(new JavaScriptSerializer().Serialize(response));
                }
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod(EnableSession = true)]
        public void addNewUser(BE_USUARIO newUser)
        {
            Response response = new Response(false, "", "", "No tiene acceso", null);
            if (Utils.haveAccessTo(Utils.MODULOUSUARIO))
            {
                bienestarEntities db = new bienestarEntities();

                try
                {
                    newUser.CONTRASENAACTUAL = newUser.CEDULA;
                    newUser.CONTRASENAANTERIOR = newUser.CEDULA;

                    db.BE_USUARIO.AddObject(newUser);

                    db.SaveChanges();

                    response = new Response(true, "info", "Agregar", "El usuario agregado correctamente", newUser);
                }
                catch (Exception)
                {
                    response = new Response(false, "error", "Error", "Error al agregar el usuario", null);
                    writeResponse(new JavaScriptSerializer().Serialize(response));
                }
            }
            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod(EnableSession = true)]
        public void removeUserById(int id)
        {
            Response response = new Response(false, "", "", "No tiene acceso", null);
            if (Utils.haveAccessTo(Utils.MODULOUSUARIO))
            {
                bienestarEntities db = new bienestarEntities();

                try
                {
                    BE_USUARIO usuario = db.BE_USUARIO.Single(u => u.CODIGO == id);

                    db.BE_USUARIO.DeleteObject(usuario);

                    db.SaveChanges();

                    response = new Response(true, "info", "Eliminar", "Usuario eliminado correctamente", null);
                }
                catch (InvalidOperationException)
                {
                    response = new Response(false, "error", "Error", "Error al obtener los datos para eliminar el usuario", null);
                    writeResponse(new JavaScriptSerializer().Serialize(response));
                }
                catch (Exception)
                {
                    response = new Response(false, "error", "Error", "Error al eliminar el usuario", null);
                    writeResponse(new JavaScriptSerializer().Serialize(response));
                }
            }
            writeResponse(new JavaScriptSerializer().Serialize(response));
        }
    }
}