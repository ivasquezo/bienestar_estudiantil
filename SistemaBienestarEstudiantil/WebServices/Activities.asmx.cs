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
    public class Activities : System.Web.Services.WebService
    {
        private void writeResponse(String response)
        {
            Context.Response.Write(response);
            Context.Response.Flush();
            Context.Response.End();
        }

        /**
         * Obtiene todas las actividades existentes
         */
        [WebMethod]
        public void getAllGeneralActivities()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                // Busca las actividades
                //var data = db.ACTIVIDAD_GENERAL.Join(db.ACTIVIDAD_GENERAL_ACTIVIDAD, ag => ag.CODIGO, aga => aga.CODIGOACTIVIDADGENERAL, 
                //    (ag, aga) => new { ag, aga }).Join(db.ACTIVIDADs, agaga => agaga.aga.CODIGOACTIVIDAD, a => a.CODIGO,
                //    (agaga, a) => new { agaga, a }).Select(x => new
                //    {
                //        CODIGOACTIVIDAD = x.agaga.ag.CODIGO,
                //        NOMBREACTIVIDAD = x.agaga.ag.NOMBRE,
                //        CODIGO = x.a.CODIGO,
                //        NOMBRE = x.a.NOMBRE,
                //        FECHA = x.a.FECHA,
                //        ESTADO = x.a.ESTADO
                //    }).ToList();
                //// Verifica si se encontraron resultados
                //if (data != null && data.Count > 0)
                //    response = new Response(true, "", "", "", data);
                //else
                    response = new Response(false, "info", "Información", "No se han encontrado actividades", null);
            }
            catch (Exception e)
            {
                response = new Response(false, "error", "Error", "Error al obtener las actividades", e);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        /**
         * Obtiene las actividades generales de la actividad a modificar
         */
        [WebMethod]
        public void getGeneralActivityByActivityId(int activityId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                // Busca las actividades generales de la actividad seleccionada
                //var data = db.ACTIVIDAD_GENERAL.Join(db.ACTIVIDAD_GENERAL_ACTIVIDAD, ag => ag.CODIGO, aga => aga.CODIGOACTIVIDADGENERAL,
                //           (ag, aga) => new { ACTIVIDAD_GENERAL = ag, ACTIVIDAD_GENERAL_ACTIVIDAD = aga })
                //           .Select(x => new { x.ACTIVIDAD_GENERAL.NOMBRE, x.ACTIVIDAD_GENERAL_ACTIVIDAD.ACTIVIDAD_GENERAL.CODIGO, x.ACTIVIDAD_GENERAL_ACTIVIDAD.CODIGOACTIVIDAD })
                //           .Where(y => y.CODIGOACTIVIDAD == activityId).ToList();

                //response = new Response(true, "", "", "", data);
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener las actividades generales", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        /**
         * Obtiene todas las actividades generales
         */
        [WebMethod]
        public void getAllGeneralActivity()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                // Busca todas las actividades generales
                response = new Response(true, "", "", "", db.ACTIVIDAD_GENERAL.ToList());
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener todas las actividades generales", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        /**
         * Obtiene todos los niveles academicos
         */
        [WebMethod]
        public void getAllGroupLevels()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                // Busca todos los niveles existentes
                response = new Response(true, "", "", "", db.GRUPOes.ToList());
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener los niveles academicos", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        /**
         * Obtiene los niveles asignados a una actividad
         */
        [WebMethod]
        public void getGroupLevelByActivityId(int activityId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                // Busca los niveles de la actividad seleccionada
                var data = db.GRUPOes.Join(db.GRUPO_ACTIVIDAD, g => g.CODIGO, ga => ga.CODIGOGRUPO,
                           (g, ga) => new { GRUPO = g, GRUPO_ACTIVIDAD = ga })
                           .Select(x => new { x.GRUPO_ACTIVIDAD.CODIGOACTIVIDAD, x.GRUPO_ACTIVIDAD.CODIGOGRUPO })
                           .Where(y => y.CODIGOACTIVIDAD == activityId).ToList();
                
                response = new Response(true, "", "", "", data);
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener los niveles de la actividad", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        /**
         * Busca todos los docentes registrados para realizar una actividad
         */
        [WebMethod]
        public void getAllResponsables()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                // Busca todos los docentes registrados
                var data = db.USUARIOs.Join(db.ROLs, u => u.CODIGOROL, r => r.CODIGO, 
                    (u, r) => new { USUARIO = u, ROL = r })
                    .Select(x => new { ROL = x.ROL.NOMBRE, CODIGO = x.USUARIO.CODIGO, NOMBRECOMPLETO = x.USUARIO.NOMBRECOMPLETO })
                    .Where(y => y.ROL == "DOCENTE").ToList();

                response = new Response(true, "", "", "", data);
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener los usuarios responsables", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void getResponsablesByActivity(int activityId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                 //rolAccess = db.ROL_ACCESO.Single(ra => ra.CODIGOROL == idRol && ra.CODIGOACCESO == idAccess);

                // Busca todos los docentes registrados
                var data = db.USUARIOs.Join(db.ROLs, u => u.CODIGOROL, r => r.CODIGO,
                    (u, r) => new { USUARIO = u, ROL = r })
                    .Select(x => new { ROL = x.ROL.NOMBRE, CODIGO = x.USUARIO.CODIGO, NOMBRECOMPLETO = x.USUARIO.NOMBRECOMPLETO })
                    .Where(y => y.ROL == "DOCENTE").ToList();

                response = new Response(true, "", "", "", data);
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener los usuarios responsables", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void removeActivityById(int activityId)
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                bienestarEntities db = new bienestarEntities();

                ACTIVIDAD activityDeleted = db.ACTIVIDADs.Single(a => a.CODIGO == activityId);
                //ACTIVIDAD_GENERAL_ACTIVIDAD activityGeneralDeleted = db.ACTIVIDAD_GENERAL_ACTIVIDAD.Single(ag => ag.CODIGOACTIVIDAD == activityId);
               
                //db.ACTIVIDADs.DeleteObject(activityDeleted);
                //db.ACTIVIDAD_GENERAL_ACTIVIDAD.DeleteObject(activityGeneralDeleted);
                //db.SaveChanges();

                response = new Response(true, "info", "Eliminar", "Actividad eliminada correctamente", null);
            }
            catch (InvalidOperationException)
            {
                // Error al eliminar el rol
                response = new Response(false, "error", "Error", "Error al obtener la actividad para eliminar", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            catch (Exception)
            {
                // Error al eliminar el rol
                response = new Response(false, "error", "Error", "Error al eliminar la actividad", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        

        /**
         * Actualizar una actividad
         */
        [WebMethod]
        public void saveActivityData(int activityId, String activityName, DateTime activityDate, int activityStatus, String activityObservation)
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                // Conecta con las entidades
                bienestarEntities db = new bienestarEntities();

                // Obtiene la actividad que se va a actualizar
                ACTIVIDAD activityUpdated = db.ACTIVIDADs.Single(a => a.CODIGO == activityId);

                Boolean actualizar = false;

                // Validacion del nombre de la actividad que se va a actualizar
                if (activityUpdated.NOMBRE == activityName.ToUpper())
                    actualizar = true;
                else
                {
                    // Busca su el nombre existe
                    List<ACTIVIDAD> activitiesExist = db.ACTIVIDADs.Where(a => a.NOMBRE == activityName.ToUpper()).ToList();

                    // Si el nombre del rol ya existe presenta un mensaje caso contrario actualiza el nombre
                    if (activitiesExist != null && activitiesExist.Count > 0)
                    {
                        response = new Response(false, "error", "Error", "El nombre de la actividad ya existe", null);
                        actualizar = false;
                    }
                    else
                        actualizar = true;
                }

                if (actualizar)
                {
                    activityUpdated.NOMBRE = activityName.ToUpper();
                    activityUpdated.FECHA = activityDate;
                    activityUpdated.ESTADO = activityStatus;
                    if (activityObservation != null)
                        activityUpdated.OBSERVACION = activityObservation.ToUpper();
                    else
                        activityUpdated.OBSERVACION = null;

                    db.SaveChanges();

                    response = new Response(true, "info", "Actualizar", "Actividad actualizada correctamente", null);
                }
            }
            catch (InvalidOperationException)
            {
                // Error al actualziar la actividad
                response = new Response(false, "error", "Error", "Error al obtener la actividad para actualizarla", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            catch (Exception)
            {
                // Error al eliminar el rol
                response = new Response(false, "error", "Error", "Error al actualizar la actividad", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        /**
         * Agregar un rol
         */
        [WebMethod]
        public void addNewActivity(String activityName, DateTime activityDate, int activityStatus, String activityObservation)
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                // Conecta con las entidades
                bienestarEntities db = new bienestarEntities();

                // Inicializa la actividad que se va a agregar
                ACTIVIDAD newActivity = new ACTIVIDAD();

                // Busca si el nombre existe
                List<ACTIVIDAD> activitiesExist = db.ACTIVIDADs.Where(a => a.NOMBRE == activityName).ToList();

                Boolean agregar = false;

                // Si el nombre del rol ya existe presenta un mensaje caso contrario actualiza el nombre
                if (activitiesExist != null && activitiesExist.Count > 0)
                {
                    response = new Response(false, "error", "Error", "El nombre de la actividad ya existe", null);
                    agregar = false;
                }
                else
                    agregar = true;

                if (agregar)
                {
                    newActivity.NOMBRE = activityName.ToUpper();
                    newActivity.FECHA = activityDate;
                    newActivity.ESTADO = activityStatus;
                    if (activityObservation != null || activityObservation != "")
                        newActivity.OBSERVACION = activityObservation.ToUpper();
                    else
                        newActivity.OBSERVACION = null;

                    // Agrega la actividad
                    db.ACTIVIDADs.AddObject(newActivity);
                    db.SaveChanges();

                    response = new Response(true, "info", "Agregar", "La actividad se agregó correctamente", newActivity);
                }
            }
            catch (Exception)
            {
                // Error al agregar la actividad
                response = new Response(false, "error", "Error", "Error al agregar la actividad", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void getAllUsersByRol()
        {
            Response response = new Response(true, "", "", "", null);

            // Conecta con las entidades
            bienestarEntities db = new bienestarEntities();

            // Trae los usuarios en base
            List<USUARIO> users = db.USUARIOs.ToList();
            List<USUARIO> usersByRol = new List<USUARIO>();

            // Si existen usuarios
            if (users != null && users.Count > 0)
            {
                // Verificar la existencia de roles en cada usuario
                //foreach (USUARIO user in users)
                //    if (user.USUARIO_ROL != null && user.USUARIO_ROL.Count > 0)
                //        foreach (ROL rol in user.USUARIO_ROL)
                //            if (rol.NOMBRE == "DOCENTE")
                //                usersByRol.Add(user);
            }

            if (usersByRol != null && usersByRol.Count > 0)
                response = new Response(true, "", "", "", usersByRol);
            else
                response = new Response(false, "error", "Error", "Usuarios y asignarles el rol de Docente", null);

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void getAllLevels()
        {
            Response response = new Response(true, "", "", "", null);

            // Conecta con las entidades
            bienestarEntities db = new bienestarEntities();

            // Trae los niveles en base
            List<GRUPO> groups = db.GRUPOes.ToList();

            // Si existen usuarios
            if (groups != null && groups.Count > 0)
                response = new Response(true, "", "", "", groups);
            else
                response = new Response(false, "error", "Error", "No existen niveles registrados", groups);

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }
    }
}
