using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data.Objects.DataClasses;
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
    public class Rols : System.Web.Services.WebService
    {

        private void writeResponse(String response)
        {
            Context.Response.Write(response);
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void getAllRols()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                List<BE_ROL> rols = db.BE_ROL.ToList();
                
                if (rols != null && rols.Count > 0)
                    response = new Response(true, "", "", "", rols);
                else
                    response = new Response(false, "info", "Informaci\u00F3n", "No se han encontrado roles registrados", null);
            }
            catch (Exception e)
            {
                response = new Response(false, "error", "Error", "Error al obtener roles", e);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void removeRolById(int rolId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                BE_ROL rolDeleted = db.BE_ROL.Single(r => r.CODIGO == rolId);

                this.removeRolAccesByRolId(rolId);

                this.changeUserRol(rolId);

                db.BE_ROL.DeleteObject(rolDeleted);

                db.SaveChanges();

                response = new Response(true, "info", "Eliminar", "Rol eliminado correctamente", null);
            }
            catch (InvalidOperationException)
            {
                response = new Response(false, "error", "Error", "Error al obtener los datos para eliminar el rol", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al eliminar el rol", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        private void removeRolAccesByRolId(int rolId)
        {
            bienestarEntities db = new bienestarEntities();

            List<BE_ROL_ACCESO> rolAccessDeleted = db.BE_ROL_ACCESO.Where(ra => ra.CODIGOROL == rolId).ToList();

            if (rolAccessDeleted != null && rolAccessDeleted.Count > 0)
            {
                foreach (BE_ROL_ACCESO rolAccess in rolAccessDeleted)
                    db.BE_ROL_ACCESO.DeleteObject(rolAccess);

                db.SaveChanges();
            }
        }

        [WebMethod]
        private void changeUserRol(int rolId)
        {
            bienestarEntities db = new bienestarEntities();

            List<BE_USUARIO> userChanged = db.BE_USUARIO.Where(u => u.CODIGOROL == rolId).ToList();

            BE_ROL rolGuess = db.BE_ROL.Single(r => r.NOMBRE == "INVITADO");

            if (userChanged != null && userChanged.Count > 0)
            {
                foreach (BE_USUARIO user in userChanged)
                    user.CODIGOROL = rolGuess.CODIGO;

                db.SaveChanges();
            }
        }

        [WebMethod]
        public void saveRolData(int rolId, String rolName, int[] accessRols)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                BE_ROL rolUpdated = db.BE_ROL.Single(r => r.CODIGO == rolId);

                rolUpdated.NOMBRE = rolName;

                for (int accRol = 0; accRol < accessRols.Length; accRol++)
                {
                    BE_ROL_ACCESO rolAccess = getStatusRolAccesById(rolId, accessRols[accRol]);

                    if (rolAccess != null)
                        updateRolAccess(rolId, accessRols[accRol]);
                    else
                        createRolAccess(rolId, accessRols[accRol]);
                }

                db.SaveChanges();

                response = new Response(true, "info", "Actualizar", "Rol actualizado correctamente", null);
            }
            catch (InvalidOperationException)
            {
                response = new Response(false, "error", "Error", "Error al obtener los datos para actualizar el rol", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al actualizar el rol", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        private BE_ROL_ACCESO getStatusRolAccesById(int idRol, int idAccess)
        {
            bienestarEntities db = new bienestarEntities();

            BE_ROL_ACCESO rolAccess = null;

            try
            {
                rolAccess = db.BE_ROL_ACCESO.Single(ra => ra.CODIGOROL == idRol && ra.CODIGOACCESO == idAccess);
            }
            catch (System.InvalidOperationException)
            {
                Console.WriteLine("No se encontr\u00F3 un elemento con los filtros ingresados");
            }

            return rolAccess;
        }

        [WebMethod]
        public void getAccessByRolId(int rolId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                var data = db.BE_ACCESO.Join(db.BE_ROL_ACCESO, a => a.CODIGO, ra => ra.CODIGOACCESO, 
                    (a, ra) => new { ACCESO = a, ROL_ACCESO = ra })
                    .Select(x => new { x.ACCESO.NOMBRE, x.ROL_ACCESO.BE_ACCESO.CODIGO, x.ROL_ACCESO.CODIGOROL, x.ROL_ACCESO.VALIDO })
                    .Where(y => y.CODIGOROL == rolId).ToList();

                response = new Response(true, "", "", "", data);
            }
            catch(Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener los accesos del rol", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            
            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void getAllAccess()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                response = new Response(true, "", "", "", db.BE_ACCESO.ToList());
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener los accesos existentes", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void addNewRol(String rolName, int[] accessRols)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                BE_ROL newRol = new BE_ROL();

                newRol.NOMBRE = rolName;

                db.BE_ROL.AddObject(newRol);
                db.SaveChanges();

                for (int accRol = 0; accRol < accessRols.Length; accRol++)
                    createRolAccess(Decimal.ToInt32(newRol.CODIGO), accessRols[accRol]);

                response = new Response(true, "info", "Agregar", "Rol agregado correctamente", newRol);
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al agregar el rol", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            
            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        private void updateRolAccess(int idRol, int idAccess)
        {
            bienestarEntities db = new bienestarEntities();

            BE_ROL_ACCESO rolAccess = db.BE_ROL_ACCESO.Single(ra => ra.CODIGOROL == idRol && ra.CODIGOACCESO == idAccess);

            if (rolAccess.VALIDO)
                rolAccess.VALIDO = false;
            else
                rolAccess.VALIDO = true;

            db.SaveChanges();
        }

        [WebMethod]
        private void createRolAccess(int rolId, int accessId)
        {
            bienestarEntities db = new bienestarEntities();

            BE_ROL_ACCESO newRolAccess = new BE_ROL_ACCESO();

            newRolAccess.CODIGOROL = rolId;
            newRolAccess.CODIGOACCESO = accessId;
            newRolAccess.VALIDO = true;

            db.BE_ROL_ACCESO.AddObject(newRolAccess);

            db.SaveChanges();
        }

        [WebMethod]
        public void countRolWithName(String rolName)
        {
            bienestarEntities db = new bienestarEntities();
            int cantidad = db.BE_ROL.Where(r => rolName != null && r.NOMBRE == rolName).Count();
            writeResponse("{\"cantidad\":" + cantidad + "}");
        }
    }
}