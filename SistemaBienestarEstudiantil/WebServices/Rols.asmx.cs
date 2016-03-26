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
    // Para permitir que se llame a este servicio Web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
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

            try
            {
                bienestarEntities db = new bienestarEntities();

                List<ROL> rols = db.ROLs.ToList();

                if (rols != null && rols.Count > 0)
                    response = new Response(true, "", "", "", rols);
                else
                    response = new Response(false, "info", "Información", "No se han encontrado roles", null);
            }
            catch (Exception e)
            {
                response = new Response(false, "error", "Error", "Error al obtener roles", e);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void getRolStatus(int rolId)
        {
            bienestarEntities db = new bienestarEntities();

            ROL rol = db.ROLs.Single(r => r.CODIGO == rolId);

            writeResponse(new JavaScriptSerializer().Serialize(rol));
        }

        [WebMethod]
        public void removeRolById(int rolId)
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                bienestarEntities db = new bienestarEntities();

                ROL rolDeleted = db.ROLs.Single(r => r.CODIGO == rolId);

                this.removeRolAccesByRolId(rolId);

                db.ROLs.DeleteObject(rolDeleted);
                db.SaveChanges();

                response = new Response(true, "info", "Eliminar", "Rol eliminado correctamente", null);
            }
            catch (InvalidOperationException)
            {
                response = new Response(false, "error", "Error", "Error al obtener el rol para eliminar", null);
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
            List<ROL_ACCESO> rolAccessDeleted;
            bienestarEntities db;

            db = new bienestarEntities();

            rolAccessDeleted = db.ROL_ACCESO.Where(ra => ra.CODIGOROL == rolId).ToList();

            if (rolAccessDeleted != null && rolAccessDeleted.Count > 0)
            {
                foreach (ROL_ACCESO rolAccess in rolAccessDeleted)
                    db.ROL_ACCESO.DeleteObject(rolAccess);

                db.SaveChanges();
            }
        }

        [WebMethod]
        public void saveRolData(int rolId, String rolName, int[] accessRols)
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                bienestarEntities db = new bienestarEntities();

                ROL rolUpdated = db.ROLs.Single(r => r.CODIGO == rolId);

                Boolean actualizar = false;

                if (rolUpdated.NOMBRE == rolName)
                    actualizar = true;
                else
                {
                    List<ROL> rolsExist = db.ROLs.Where(r => r.NOMBRE == rolName).ToList();

                    if (rolsExist != null && rolsExist.Count > 0)
                    {
                        response = new Response(false, "error", "Error", "El nombre del rol ya existe", null);
                        actualizar = false;
                    }
                    else
                        actualizar = true;
                }

                if (actualizar)
                {
                    rolUpdated.NOMBRE = rolName;

                    for (int accRol = 0; accRol < accessRols.Length; accRol++)
                    {
                        ROL_ACCESO rolAccess = getStatusRolAccesById(rolId, accessRols[accRol]);

                        if (rolAccess != null)
                            updateRolAccess(rolId, accessRols[accRol]);
                        else
                            createRolAccess(rolId, accessRols[accRol]);
                    }

                    db.SaveChanges();

                    response = new Response(true, "info", "Actualizar", "Rol actualizado correctamente", null);
                }
            }
            catch (InvalidOperationException)
            {
                response = new Response(false, "error", "Error", "Error al obtener rol para actualizar", null);
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
        private ROL_ACCESO getStatusRolAccesById(int idRol, int idAccess)
        {
            bienestarEntities db = new bienestarEntities();

            ROL_ACCESO rolAccess = null;

            try
            {
                rolAccess = db.ROL_ACCESO.Single(ra => ra.CODIGOROL == idRol && ra.CODIGOACCESO == idAccess);
            }
            catch (System.InvalidOperationException)
            {
                Console.WriteLine("No se encontró un elemento con los filtros ingresados");
            }

            return rolAccess;
        }

        [WebMethod]
        public void getAccessByRolId(int rolId)
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                bienestarEntities db = new bienestarEntities();

                var data = db.ACCESOes.Join(db.ROL_ACCESO, a => a.CODIGO, ra => ra.CODIGOACCESO,
                           (a, ra) => new { ACCESO = a, ROL_ACCESO = ra })
                           .Select(x => new { x.ACCESO.NOMBRE, x.ROL_ACCESO.ACCESO.CODIGO, x.ROL_ACCESO.CODIGOROL, x.ROL_ACCESO.VALIDO })
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

            try
            {
                bienestarEntities db = new bienestarEntities();

                response = new Response(true, "", "", "", db.ACCESOes.ToList());
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

            try
            {
                bienestarEntities db = new bienestarEntities();

                ROL newRol = new ROL();

                List<ROL> rolsExist = db.ROLs.Where(r => r.NOMBRE == rolName).ToList();

                Boolean agregar = false;

                if (rolsExist != null && rolsExist.Count > 0)
                {
                    response = new Response(false, "error", "Error", "El nombre del rol ya existe", null);
                    agregar = false;
                }
                else
                    agregar = true;

                if (agregar)
                {
                    newRol.NOMBRE = rolName;

                    db.ROLs.AddObject(newRol);
                    db.SaveChanges();

                    for (int accRol = 0; accRol < accessRols.Length; accRol++)
                        createRolAccess(Decimal.ToInt32(newRol.CODIGO), accessRols[accRol]);

                    response = new Response(true, "info", "Agregar", "El rol se agregó correctamente", newRol);
                }
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

            ROL_ACCESO rolAccess = db.ROL_ACCESO.Single(ra => ra.CODIGOROL == idRol && ra.CODIGOACCESO == idAccess);

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

            ROL_ACCESO newRolAccess = new ROL_ACCESO();

            newRolAccess.CODIGOROL = rolId;
            newRolAccess.CODIGOACCESO = accessId;
            newRolAccess.VALIDO = true;

            db.ROL_ACCESO.AddObject(newRolAccess);

            db.SaveChanges();
        }
    }
}