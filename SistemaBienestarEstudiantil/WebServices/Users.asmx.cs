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

        private void writeResponse(Object response)
        {
            Context.Response.Write(new JavaScriptSerializer().Serialize(response));
            Context.Response.Flush();
            Context.Response.End();
        }
        /// <summary>
        /// Validacion del usuario logeado
        /// </summary>
        [WebMethod(EnableSession = true)]
        public void checkSession()
        {
            bool isLogged = Utils.isLogged();
            Response response = new Response(isLogged, null, null, null, null);
            if (!isLogged) Utils.terminateSession();
            writeResponse(new JavaScriptSerializer().Serialize(response));
        }
        /// <summary>
        /// Obtener todos los usuarios registrados
        /// </summary>
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
        /// <summary>
        /// Contabilizar los usuarios para validar si se ingresa un usuario con un nombre de usuario repetido
        /// </summary>
        /// <param name="userName"></param>
        [WebMethod]
        public void countUserWithUserName(String userName)
        {
            bienestarEntities db = new bienestarEntities();
            int cantidad = db.BE_USUARIO.Where(u => userName != null && u.NOMBREUSUARIO == userName).Count();
            writeResponse("{\"cantidad\":" + cantidad + "}");
        }
        /// <summary>
        /// Contabilizar los usuarios para validar si se ingresa un usuario con una cedula repetida
        /// </summary>
        /// <param name="cedula"></param>
        [WebMethod]
        public void countUserWithCedula(string cedula)
        {
            bienestarEntities db = new bienestarEntities();
            int cantidad = db.BE_USUARIO.Where(u => cedula != null && u.CEDULA == cedula).Count();
            writeResponse("{\"cantidad\":" + cantidad + "}");
        }
        /// <summary>
        /// Registrar la edicion de un usuario
        /// </summary>
        /// <param name="user"></param>
        /// <param name="resetPassword"></param>
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
                        usuario.CONTRASENAACTUAL = Utils.Encripta(user.CEDULA);
                        usuario.CONTRASENAANTERIOR = Utils.Encripta(user.CEDULA);
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
        /// <summary>
        /// Insertar un nuevo usuario
        /// </summary>
        /// <param name="newUser"></param>
        [WebMethod(EnableSession = true)]
        public void addNewUser(BE_USUARIO newUser)
        {
            Response response = new Response(false, "", "", "No tiene acceso", null);
            if (Utils.haveAccessTo(Utils.MODULOUSUARIO))
            {
                bienestarEntities db = new bienestarEntities();
                try
                {
                    newUser.CONTRASENAACTUAL = Utils.Encripta(newUser.CEDULA);
                    newUser.CONTRASENAANTERIOR = Utils.Encripta(newUser.CEDULA);
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
        /// <summary>
        /// Obtener el correo electronico registrado para recibir notificaciones
        /// </summary>
        [WebMethod]
        public void getEmailBecasMail()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();
            try
            {
                var email = db.BE_DATOS_SISTEMA.Select(s => new
                {
                    NOMBRE = s.NOMBRE,
                    EMAIL = s.VALOR
                }).Where(w => w.NOMBRE == Utils.BECANOTIFICACION).FirstOrDefault();
                if (email != null)
                    response = new Response(true, "", "", "", email);
                else
                    response = new Response(false, "info", "Informaci\u00F3n", "No se han encontrado correos electr\u00F3nico registrados", null);
            }
            catch (Exception e)
            {
                response = new Response(false, "error", "Error", "Error al obtener los correos electr\u00F3nico registrados", e);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            writeResponse(new JavaScriptSerializer().Serialize(response));
        }
        /// <summary>
        /// Insertar un nuevo correo electronico de quien recibira las notificaciones
        /// </summary>
        /// <param name="newUser"></param>
        [WebMethod]
        public void addNewMail(String mailNotification)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();
            try
            {
                BE_DATOS_SISTEMA newData = db.BE_DATOS_SISTEMA.Single(w => w.NOMBRE == Utils.BECANOTIFICACION);
                newData.NOMBRE = Utils.BECANOTIFICACION;
                newData.VALOR = mailNotification;
                db.SaveChanges();
                response = new Response(true, "info", "Actualizar", "El correo electr\u00F3nico ha sido agregado correctamente", null);
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al agregar el correo electr\u00F3nico", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            writeResponse(new JavaScriptSerializer().Serialize(response));
        }
    }
}