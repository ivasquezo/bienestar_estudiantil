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
         * Obtener todas los actividades de la base de datos
         */
        [WebMethod]
        public void getAllActivities()
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                // Conecta con las entidades
                bienestarEntities db = new bienestarEntities();

                // Busca actividades
                List<ACTIVIDAD> activities = db.ACTIVIDADs.ToList();

                // Verifica existencia de actividades
                if (activities != null && activities.Count > 0)
                {
                    response = new Response(true, "", "", "", activities);
                }
                else
                    response = new Response(false, "info", "Información", "No se han encontrado actividades", null);
            }
            catch (Exception e)
            {
                // Error al obtener actividades
                response = new Response(false, "error", "Error", "Error al obtener las actividades", e);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        /**
         * Eliminar un rol
         */
        [WebMethod]
        public void removeActivityById(int activityId)
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                // Conecta con las entidades
                bienestarEntities db = new bienestarEntities();

                // Obtiene la actividad que va a eliminar
                ACTIVIDAD activityDeleted = db.ACTIVIDADs.Single(a => a.CODIGO == activityId);

                // Elimina la actividad
                db.ACTIVIDADs.DeleteObject(activityDeleted);
                db.SaveChanges();

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
        public void saveActivityData(int activityId, String activityName, DateTime activityDate, Boolean activityStatus, String activityObservation)
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
    }
}
