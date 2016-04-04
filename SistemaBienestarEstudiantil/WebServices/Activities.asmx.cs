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
using System.IO;

namespace SistemaBienestarEstudiantil.WebServices
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio Web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la linea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class Activities : System.Web.Services.WebService
    {
        private void writeResponse(String response)
        {
            Context.Response.Write(response);
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void getAllGeneralActivities()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                var data = db.BE_ACTIVIDAD_GENERAL.Join(db.BE_ACTIVIDAD, ag => ag.CODIGO, a => a.CODIGOACTIVIDADGENERAL,
                    (ag, a) => new { ACTIVIDAD_GENERAL = ag, ACTIVIDAD = a })
                    .Select(x => new {
                        CODIGOACTIVIDAD = x.ACTIVIDAD_GENERAL.CODIGO,
                        NOMBREACTIVIDAD = x.ACTIVIDAD_GENERAL.NOMBRE,
                        CODIGO = x.ACTIVIDAD.CODIGO,
                        NOMBRE = x.ACTIVIDAD.NOMBRE,
                        FECHA = x.ACTIVIDAD.FECHA,
                        ESTADO = x.ACTIVIDAD.ESTADO,
                        OBSERVACION = x.ACTIVIDAD.OBSERVACION,
                        CODIGOUSUARIO = x.ACTIVIDAD.CODIGOUSUARIO
                    }).OrderBy(y => y.NOMBREACTIVIDAD).ToList();

                if (data != null && data.Count > 0)
                    response = new Response(true, "", "", "", data);
                else
                    response = new Response(false, "info", "Informaci\u00F3n", "No se han encontrado actividades registradas", null);
            }
            catch (Exception e)
            {
                response = new Response(false, "error", "Error", "Error al obtener las actividades", e);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void getAllGeneralActivity()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                response = new Response(true, "", "", "", db.BE_ACTIVIDAD_GENERAL.ToList());
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener todas las actividades generales", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void getAllResponsables()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                var data = db.BE_USUARIO.Join(db.BE_ROL, u => u.CODIGOROL, r => r.CODIGO,
                    (u, r) => new { USUARIO = u, ROL = r })
                    .Select(x => new { ROL = x.ROL.NOMBRE, CODIGO = x.USUARIO.CODIGO, NOMBRECOMPLETO = x.USUARIO.NOMBRECOMPLETO })
                    .Where(y => y.ROL == "DOCENTE").ToList();

                if (data != null && data.Count > 0)
                    response = new Response(true, "", "", "", data);
                else
                    response = new Response(false, "info", "Informaci\u00F3n", "No existen personas responsables que se har\u00E1n cargo de la actividad", data);
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener los usuarios responsables", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void countActivityWithName(String activityName)
        {
            bienestarEntities db = new bienestarEntities();
            int cantidad = db.BE_ACTIVIDAD.Where(a => activityName != null && a.NOMBRE == activityName).Count();
            writeResponse("{\"cantidad\":" + cantidad + "}");
        }

        [WebMethod]
        public void saveActivityData(int activityId, String activityName, DateTime activityDate, int activityStatus,
            String activityObservation, int generalActivityId, int userId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                BE_ACTIVIDAD activityUpdated = db.BE_ACTIVIDAD.Single(a => a.CODIGO == activityId);

                activityUpdated.NOMBRE = activityName.ToUpper();
                activityUpdated.FECHA = activityDate;
                activityUpdated.ESTADO = activityStatus;
                if (activityObservation != null)
                    activityUpdated.OBSERVACION = activityObservation.ToUpper();
                else
                    activityUpdated.OBSERVACION = null;
                activityUpdated.CODIGOACTIVIDADGENERAL = generalActivityId;
                activityUpdated.CODIGOUSUARIO = userId;

                db.SaveChanges();

                response = new Response(true, "info", "Actualizar", "Actividad actualizada correctamente", null);
            }
            catch (InvalidOperationException)
            {
                response = new Response(false, "error", "Error", "Error al obtener los datos para actualiar la actividad", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al actualizar la actividad", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void addNewActivity(String activityName, DateTime activityDate, int activityStatus,
            String activityObservation, int generalActivityId, int userId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                BE_ACTIVIDAD newActivity = new BE_ACTIVIDAD();

                newActivity.NOMBRE = activityName;
                newActivity.FECHA = activityDate;
                newActivity.ESTADO = activityStatus;
                if (activityObservation != null || activityObservation != "")
                    newActivity.OBSERVACION = activityObservation;
                else
                    newActivity.OBSERVACION = null;
                newActivity.CODIGOACTIVIDADGENERAL = generalActivityId;
                newActivity.CODIGOUSUARIO = userId;

                db.BE_ACTIVIDAD.AddObject(newActivity);

                db.SaveChanges();

                response = new Response(true, "info", "Agregar", "Actividad agregada correctamente", newActivity);
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al agregar la actividad", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void getAllFaculties()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                response = new Response(true, "", "", "", db.FACULTADs.ToList());
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener las facultades", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void getAllSchools()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                response = new Response(true, "", "", "", db.ESCUELAs.ToList());
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener las escuelas", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void getAllCareers()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                response = new Response(true, "", "", "", db.CARRERAs.ToList());
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener las carreras", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void getAllModalities()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                response = new Response(true, "", "", "", db.MODALIDADs.ToList());
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener las modalidades", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void getAllLevels()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                response = new Response(true, "", "", "", db.NIVELs.ToList());
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener los niveles", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }




        

        

        




        [WebMethod]
        private void createGroupActivities(int activityId, int groupId)
        {
            bienestarEntities db = new bienestarEntities();

            BE_GRUPO_ACTIVIDAD newGroupActivity = new BE_GRUPO_ACTIVIDAD();

            newGroupActivity.CODIGOACTIVIDAD = activityId;
            newGroupActivity.CODIGOGRUPO = groupId;
            newGroupActivity.ESTADO = true;

            createStudentsByGroup(activityId, groupId);

            db.BE_GRUPO_ACTIVIDAD.AddObject(newGroupActivity);

            db.SaveChanges();
        }

        

        [WebMethod]
        public void getAllGroupLevels()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                response = new Response(true, "", "", "", db.BE_GRUPO.ToList());
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener los niveles acad\u00E9micos", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void getGroupLevelByActivityId(int activityId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                // Busca los niveles de la actividad seleccionada
                var data = db.BE_GRUPO.Join(db.BE_GRUPO_ACTIVIDAD, g => g.CODIGO, ga => ga.CODIGOGRUPO,
                           (g, ga) => new { GRUPO = g, GRUPO_ACTIVIDAD = ga })
                           .Select(x => new 
                           { 
                               x.GRUPO_ACTIVIDAD.CODIGOACTIVIDAD, 
                               x.GRUPO_ACTIVIDAD.CODIGOGRUPO, 
                               ESTADO = x.GRUPO_ACTIVIDAD.ESTADO, 
                               x.GRUPO.NIVEL, 
                               //x.GRUPO.PARALELO, MICHEL CAMBIO
                               x.GRUPO.MODALIDAD })
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
         * Verifica la existencia del nivel
         */
        [WebMethod]
        private BE_GRUPO_ACTIVIDAD getStatusGroupActivity(int activityId, int groupId)
        {
            bienestarEntities db = new bienestarEntities();
            BE_GRUPO_ACTIVIDAD groupActivity = null;

            try
            {
                groupActivity = db.BE_GRUPO_ACTIVIDAD.Single(ga => ga.CODIGOGRUPO == groupId && ga.CODIGOACTIVIDAD == activityId);
            }
            catch (System.InvalidOperationException)
            {
                Console.WriteLine("No se encontró un elemento con los filtros ingresados");
            }

            return groupActivity;
        }

        /**
         * Actualiza los estados del los niveles
         */
        [WebMethod]
        private void updateGroupActivities(int activityId, int groupId)
        {
            bienestarEntities db = new bienestarEntities();

            BE_GRUPO_ACTIVIDAD groupActivity = db.BE_GRUPO_ACTIVIDAD.Single(ga => ga.CODIGOACTIVIDAD == activityId && ga.CODIGOGRUPO == groupId);

            if (groupActivity.ESTADO == true)
            {
                groupActivity.ESTADO = false;
                deleteStudentsByGroupActivity(activityId, groupId);
            }
            else
            {
                groupActivity.ESTADO = true;
                createStudentsByGroup(activityId, groupId);
            }

            db.SaveChanges();
        }

        private void deleteStudentsByGroupActivity(int activityId, int groupId)
        {
            bienestarEntities db = new bienestarEntities();

            List<BE_ASISTENCIA> assistanceList = db.BE_ASISTENCIA.Where(a => a.CODIGOGRUPO == groupId && a.CODIGOACTIVIDAD == activityId).ToList();

            foreach (BE_ASISTENCIA assistance in assistanceList)
            {
                db.BE_ASISTENCIA.DeleteObject(assistance);
                db.SaveChanges();
            }
        }

        private void createStudentsByGroup(int activityId, int groupId)
        {
            bienestarEntities db = new bienestarEntities();

            /*
            List<BE_ALUMNO> students = db.BE_ALUMNOes.Where(a => a.CODIGOGRUPO == groupId).ToList();

            foreach (BE_ALUMNO student in students)
            {
                BE_ASISTENCIA newAssistance = new BE_ASISTENCIA();

                newAssistance.CODIGOGRUPO = groupId;
                newAssistance.CODIGOALUMNO = student.CODIGO;
                newAssistance.CODIGOACTIVIDAD = activityId;
                newAssistance.ASISTENCIA = false;

                db.BE_ASISTENCIA.AddObject(newAssistance);

                db.SaveChanges();
            }
            MICHEL CAMBIO
            */
        }

        

        

        [WebMethod]
        public void getAssistanceList(int activityId, int levelId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                List<BE_ASISTENCIA> assistanceList = db.BE_ASISTENCIA.Where(a => a.CODIGOACTIVIDAD == activityId && a.CODIGOGRUPO == levelId).ToList();

                if (assistanceList != null && assistanceList.Count > 0)
                    response = new Response(true, "", "", "", assistanceList);
                else
                    response = new Response(false, "info", "Información", "No existen datos con el nivel seleccionado", assistanceList);
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener la asistencia de los alumnos", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void saveAssistanceData(int[] assistance)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                for (int assist = 0; assist < assistance.Length; assist++)
                {
                    int code = assistance[assist];
                    BE_ASISTENCIA student = db.BE_ASISTENCIA.Single(a => a.CODIGO == code);

                    if (student.ASISTENCIA == true)
                        student.ASISTENCIA = false;
                    else
                        student.ASISTENCIA = true;

                    db.SaveChanges();
                }

                response = new Response(true, "info", "Actualizar", "Asistencia registrada correctamente", null);
            }
            catch (InvalidOperationException)
            {
                response = new Response(false, "error", "Error", "Error al obtener los datos de asistencia para actualizarla", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al actualizar los datos de asistencia", null);
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

                BE_ACTIVIDAD activityDeleted = db.BE_ACTIVIDAD.Single(a => a.CODIGO == activityId);

                deleteAssistanceActivity(activityId);

                deleteGroupActivity(activityId);

                deleteAttachedActivity(activityId);

                db.BE_ACTIVIDAD.DeleteObject(activityDeleted);
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

        private void deleteAssistanceActivity(int activityId)
        {
            bienestarEntities db = new bienestarEntities();

            List<BE_ASISTENCIA> assistanceDeleted = db.BE_ASISTENCIA.Where(a => a.CODIGOACTIVIDAD == activityId).ToList();

            foreach (BE_ASISTENCIA assistance in assistanceDeleted)
            {
                db.BE_ASISTENCIA.DeleteObject(assistance);
                db.SaveChanges();
            }
        }

        private void deleteGroupActivity(int activityId)
        {
            bienestarEntities db = new bienestarEntities();

            List<BE_GRUPO_ACTIVIDAD> groupDeleted = db.BE_GRUPO_ACTIVIDAD.Where(ga => ga.CODIGOACTIVIDAD == activityId).ToList();

            foreach (BE_GRUPO_ACTIVIDAD group in groupDeleted)
            {
                db.BE_GRUPO_ACTIVIDAD.DeleteObject(group);
                db.SaveChanges();
            }
        }

        private void deleteAttachedActivity(int activityId)
        {
            bienestarEntities db = new bienestarEntities();

            List<BE_ACTIVIDAD_ADJUNTO> attachedDeleted = db.BE_ACTIVIDAD_ADJUNTO.Where(aa => aa.CODIGOACTIVIDAD == activityId).ToList();

            foreach (BE_ACTIVIDAD_ADJUNTO attached in attachedDeleted)
            {
                db.BE_ACTIVIDAD_ADJUNTO.DeleteObject(attached);
                db.SaveChanges();
            }
        }

        //[WebMethod]
        //public void addUploadedFileDataBase()
        //{
        //    Response response = new Response(true, "", "", "", null);
        //    bienestarEntities db = new bienestarEntities();

        //    try {
        //        HttpFileCollection hfc = HttpContext.Current.Request.Files;

        //        if (hfc.Count > 0)
        //        {
        //            ACTIVIDAD_ADJUNTO activityAttached = new ACTIVIDAD_ADJUNTO();
        //            HttpPostedFile hpf = hfc[0];
        //            if (hpf.ContentLength > 0)
        //            {
        //                using (var memoryStream = new MemoryStream())
        //                {
        //                    hpf.InputStream.CopyTo(memoryStream);
        //                    byte[] fileBytes = memoryStream.ToArray();

        //                    activityAttached = new ACTIVIDAD_ADJUNTO();
        //                    activityAttached.CODIGOACTIVIDAD = 7;
        //                    activityAttached.ADJUNTO = fileBytes;
        //                    activityAttached.NOMBRE = hfc[0].FileName;
        //                    activityAttached.CONTENTTYPE = hfc[0].ContentType;
        //                }
        //            }

        //            db.ACTIVIDAD_ADJUNTO.AddObject(activityAttached);
        //            db.SaveChanges();
        //        }
            
        //        response = new Response(true, "info", "Información", "El archivo se adjuntó correctamente", null);
        //    }
        //    catch (Exception)
        //    {
        //        response = new Response(false, "error", "Error", "Error al adjuntar el archivo", null);
        //        writeResponse(new JavaScriptSerializer().Serialize(response));
        //    }

        //    writeResponse(new JavaScriptSerializer().Serialize(response));
        //}

        [WebMethod]
        public void addUploadedFileDataBase()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                HttpFileCollection hfc = HttpContext.Current.Request.Files;

                if (hfc.Count > 0)
                {
                    BE_ACTIVIDAD_ADJUNTO activityAttached = new BE_ACTIVIDAD_ADJUNTO();
                    HttpPostedFile hpf = hfc[0];
                    if (hpf.ContentLength > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            hpf.InputStream.CopyTo(memoryStream);
                            byte[] fileBytes = memoryStream.ToArray();

                            activityAttached = new BE_ACTIVIDAD_ADJUNTO();
                            activityAttached.ADJUNTO = fileBytes;
                            activityAttached.NOMBRE = hfc[0].FileName;
                            activityAttached.CONTENTTYPE = hfc[0].ContentType;
                        }
                    }

                    response = new Response(true, "", "", "", activityAttached);
                }
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al adjuntar el archivo", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void saveActivityAttached(BE_ACTIVIDAD_ADJUNTO activityAttached, int activityId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try {
                BE_ACTIVIDAD_ADJUNTO activityCreate = activityAttached;
                activityCreate.CODIGOACTIVIDAD = activityId;

                db.BE_ACTIVIDAD_ADJUNTO.AddObject(activityCreate);
                db.SaveChanges();

                response = new Response(true, "info", "Información", "El archivo se adjuntó correctamente", null);
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al adjuntar el archivo", null);
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
            List<BE_USUARIO> users = db.BE_USUARIO.ToList();
            List<BE_USUARIO> usersByRol = new List<BE_USUARIO>();

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
    }
}
