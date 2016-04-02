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

        /**
         * Obtener todos los roles existentes
         */
        [WebMethod]
        public void getAllRols()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                // Busca todos los roles
                List<BE_ROL> rols = db.BE_ROL.ToList();
                // Verifica si la busqueda retorno valores
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

        /**
         * Elimina rol seleccionado
         */
        [WebMethod]
        public void removeRolById(int rolId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                // Busca rol a eliminar
                BE_ROL rolDeleted = db.BE_ROL.Single(r => r.CODIGO == rolId);
                // Elimina los accesos del rol
                this.removeRolAccesByRolId(rolId);
                // Elimina el rol
                db.BE_ROL.DeleteObject(rolDeleted);
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

        /**
         * Elimina los accesos del rol seleccionado
         */
        [WebMethod]
        private void removeRolAccesByRolId(int rolId)
        {
            bienestarEntities db = new bienestarEntities();
            // Busca los accesos del rol
            List<BE_ROL_ACCESO> rolAccessDeleted = db.BE_ROL_ACCESO.Where(ra => ra.CODIGOROL == rolId).ToList();
            // Si se encontraron accesos se los elimina
            if (rolAccessDeleted != null && rolAccessDeleted.Count > 0)
            {
                foreach (BE_ROL_ACCESO rolAccess in rolAccessDeleted)
                    db.BE_ROL_ACCESO.DeleteObject(rolAccess);

                db.SaveChanges();
            }
        }

        /**
         * Guarda el rol modificado
         */
        [WebMethod]
        public void saveRolData(int rolId, String rolName, int[] accessRols)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                // Busca el rol a modificar
                BE_ROL rolUpdated = db.BE_ROL.Single(r => r.CODIGO == rolId);

                Boolean actualizar = false;
                // Verifica si el rol va a cambiar su nombre
                if (rolUpdated.NOMBRE == rolName)
                    actualizar = true;
                else
                {
                    // Verifica si existen otros roles con el nombre a cambiar
                    List<BE_ROL> rolsExist = db.BE_ROL.Where(r => r.NOMBRE == rolName).ToList();
                    // Si encontro roles con el mismo nombre no lo guarda
                    if (rolsExist != null && rolsExist.Count > 0)
                    {
                        response = new Response(false, "error", "Error", "El nombre del rol ya existe", null);
                        actualizar = false;
                    }
                    else
                        actualizar = true;
                }
                // Si no hay problema para actualiar el rol
                if (actualizar)
                {
                    rolUpdated.NOMBRE = rolName;
                    // Guarda los accesos seleccionados
                    for (int accRol = 0; accRol < accessRols.Length; accRol++)
                    {
                        // Verifica si el acceso ya existe
                        BE_ROL_ACCESO rolAccess = getStatusRolAccesById(rolId, accessRols[accRol]);
                        // Si el acceso existe se lo actualiza, caso contrario se lo agrega
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

        /**
         * Verifica la existencia del acceso
         */
        [WebMethod]
        private BE_ROL_ACCESO getStatusRolAccesById(int idRol, int idAccess)
        {
            bienestarEntities db = new bienestarEntities();

            BE_ROL_ACCESO rolAccess = null;

            try
            {
                // Busca el acceso del rol
                rolAccess = db.BE_ROL_ACCESO.Single(ra => ra.CODIGOROL == idRol && ra.CODIGOACCESO == idAccess);
            }
            catch (System.InvalidOperationException)
            {
                Console.WriteLine("No se encontró un elemento con los filtros ingresados");
            }

            return rolAccess;
        }

        /**
         * Obtiene los accesos del rol seleccionado
         */
        [WebMethod]
        public void getAccessByRolId(int rolId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                // Busca los accesos del rol
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

        /**
         * Obtiene todos los accesos existentes
         */
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

        /**
         * Crea un nuevo rol
         */
        [WebMethod]
        public void addNewRol(String rolName, int[] accessRols)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                BE_ROL newRol = new BE_ROL();
                // Verifica si existe un rol con el nombre que se desea guardar
                List<BE_ROL> rolsExist = db.BE_ROL.Where(r => r.NOMBRE == rolName).ToList();

                Boolean agregar = false;
                // Si ya existe ek rol no lo guarda
                if (rolsExist != null && rolsExist.Count > 0)
                {
                    response = new Response(false, "error", "Error", "El nombre del rol ya existe", null);
                    agregar = false;
                }
                else
                    agregar = true;
                // Si no hay problemas para crear el rol
                if (agregar)
                {
                    newRol.NOMBRE = rolName;
                    // Guarda el rol nuevo
                    db.BE_ROL.AddObject(newRol);
                    db.SaveChanges();
                    // Crea los accesos seleccionados para el rol nuevo
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

        /**
         * Actualiza los estados del los accesos existentes
         */
        [WebMethod]
        private void updateRolAccess(int idRol, int idAccess)
        {
            bienestarEntities db = new bienestarEntities();
            // Busca el acceso a actualizar
            BE_ROL_ACCESO rolAccess = db.BE_ROL_ACCESO.Single(ra => ra.CODIGOROL == idRol && ra.CODIGOACCESO == idAccess);
            // Verifica el estado del acceso para modificarlo
            if (rolAccess.VALIDO)
                rolAccess.VALIDO = false;
            else
                rolAccess.VALIDO = true;

            db.SaveChanges();
        }

        /**
         * Crea un acceso nuevo en determinado rol
         */
        [WebMethod]
        private void createRolAccess(int rolId, int accessId)
        {
            bienestarEntities db = new bienestarEntities();

            BE_ROL_ACCESO newRolAccess = new BE_ROL_ACCESO();
            // Setea los datos del acceso
            newRolAccess.CODIGOROL = rolId;
            newRolAccess.CODIGOACCESO = accessId;
            newRolAccess.VALIDO = true;

            db.BE_ROL_ACCESO.AddObject(newRolAccess);

            db.SaveChanges();
        }
    }
}