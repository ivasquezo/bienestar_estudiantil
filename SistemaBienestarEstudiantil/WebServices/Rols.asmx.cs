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
         * Obtener todos los roles de la base de datos
         */
        [WebMethod]
        public void getAllRols()
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                // Conecta con las entidades
                bienestarEntities db = new bienestarEntities();

                // Busca roles
                List<ROL> rols = db.ROLs.ToList();

                // Verifica existencia de roles
                if (rols != null && rols.Count > 0)
                    response = new Response(true, "", "", "", rols);
                else
                    response = new Response(false, "info", "Información", "No se han encontrado roles", null);
            }
            catch (Exception e)
            {
                // Error al obtener roles
                response = new Response(false, "error", "Error", "Error al obtener roles", e);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        /**
         * Eliminar un rol
         */
        [WebMethod]
        public void removeRolById(int rolId)
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                // Conecta con las entidades
                bienestarEntities db = new bienestarEntities();

                // Obtiene el rol que va a eliminar
                ROL rolDeleted = db.ROLs.Single(r => r.CODIGO == rolId);

                // Elimina los accesos asignados al rol
                this.removeRolAccesByRolId(rolId);

                // Elimina el rol
                db.ROLs.DeleteObject(rolDeleted);
                db.SaveChanges();

                response = new Response(true, "info", "Eliminar", "Rol eliminado correctamente", null);
            }
            catch (InvalidOperationException)
            {
                // Error al eliminar el rol
                response = new Response(false, "error", "Error", "Error al obtener el rol para eliminar", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            catch (Exception)
            {
                // Error al eliminar el rol
                response = new Response(false, "error", "Error", "Error al eliminar el rol", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        /**
         * Eliminar los accesos de un rol
         */
        [WebMethod]
        private void removeRolAccesByRolId(int rolId)
        {
            List<ROL_ACCESO> rolAccessDeleted;
            bienestarEntities db;

            // Conecta con las entidades
            db = new bienestarEntities();

            // Obtiene los accesos del rol a eliminar
            rolAccessDeleted = db.ROL_ACCESO.Where(ra => ra.CODIGOROL == rolId).ToList();

            // Valida si existen accesos para eliminar
            if (rolAccessDeleted != null && rolAccessDeleted.Count > 0)
            {
                // Elimina los accesos del rol
                foreach (ROL_ACCESO rolAccess in rolAccessDeleted)
                    db.ROL_ACCESO.DeleteObject(rolAccess);

                db.SaveChanges();
            }
        }

        /**
         * Actualizar un rol
         */
        [WebMethod]
        public void saveRolData(int rolId, String rolName, int[] accessRols)
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                // Conecta con las entidades
                bienestarEntities db = new bienestarEntities();

                // Obtiene el rol que se va a actualizar
                ROL rolUpdated = db.ROLs.Single(r => r.CODIGO == rolId);

                Boolean actualizar = false;

                // Validacion del nombre del rol que se va a actualizar
                if (rolUpdated.NOMBRE == rolName)
                {
                    rolUpdated.NOMBRE = rolName;
                    actualizar = true;
                }
                else
                {
                    // Busca su el nombre existe
                    List<ROL> rolsExist = db.ROLs.Where(r => r.NOMBRE == rolName).ToList();

                    // Si el nombre del rol ya existe presenta un mensaje caso contrario actualiza el nombre
                    if (rolsExist != null && rolsExist.Count > 0)
                    {
                        response = new Response(false, "error", "Error", "El nombre del rol ya existe", null);
                        actualizar = false;
                    }
                    else
                    {
                        actualizar = true;
                        rolUpdated.NOMBRE = rolName;
                    }
                }

                if (actualizar)
                {
                    // Actualiza los accesos del rol
                    for (int accRol = 0; accRol < accessRols.Length; accRol++)
                    {
                        // Obtiene los estados de los accesos
                        ROL_ACCESO rolAccess = getStatusRolAccesById(rolId, accessRols[accRol]);

                        // Si el acceso existe lo actualiza caso contrario lo inserta
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
                // Error al eliminar el rol
                response = new Response(false, "error", "Error", "Error al obtener rol para actualizar", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            catch (Exception)
            {
                // Error al eliminar el rol
                response = new Response(false, "error", "Error", "Error al actualizar el rol", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        /**
         * Obtener los estados de los roles
         */
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

        /**
         * Obtener los accesos de un rol
         */
        [WebMethod]
        public void getAccessByRolId(int rolId)
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                // Conecta con las entidades
                bienestarEntities db = new bienestarEntities();

                // Obtiene todos los accesos existenten pero solo los accesos del rol saldran seleccionados
                var data = db.ACCESOes.Join(db.ROL_ACCESO, a => a.CODIGO, ra => ra.CODIGOACCESO,
                           (a, ra) => new { ACCESO = a, ROL_ACCESO = ra })
                           .Select(x => new { x.ACCESO.NOMBRE, x.ROL_ACCESO.ACCESO.CODIGO, x.ROL_ACCESO.CODIGOROL, x.ROL_ACCESO.VALIDO })
                           .Where(y => y.CODIGOROL == rolId).ToList();

                response = new Response(true, "", "", "", data);
            }
            catch(Exception)
            {
                // Error al obtener los accesos del rol
                response = new Response(false, "error", "Error", "Error al obtener los accesos del rol", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            
            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        /**
         * Obtener todos los accesos existentes
         */
        [WebMethod]
        public void getAllAccess()
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                // Conecta con las entidades
                bienestarEntities db = new bienestarEntities();

                // Obtiene todos los accesos existentes
                response = new Response(true, "", "", "", db.ACCESOes.ToList());
            }
            catch (Exception)
            {
                // Error al obtener los accesos del rol
                response = new Response(false, "error", "Error", "Error al obtener los accesos existentes", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        /**
         * Agregar un rol
         */
        [WebMethod]
        public void addNewRol(String rolName, int[] accessRols)
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                // Conecta con las entidades
                bienestarEntities db = new bienestarEntities();

                // Inicializa el rol que se va a agregar
                ROL newRol = new ROL();

                // Busca su el nombre existe
                List<ROL> rolsExist = db.ROLs.Where(r => r.NOMBRE == rolName).ToList();

                Boolean agregar = false;

                // Si el nombre del rol ya existe presenta un mensaje caso contrario actualiza el nombre
                if (rolsExist != null && rolsExist.Count > 0)
                {
                    response = new Response(false, "error", "Error", "El nombre del rol ya existe", null);
                    agregar = false;
                }
                else
                {
                    agregar = true;
                    newRol.NOMBRE = rolName;
                }

                if (agregar)
                {
                    // Agrega el rol
                    db.ROLs.AddObject(newRol);
                    db.SaveChanges();

                    // Agrega los accesos nuevos al rol
                    for (int accRol = 0; accRol < accessRols.Length; accRol++)
                        createRolAccess(Decimal.ToInt32(newRol.CODIGO), accessRols[accRol]);

                    response = new Response(true, "info", "Agregar", "El rol se agregó correctamente", newRol);
                }
            }
            catch (Exception)
            {
                // Error al obtener los accesos del rol
                response = new Response(false, "error", "Error", "Error al agregar el rol", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            
            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        /**
         * Actualiza los accesos del rol
         */
        [WebMethod]
        private void updateRolAccess(int idRol, int idAccess)
        {
            // Conecta con las entidades
            bienestarEntities db = new bienestarEntities();

            // Obtiene los accesos que tiene el rol
            ROL_ACCESO rolAccess = db.ROL_ACCESO.Single(ra => ra.CODIGOROL == idRol && ra.CODIGOACCESO == idAccess);

            // Si el acceso esta activo lo inactiva o viceversa
            if (rolAccess.VALIDO)
                rolAccess.VALIDO = false;
            else
                rolAccess.VALIDO = true;

            db.SaveChanges();
        }

        /**
         * Crea los accesos del rol
         */
        [WebMethod]
        private void createRolAccess(int rolId, int accessId)
        {
            // Conecta con las entidades
            bienestarEntities db = new bienestarEntities();

            // Inicializa los accesos de roles
            ROL_ACCESO newRolAccess = new ROL_ACCESO();

            // Setea los valores que contendrá el nuevo acceso de rol
            newRolAccess.CODIGOROL = rolId;
            newRolAccess.CODIGOACCESO = accessId;
            newRolAccess.VALIDO = true;

            db.ROL_ACCESO.AddObject(newRolAccess);

            db.SaveChanges();
        }
    }
}