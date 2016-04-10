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
using System.Web.Script.Services;

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
        public void getAllGeneralActivitiesWithActivity()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                var data = db.BE_ACTIVIDAD_GENERAL.Join(db.BE_ACTIVIDAD, ag => ag.CODIGO, a => a.CODIGOACTIVIDADGENERAL,
                    (ag, a) => new { ACTIVIDAD_GENERAL = ag, ACTIVIDAD = a })
                    .Select(x => new
                    {
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
                    .Select(x => new
                    {
                        ROL = x.ROL.NOMBRE,
                        CODIGO = x.USUARIO.CODIGO,
                        NOMBRECOMPLETO = x.USUARIO.NOMBRECOMPLETO
                    })
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
        public void getGroupActivityByActivity(int activityId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                var data = db.BE_GRUPO_ACTIVIDAD.Join(db.BE_GRUPO, ga => ga.CODIGOGRUPO, g => g.CODIGO,
                    (ga, g) => new { ga, g }).Join(db.CARRERA_MODAL, gcm => gcm.g.CODIGOMODALIDAD, cm => cm.CRRMODCODIGOI,
                    (gcm, cm) => new { gcm, cm }).Select(s => new
                    {
                        CODIGOACTIVIDAD = s.gcm.ga.CODIGOACTIVIDAD,
                        CODIGOGRUPO = s.gcm.ga.CODIGOGRUPO,
                        CODIGOCARRERA = s.cm.CRRCODIGOI,
                        CODIGOMODALIDAD = s.cm.MDLCODIGOI,
                        CODIGONIVEL = s.gcm.g.CODIGONIVEL
                    }).Where(w => w.CODIGOACTIVIDAD == activityId).ToList();

                if (data != null && data.Count > 0)
                    response = new Response(true, "", "", "", data);
                else
                    response = new Response(false, "info", "Informaci\u00F3n", "Todav\u00EDa no existen grupos registrados en la actividad", data);
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al cargar los grupos asignados a una actividad", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        private int[] getCareerModalityIds(int[] modalities, int[] carees)
        {
            bienestarEntities db = new bienestarEntities();
            List<int> data = null;
            if (modalities != null && modalities.Length > 0 && carees != null && carees.Length > 0)
                data = db.CARRERA_MODAL.Where(cm => modalities.Contains(cm.MDLCODIGOI) && carees.Contains(cm.CRRCODIGOI)).Select(m => m.CRRMODCODIGOI).ToList();
            else if (carees != null && carees.Length > 0)
                data = db.CARRERA_MODAL.Where(cm => carees.Contains(cm.CRRCODIGOI)).Select(m => m.CRRMODCODIGOI).ToList();
            else
                data = db.CARRERA_MODAL.Where(cm => modalities.Contains(cm.MDLCODIGOI)).Select(m => m.CRRMODCODIGOI).ToList();

            int[] carMod = new int[data.Count];
            int counter = 0;
            foreach (int s in data)
                carMod[counter++] = int.Parse(s.ToString());

            return carMod;
        }

        [WebMethod]
        public void saveAllGroups()
        {
            bienestarEntities db = new bienestarEntities();

            List<CARRERA> careerList = db.CARRERAs.ToList();
            List<MODALIDAD> modalityList = db.MODALIDADs.ToList();

            int[] careerIds = new int[careerList.Count];
            int[] modalityIds = new int[modalityList.Count];

            int index = 0;
            foreach (CARRERA career in careerList)
                careerIds[index++] = career.CRRCODIGOI;

            index = 0;
            foreach (MODALIDAD modality in modalityList)
                modalityIds[index++] = modality.MDLCODIGOI;

            int[] careerModalityIds = getCareerModalityIds(modalityIds, careerIds);

            for (int i = 0; i < careerModalityIds.Length; i++)
            {
                int[] levelIds = getLevelByCareerModality(careerModalityIds[i]);
                
                for (int j = 0; j < levelIds.Length; j++)
                {
                    int careerId = careerModalityIds[i];
                    int modalId = levelIds[j];
                    List<BE_GRUPO> groupExist = db.BE_GRUPO.Where(g => g.CODIGOMODALIDAD == careerId && g.CODIGONIVEL == modalId).ToList();

                    if (groupExist.Count == 0)
                    {
                        BE_GRUPO newGroup = new BE_GRUPO();
                        newGroup.CODIGONIVEL = levelIds[j];
                        newGroup.CODIGOMODALIDAD = careerModalityIds[i];
                        db.BE_GRUPO.AddObject(newGroup);

                        db.SaveChanges();
                    }
                }
            }
        }

        private int[] getLevelByCareerModality(int carMod)
        {
            bienestarEntities db = new bienestarEntities();
            int period = getPresentPeriod();
            var data = db.MATRICULAs.Where(m => m.PRDCODIGOI == period && m.CRRMODCODIGOI == carMod)
                .Select(m => m.NVLCODIGOI).Distinct().ToList();

            int[] mod = new int[data.Count];
            int counter = 0;
            foreach (var s in data)
                mod[counter++] = int.Parse(s.ToString());

            return mod;
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
        public void getAllSchools(int[] faculties)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                if (faculties != null && faculties.Length > 0)
                    response = new Response(true, "", "", "", db.ESCUELAs.Where(e => faculties.Contains(e.FCLCODIGOI)).ToList());
                else
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
        public void getAllCareers(int[] schools)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                if (schools != null && schools.Length > 0)
                    response = new Response(true, "", "", "", db.CARRERAs.Where(c => schools.Contains(c.ESCCODIGOI)).ToList());
                else
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
        public void getAllLevels(int[] modalities, int[] carees)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                int[] careerModalityIds;
                int[] matriculaIds;

                if ((modalities != null && modalities.Length > 0) || (carees != null && carees.Length > 0))
                {
                    careerModalityIds = getCareerModalityIds(modalities, carees);
                    matriculaIds = getMatricula(careerModalityIds);

                    response = new Response(true, "", "", "", db.NIVELs.Where(l => matriculaIds.Contains(l.NVLCODIGOI)).ToList());
                }
                else
                    response = new Response(true, "", "", "", db.NIVELs.Where(n => n.NVLVISTAWEB == true).ToList());
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener los niveles", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        private int getPresentPeriod()
        {
            bienestarEntities db = new bienestarEntities();
            PERIODO period = db.PERIODOes.Single(p => p.TPECODIGOI == 1 && p.PRDHABILMAT == "1");
            return period.PRDCODIGOI;
        }

        private int[] getMatricula(int[] carMod)
        {
            bienestarEntities db = new bienestarEntities();
            int period = getPresentPeriod();
            var data = db.MATRICULAs.Where(m => m.PRDCODIGOI == period && carMod.Contains(m.CRRMODCODIGOI))
                .Select(m => m.NVLCODIGOI).ToList();

            int[] mod = new int[data.Count];
            int counter = 0;
            foreach (var s in data)
                mod[counter++] = int.Parse(s.ToString());

            return mod;
        }

        private int[] getObjectDistinct(HashSet<int> objectSetDistinct)
        {
            int[] objectDistinct = new int[objectSetDistinct.Count];
            int band = 0;

            foreach (int o in objectSetDistinct)
            {
                objectDistinct[band] = o;
                band++;
            }

            return objectDistinct;
        }

        [WebMethod]
        public void saveGroupActivity(int[] careersV, int[] modalitiesV, int[] levelsV, int activityId, int[] originCareersV, int[] originModalitiesV, int[] originLevelsV)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                HashSet<int> careersDistinct = new HashSet<int>(careersV);
                HashSet<int> modalitiesDistinct = new HashSet<int>(modalitiesV);
                HashSet<int> levelsDistinct = new HashSet<int>(levelsV);
                HashSet<int> originCareersDistinct = new HashSet<int>(originCareersV);
                HashSet<int> originModalitiesDistinct = new HashSet<int>(originModalitiesV);
                HashSet<int> originLevelsDistinct = new HashSet<int>(originLevelsV);

                int[] careers = getObjectDistinct(careersDistinct);
                int[] modalities = getObjectDistinct(modalitiesDistinct);
                int[] levels = getObjectDistinct(levelsDistinct);
                int[] originCareers = getObjectDistinct(originCareersDistinct);
                int[] originModalities = getObjectDistinct(originModalitiesDistinct);
                int[] originLevels = getObjectDistinct(originLevelsDistinct);

                if (originCareers.Length > 0 || originModalities.Length > 0 || originLevels.Length > 0)
                    deleteCareerDeselected(careers, modalities, levels, activityId, originCareers, originModalities, originLevels);

                int[] careerModalityIds = getCareerModalityIds(modalities, careers);

                for (int i = 0; i < careerModalityIds.Length; i++)
                {
                    for (int j = 0; j < levels.Length; j++)
                        saveGroupActivity(careerModalityIds[i], levels[j], activityId);                        
                }

                response = new Response(true, "info", "Agregar", "Grupos agregado correctamente", null);
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al actualizar los grupos", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        private void saveGroupActivity(int careerModalityId, int levelId, int activityId)
        {
            bienestarEntities db = new bienestarEntities();

            try
            {
                BE_GRUPO group = db.BE_GRUPO.Single(g => g.CODIGOMODALIDAD == careerModalityId && g.CODIGONIVEL == levelId);

                List<BE_GRUPO_ACTIVIDAD> groupActivity = db.BE_GRUPO_ACTIVIDAD.Where(ga => ga.CODIGOGRUPO == group.CODIGO && ga.CODIGOACTIVIDAD == activityId).ToList();

                if (groupActivity == null || groupActivity.Count == 0)
                {
                    BE_GRUPO_ACTIVIDAD newGroupActivity = new BE_GRUPO_ACTIVIDAD();
                    newGroupActivity.CODIGOGRUPO = group.CODIGO;
                    newGroupActivity.CODIGOACTIVIDAD = activityId;
                    newGroupActivity.ESTADO = true;

                    db.BE_GRUPO_ACTIVIDAD.AddObject(newGroupActivity);

                    db.SaveChanges();
                }

                saveAssistance(group.CODIGO, activityId, careerModalityId, levelId);
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("No se encontraron datos");
            }
        }

        private void saveAssistance(int groupId, int activityId, int careerModalityId, int levelId)
        {
            bienestarEntities db = new bienestarEntities();
            int period = getPresentPeriod();
            var data = db.MATRICULAs.Join(db.INSCRIPCIONs, m => m.INSCODIGOI, i => i.INSCODIGOI, (m, i) => new { m, i })
                .Join(db.DATOSPERSONALES, mi => mi.i.DTPCEDULAC, d => d.DTPCEDULAC, (mi, d) => new { mi, d })
                .Select(s => new {
                    ALUMNO = s.mi.m.MTRNUMEROI,
                    PERIODO = s.mi.m.PRDCODIGOI,
                    CODIGOCARRERAMODULO = s.mi.m.CRRMODCODIGOI,
                    NIVEL = s.mi.m.NVLCODIGOI,
                    CEDULA = s.d.DTPCEDULAC,
                    NOMBRE = s.d.DTPNOMBREC + s.d.DTPAPELLIC + s.d.DTPAPELLIC2
                })
                .Where(w => w.PERIODO == period && w.CODIGOCARRERAMODULO == careerModalityId && w.NIVEL == levelId).ToList();

            for (int i = 0; i < data.Count; i++)
            {
                long studentId = data[i].ALUMNO;
                try
                {
                    BE_ASISTENCIA assistance = db.BE_ASISTENCIA.Single(a => a.CODIGOGRUPO == groupId && a.CODIGOACTIVIDAD == activityId && a.CODIGOALUMNO == studentId);
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("No se encontraron datos");
                }

                BE_ASISTENCIA newAssitance = new BE_ASISTENCIA();
                newAssitance.CODIGOACTIVIDAD = activityId;
                newAssitance.CODIGOGRUPO = groupId;
                newAssitance.CODIGOALUMNO = studentId;
                newAssitance.ASISTENCIA = false;

                db.BE_ASISTENCIA.AddObject(newAssitance);

                db.SaveChanges();
            }
        }

        [WebMethod]
        private void deleteCareerDeselected(int[] careers, int[] modalities, int[] levels, int activityId, int[] originCareers, int[] originModalities, int[] originLevels)
        {
            int[] careerDeleted = getObjectsToDelete(originCareers, careers);
            int[] modalityDeleted = getObjectsToDelete(originModalities, modalities);
            int[] levelDeleted = getObjectsToDelete(originLevels, levels);

            int[] careerModalityIds = getCareerModalityIds(modalityDeleted, careerDeleted);

            deleteDependingData(careerModalityIds, levelDeleted, activityId);
        }

        [WebMethod]
        private void deleteDependingData(int[] careerModalityIds, int[] levelDeleted, int activityId)
        {
            bienestarEntities db = new bienestarEntities();

            if (careerModalityIds.Length > 0 && levelDeleted.Length > 0)
            {
                for (int i = 0; i < careerModalityIds.Length; i++)
                {
                    for (int j = 0; j < levelDeleted.Length; j++)
                    {
                        int careerId = careerModalityIds[i];
                        int levelId = levelDeleted[j];
                        BE_GRUPO group = db.BE_GRUPO.Single(g => g.CODIGOMODALIDAD == careerId && g.CODIGONIVEL == levelId);

                        List<BE_GRUPO_ACTIVIDAD> groupActivity = db.BE_GRUPO_ACTIVIDAD.Where(ga => ga.CODIGOGRUPO == group.CODIGO && ga.CODIGOACTIVIDAD == activityId).ToList();

                        if (groupActivity != null && groupActivity.Count > 0)
                        {
                            foreach (BE_GRUPO_ACTIVIDAD groupActivityDeleted in groupActivity)
                            {
                                db.BE_GRUPO_ACTIVIDAD.DeleteObject(groupActivityDeleted);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            else if (careerModalityIds.Length > 0)
            {
                for (int i = 0; i < careerModalityIds.Length; i++)
                {
                    int careerId = careerModalityIds[i];
                    List<int> group = db.BE_GRUPO.Where(g => g.CODIGOMODALIDAD == careerId).Select(s => s.CODIGO).ToList();

                    List<BE_GRUPO_ACTIVIDAD> groupActivity = db.BE_GRUPO_ACTIVIDAD.Where(ga => group.Contains(ga.CODIGOGRUPO) && ga.CODIGOACTIVIDAD == activityId).ToList();
                    List<BE_ASISTENCIA> assistance = db.BE_ASISTENCIA.Where(a => group.Contains(a.CODIGOGRUPO) && a.CODIGOACTIVIDAD == activityId).ToList();

                    if (groupActivity != null && groupActivity.Count > 0)
                    {
                        foreach (BE_GRUPO_ACTIVIDAD groupActivityDeleted in groupActivity)
                        {
                            db.BE_GRUPO_ACTIVIDAD.DeleteObject(groupActivityDeleted);
                            db.SaveChanges();
                        }
                    }

                    if (assistance != null && assistance.Count > 0)
                    {
                        foreach (BE_ASISTENCIA assistanceDeleted in assistance)
                        {
                            db.BE_ASISTENCIA.DeleteObject(assistanceDeleted);
                            db.SaveChanges();
                        }
                    }
                }
            }
            else if (levelDeleted.Length > 0)
            {
                for (int j = 0; j < levelDeleted.Length; j++)
                {
                    int levelId = levelDeleted[j];
                    List<int> group = db.BE_GRUPO.Where(g => g.CODIGONIVEL == levelId).Select(s => s.CODIGO).ToList();

                    List<BE_GRUPO_ACTIVIDAD> groupActivity = db.BE_GRUPO_ACTIVIDAD.Where(ga => group.Contains(ga.CODIGOGRUPO) && ga.CODIGOACTIVIDAD == activityId).ToList();

                    if (groupActivity != null && groupActivity.Count > 0)
                    {
                        foreach (BE_GRUPO_ACTIVIDAD groupActivityDeleted in groupActivity)
                        {
                            db.BE_GRUPO_ACTIVIDAD.DeleteObject(groupActivityDeleted);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        private int[] getObjectsToDelete(int[] originArray, int[] lastArray)
        {
            Boolean exist;            
            int band = 0;

            for (int i = 0; i < originArray.Length; i++)
            {
                exist = false;
                for (int j = 0; j < lastArray.Length; j++)
                    if (originArray[i] == lastArray[j])
                        exist = true;

                if (!exist)
                    band++;
            }

            int[] arrayDeleted = new int[band];
            if (band > 0)
            {
                band = 0;
                for (int i = 0; i < originArray.Length; i++)
                {
                    exist = false;
                    for (int j = 0; j < lastArray.Length; j++)
                        if (originArray[i] == lastArray[j])
                            exist = true;

                    if (!exist)
                    {
                        arrayDeleted[band] = originArray[i];
                        band++;
                    }
                }
            }

            return arrayDeleted;
        }

        [WebMethod]
        public void getStudentsAssistance(int activityId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                var data = db.BE_ASISTENCIA.Join(db.MATRICULAs, a => a.CODIGOALUMNO, m => m.MTRNUMEROI, (a, m) => new { a, m })
                    .Join(db.INSCRIPCIONs, am => am.m.INSCODIGOI, i => i.INSCODIGOI, (am, i) => new { am, i })
                    .Join(db.DATOSPERSONALES, ami => ami.i.DTPCEDULAC, d => d.DTPCEDULAC, (ami, d) => new { ami, d })
                    .Select(s => new {
                        CODIGO = s.ami.am.a.CODIGO,
                        ACTIVIDAD = s.ami.am.a.CODIGOACTIVIDAD,
                        CEDULA = s.d.DTPCEDULAC,
                        NOMBRE = s.d.DTPNOMBREC + s.d.DTPAPELLIC + s.d.DTPAPELLIC2,
                        ASISTENCIA = s.ami.am.a.ASISTENCIA
                    })
                    .Where(w => w.ACTIVIDAD == activityId).ToList();

                if (data != null && data.Count > 0)
                    response = new Response(true, "", "", "", data);
                else
                    response = new Response(false, "info", "Informaci\u00F3n", "No se han encontrado datos de asistencia", null);
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener los datos de asistencia", null);
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
        public void addUploadedFileDataBase()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();
            int codigoActividad = Int32.Parse(System.Web.HttpContext.Current.Request.Params.Get("codigoActividad"));
            string observacion = System.Web.HttpContext.Current.Request.Params.Get("observacion");

            try
            {
                HttpFileCollection hfc = HttpContext.Current.Request.Files;

                if (hfc.Count > 0 && codigoActividad > 0)
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
                            activityAttached.CODIGOACTIVIDAD = codigoActividad;
                            activityAttached.ADJUNTO = fileBytes;
                            activityAttached.NOMBRE = hfc[0].FileName;
                            activityAttached.CONTENTTYPE = hfc[0].ContentType;
                            activityAttached.DESCRIPCION = observacion.ToUpper();

                            db.BE_ACTIVIDAD_ADJUNTO.AddObject(activityAttached);
                            db.SaveChanges();
                        }
                    }

                    response = new Response(true, "info", "Informaci\u00F3n", "El archivo se adjunt\u00F3 correctamente", activityAttached);
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
        public void getAttachByActivity(int activityId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                List<BE_ACTIVIDAD_ADJUNTO> attaches = db.BE_ACTIVIDAD_ADJUNTO.Where(aa => aa.CODIGOACTIVIDAD == activityId).ToList();

                if (attaches != null && attaches.Count > 0)
                    response = new Response(true, "", "", "", attaches);
                else
                    response = new Response(false, "info", "Informaci\u00F3n", "No se han encontrado adjuntos registrados", null);
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener los datos adjuntos", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void getAttach(int code)
        {
            bienestarEntities db = new bienestarEntities();

            BE_ACTIVIDAD_ADJUNTO attach = db.BE_ACTIVIDAD_ADJUNTO.Where(aa => aa.CODIGO == code).First();

            byte[] response = null;

            if (attach != null && attach.ADJUNTO != null)
                response = attach.ADJUNTO;

            Context.Response.ContentType = attach.CONTENTTYPE;
            Context.Response.AddHeader("content-disposition", "attachment; filename=" + attach.NOMBRE);
            Context.Response.BinaryWrite(response);
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void removeAttach(int attachCode)
        {
            bienestarEntities db = new bienestarEntities();

            BE_ACTIVIDAD_ADJUNTO attachDeleted = db.BE_ACTIVIDAD_ADJUNTO.Where(a => a.CODIGO == attachCode).FirstOrDefault();

            if (attachDeleted != null)
            {
                db.BE_ACTIVIDAD_ADJUNTO.DeleteObject(attachDeleted);
                db.SaveChanges();
            }
            writeResponse("ok");
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

        [WebMethod]
        public void removeActivityById(int activityId)
        {
            Response response = new Response(true, "", "", "", null);

            try
            {
                bienestarEntities db = new bienestarEntities();

                BE_ACTIVIDAD activityDeleted = db.BE_ACTIVIDAD.Single(a => a.CODIGO == activityId);

                deleteAttachedActivity(activityId);

                deleteAssistanceActivity(activityId);

                deleteGroupActivity(activityId);

                db.BE_ACTIVIDAD.DeleteObject(activityDeleted);
                db.SaveChanges();

                response = new Response(true, "info", "Eliminar", "Actividad eliminada correctamente", null);
            }
            catch (InvalidOperationException)
            {
                // Error al eliminar el rol
                response = new Response(false, "error", "Error", "Error al obtener datos para eliminar", null);
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
    }
}
