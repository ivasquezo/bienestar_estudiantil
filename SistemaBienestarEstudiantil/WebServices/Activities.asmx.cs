﻿using System;
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
using System.Text;
using System.Web.Script.Services;
using Microsoft.Office.Interop.Excel;
using System.Net.Mail;

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

        [WebMethod(EnableSession = true)]
        public void getAllGeneralActivitiesWithActivity()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            int userCode = (int)Utils.getSession("userCode");
            bool isTeacher = Utils.isTeacher(userCode);

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
                        CODIGOUSUARIO = x.ACTIVIDAD.CODIGOUSUARIO,
                        ISTEACHER = isTeacher
                    }).OrderBy(o => o.CODIGO).ToList();

                if (data != null && data.Count > 0)
                    response = new Response(true, "", "", "", isTeacher ? data.Where(w => w.CODIGOUSUARIO == userCode) : data);
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
                var data = db.BE_USUARIO.Join(db.BE_ROL, u => u.CODIGOROL, r => r.CODIGO, (u, r) => new { u, r }).
                    Join(db.BE_ROL_ACCESO, rra => rra.r.CODIGO, ra => ra.CODIGOROL, (rra, ra) => new { rra, ra }).
                    Join(db.BE_ACCESO, raa => raa.ra.CODIGOACCESO, a => a.CODIGO, (raa, a) => new { raa, a }).Select(x => new
                    {
                        NOMBREACCESO = x.a.NOMBRE,
                        VALIDO = x.raa.ra.VALIDO,
                        CODIGO = x.raa.rra.u.CODIGO,
                        NOMBRECOMPLETO = x.raa.rra.u.NOMBRECOMPLETO
                    })
                    .Where(y => y.NOMBREACCESO == Utils.MODULOTAREAS && y.VALIDO == true).ToList();

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
        public void countGeneralActivityWithName(String activityName)
        {
            bienestarEntities db = new bienestarEntities();
            // Obtiene la cantidad de actividades generales con un nombre determinado
            int cantidad = db.BE_ACTIVIDAD_GENERAL.Where(a => activityName != null && a.NOMBRE == activityName).Count();
            writeResponse("{\"cantidad\":" + cantidad + "}");
        }

        [WebMethod]
        public void saveActivityData(int activityId, String activityName, DateTime activityDate, int activityStatus,
            String activityObservation, int generalActivityId, String generalActivityName, int userId, Boolean sendMail)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();
            try
            {
                // Si se agrega una nueva actividad general
                if (generalActivityId == 0)
                {
                    // Agrega una nueva actividad general
                    BE_ACTIVIDAD_GENERAL newGeneralActivity = new BE_ACTIVIDAD_GENERAL();
                    newGeneralActivity.NOMBRE = generalActivityName;
                    db.BE_ACTIVIDAD_GENERAL.AddObject(newGeneralActivity);
                    db.SaveChanges();
                    // Obtiene los datos de la actividad general agregada
                    BE_ACTIVIDAD_GENERAL generalActivity = db.BE_ACTIVIDAD_GENERAL.Where(w => w.NOMBRE == generalActivityName).FirstOrDefault();
                    generalActivityId = generalActivity.CODIGO;
                }
                // Agregar los datos de la actividad
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
                // Si se desea enviar un correo electronico al responsable asignado
                if (sendMail)
                {
                    // Obtiene los datos del usuario seleccionado para enviar el correo electronico
                    BE_USUARIO responsable = db.BE_USUARIO.Single(u => u.CODIGO == userId);
                    string to = responsable.CORREO;
                    string subject = "Creaci\u00F3n de actividad";
                    string body = "Estimado/a " + responsable.NOMBRECOMPLETO + ": \nHa sido creada la actividad " + activityName.ToUpper() + " bajo su responsabilidad, para la fecha " + activityDate.ToString("dd/MM/yyyy") + ". \nPor favor ingrese con su usuario ''" + responsable.NOMBREUSUARIO + "'' para revisar los detalles";
                    Class.Utils.sendMail(to, subject, body);
                }
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
        public void addNewActivity(String activityName, DateTime activityDate, int activityStatus, String activityObservation, int generalActivityId,
            String generalActivityName, int userId, Boolean sendMail)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();
            try
            {
                // Si se agrega una nueva actividad general
                if (generalActivityId == 0)
                {
                    // Agrega una nueva actividad general
                    BE_ACTIVIDAD_GENERAL newGeneralActivity = new BE_ACTIVIDAD_GENERAL();
                    newGeneralActivity.NOMBRE = generalActivityName;
                    db.BE_ACTIVIDAD_GENERAL.AddObject(newGeneralActivity);
                    db.SaveChanges();
                    // Obtiene los datos de la actividad general agregada
                    BE_ACTIVIDAD_GENERAL generalActivity = db.BE_ACTIVIDAD_GENERAL.Where(w => w.NOMBRE == generalActivityName).FirstOrDefault();
                    generalActivityId = generalActivity.CODIGO;
                }
                // Agregar los datos de la actividad
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
                // Si se desea enviar un correo electronico al responsable asignado
                if (sendMail)
                {
                    // Obtiene los datos del usuario seleccionado para enviar el correo electronico
                    BE_USUARIO responsable = db.BE_USUARIO.Single(u => u.CODIGO == userId);
                    string to = responsable.CORREO;
                    string subject = "Creaci\u00F3n de actividad";
                    string body = "Estimado/a " + responsable.NOMBRECOMPLETO + ": \nHa sido creada la actividad " + activityName.ToUpper() + " bajo su responsabilidad, para la fecha " + activityDate.ToString("dd/MM/yyyy") + ". \nPor favor ingrese con su usuario ''" + responsable.NOMBREUSUARIO + "'' para revisar los detalles";
                    Class.Utils.sendMail(to, subject, body);
                }
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
                    (gcm, cm) => new { gcm, cm }).Join(db.MATRICULAs, cmm => cmm.cm.CRRMODCODIGOI, m => m.CRRMODCODIGOI,
                    (cmm, m) => new { cmm, m }).Select(s => new
                    {
                        CODIGOACTIVIDAD = s.cmm.gcm.ga.CODIGOACTIVIDAD,
                        CODIGOGRUPO = s.cmm.gcm.ga.CODIGOGRUPO,
                        CODIGOCARRERA = s.cmm.cm.CRRCODIGOI,
                        CODIGOMODALIDAD = s.cmm.cm.MDLCODIGOI,
                        CODIGONIVEL = s.cmm.gcm.g.CODIGONIVEL,
                        PARALELO = s.m.MTRPARALELO
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

        [WebMethod(EnableSession = true)]
        public void getActivitiesReport()
        {
            Response response = null;
            bienestarEntities db = new bienestarEntities();

            try
            {
                int userCode = (int)Utils.getSession("userCode");
                bool isTeacher = Utils.isTeacher(userCode);
                var actividades = db.BE_ACTIVIDAD.Join(db.BE_ACTIVIDAD_GENERAL, ac => ac.CODIGOACTIVIDADGENERAL, ag => ag.CODIGO, (ac, ag) => new { ac, ag }).
                Select(s => new
                {
                    CODIGOUSUARIO = s.ac.CODIGOUSUARIO,
                    CODIGO = s.ac.CODIGO,
                    ACTIVIDAD = s.ac.NOMBRE,
                    ACTIVIDADGENERAL = s.ag.NOMBRE,
                    FECHA = s.ac.FECHA,
                    ESTADO = s.ac.ESTADO,
                    DATOS = new Datos()
                }).OrderBy(o => o.FECHA).ToList();

                foreach (var actividad in actividades)
                {
                    Datos datos = getNivelCarreraModalidad(actividad.CODIGO);
                    actividad.DATOS.ADJUNTOS = datos.ADJUNTOS;
                    actividad.DATOS.NIVELES = datos.NIVELES;
                    actividad.DATOS.MODALIDADES = datos.MODALIDADES;
                    actividad.DATOS.CARRERAS = datos.CARRERAS;
                    actividad.DATOS.ASISTENCIA = getActitityAssistance(actividad.CODIGO);
                }

                response = new Response(true, "", "", "", isTeacher ? actividades.Where(w => w.CODIGOUSUARIO == userCode) : actividades);
            }
            catch (Exception ex)
            {
                response = new Response(false, "error", "Error", "Error al cargar los grupos asignados a una actividad", ex.ToString());
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        private Datos getNivelCarreraModalidad(int codigoActividad)
        {
            bienestarEntities db = new bienestarEntities();
            var listActivitiesCodes = db.BE_GRUPO_ACTIVIDAD.Join(db.BE_GRUPO, ga => ga.CODIGOGRUPO, g => g.CODIGO, (ga, g) => new { ga, g }).
                            Join(db.CARRERA_MODAL, gcm => gcm.g.CODIGOMODALIDAD, cm => cm.CRRMODCODIGOI, (gcm, cm) => new { gcm, cm }).
                            Select(s => new
                            {
                                CODIGOACTIVIDAD = s.gcm.ga.CODIGOACTIVIDAD,
                                CODIGOGRUPO = s.gcm.ga.CODIGOGRUPO,
                                CODIGOCARRERA = s.cm.CRRCODIGOI,
                                CODIGOMODALIDAD = s.cm.MDLCODIGOI,
                                CODIGONIVEL = s.gcm.g.CODIGONIVEL
                            }).Where(w => w.CODIGOACTIVIDAD == codigoActividad).ToList();

            List<int> codigosCarreras = listActivitiesCodes.Select(a => a.CODIGOCARRERA).Distinct().ToList();
            List<string> carreras = db.CARRERAs.Where(w => codigosCarreras.Contains(w.CRRCODIGOI)).Select(c => c.CRRDESCRIPC.Trim()).ToList();

            List<int> codigosNiveles = listActivitiesCodes.Select(a => a.CODIGONIVEL).Distinct().ToList();
            List<string> niveles = db.NIVELs.Where(w => codigosNiveles.Contains(w.NVLCODIGOI)).Select(c => c.NVLDESCRIPC.Trim()).ToList();

            List<int> codigosModalidades = listActivitiesCodes.Select(a => a.CODIGOMODALIDAD).Distinct().ToList();
            List<string> modalidades = db.MODALIDADs.Where(w => codigosModalidades.Contains(w.MDLCODIGOI)).Select(c => c.MDLDESCRIPC.Trim()).ToList();

            List<string> adjuntos = db.BE_ACTIVIDAD_ADJUNTO.Where(w => w.CODIGOACTIVIDAD == codigoActividad).Select(c => c.DESCRIPCION.Trim()).ToList();

            return new Datos(niveles, carreras, modalidades, adjuntos);
        }

        private int getActitityAssistance(int activityId)
        {
            bienestarEntities db = new bienestarEntities();

            int count = db.BE_ASISTENCIA.Join(db.MATRICULAs, a => a.CODIGOALUMNO, m => m.MTRNUMEROI, (a, m) => new { a, m })
                .Join(db.INSCRIPCIONs, am => am.m.INSCODIGOI, i => i.INSCODIGOI, (am, i) => new { am, i })
                .Join(db.DATOSPERSONALES, ami => ami.i.DTPCEDULAC, d => d.DTPCEDULAC, (ami, d) => new { ami, d })
                .Where(w => w.ami.am.a.CODIGOACTIVIDAD == activityId && w.ami.am.a.ASISTENCIA == true).Count();

            return count;
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
            var data = db.MATRICULAs.Join(db.NIVELs, mn => mn.NVLCODIGOI, n => n.NVLCODIGOI, (mn, n) => new { mn, n })
                .Where(m => m.mn.PRDCODIGOI == period && m.mn.CRRMODCODIGOI == carMod)
                .Select(m => m.n.NVLCODIGOI).Distinct().ToList();

            int[] mod = new int[data.Count];
            int counter = 0;
            foreach (var s in data)
                mod[counter++] = int.Parse(s.ToString());

            return mod;
        }

        [WebMethod]
        public void getAllCareers(int[] modalities)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                if (modalities != null && modalities.Length > 0)
                {
                    var careers = db.CARRERAs.Join(db.CARRERA_MODAL, ccm => ccm.CRRCODIGOI, c => c.CRRCODIGOI,
                        (ccm, c) => new { ccm, c }).Select(s => new
                        {
                            CODIGOMODALIDAD = s.c.MDLCODIGOI,
                            CRRCODIGOI = s.ccm.CRRCODIGOI,
                            CRRDESCRIPC = s.ccm.CRRDESCRIPC
                        }).Where(c => modalities.Contains(c.CODIGOMODALIDAD)).ToList();
                    response = new Response(true, "", "", "", careers);
                }
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
            var data = db.MATRICULAs.Join(db.NIVELs, mn => mn.NVLCODIGOI, n => n.NVLCODIGOI, (mn, n) => new { mn, n })
                .Where(m => m.mn.PRDCODIGOI == period && carMod.Contains(m.mn.CRRMODCODIGOI))
                .Select(m => m.n.NVLCODIGOI).ToList();

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
        public void getAllGroups(int[] modalities, int[] carees, int[] levels)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                int[] careerModalityIds;
                int period = getPresentPeriod();

                if ((modalities != null && modalities.Length > 0) || (carees != null && carees.Length > 0))
                {
                    careerModalityIds = getCareerModalityIds(modalities, carees);
                    var data = db.MATRICULAs.Join(db.NIVELs, mn => mn.NVLCODIGOI, n => n.NVLCODIGOI, (mn, n) => new { mn, n })
                        .Select(s => new
                        {
                            NIVEL = s.n.NVLCODIGOI,
                            MODALIDAD = s.mn.CRRMODCODIGOI,
                            PERIODO = s.mn.PRDCODIGOI,
                            PARALELO = s.mn.MTRPARALELO
                        })
                        .Where(w => w.PERIODO == period && careerModalityIds.Contains(w.MODALIDAD) && levels.Contains(w.NIVEL)).Distinct().ToList();

                    response = new Response(true, "", "", "", data);
                }
                else
                {
                    var data = db.MATRICULAs.Join(db.NIVELs, mn => mn.NVLCODIGOI, n => n.NVLCODIGOI, (mn, n) => new { mn, n })
                        .Select(s => new
                        {
                            NIVEL = s.n.NVLCODIGOI,
                            MODALIDAD = s.mn.CRRMODCODIGOI,
                            PERIODO = s.mn.PRDCODIGOI,
                            PARALELO = s.mn.MTRPARALELO
                        })
                        .Where(w => w.PERIODO == period).Distinct().ToList();

                    response = new Response(true, "", "", "", data);
                }
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al obtener los paralelos", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }

        [WebMethod]
        public void saveGroupActivity(string[] group, int[] careersV, int[] modalitiesV, int[] levelsV, int activityId, int[] originCareersV, int[] originModalitiesV, int[] originLevelsV)
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
                    deleteCareerDeselected(activityId);

                int[] careerModalityIds = getCareerModalityIds(modalities, careers);

                for (int i = 0; i < careerModalityIds.Length; i++)
                {
                    for (int j = 0; j < levels.Length; j++)
                        saveGroupActivity(careerModalityIds[i], levels[j], group, activityId);
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

        private void saveGroupActivity(int careerModalityId, int levelId, string[] groupP, int activityId)
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

                    saveAssistance(group.CODIGO, activityId, careerModalityId, levelId, groupP);
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("No se encontraron datos");
            }
        }

        private void saveAssistance(int groupId, int activityId, int careerModalityId, int levelId, string[] group)
        {
            bienestarEntities db = new bienestarEntities();
            int period = getPresentPeriod();
            var data = db.MATRICULAs.Join(db.INSCRIPCIONs, m => m.INSCODIGOI, i => i.INSCODIGOI, (m, i) => new { m, i })
                .Join(db.DATOSPERSONALES, mi => mi.i.DTPCEDULAC, d => d.DTPCEDULAC, (mi, d) => new { mi, d })
                .Select(s => new
                {
                    ALUMNO = s.mi.m.MTRNUMEROI,
                    PERIODO = s.mi.m.PRDCODIGOI,
                    CODIGOCARRERAMODULO = s.mi.m.CRRMODCODIGOI,
                    NIVEL = s.mi.m.NVLCODIGOI,
                    CEDULA = s.d.DTPCEDULAC,
                    NOMBRE = s.d.DTPNOMBREC + s.d.DTPAPELLIC + s.d.DTPAPELLIC2,
                    PARALELO = s.mi.m.MTRPARALELO
                })
                .Where(w => w.PERIODO == period && w.CODIGOCARRERAMODULO == careerModalityId && w.NIVEL == levelId && group.Contains(w.PARALELO)).ToList();

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

        private void deleteCareerDeselected(int activityId)
        {
            bienestarEntities db = new bienestarEntities();

            try
            {
                List<BE_GRUPO_ACTIVIDAD> groupActivity = db.BE_GRUPO_ACTIVIDAD.Where(ga => ga.CODIGOACTIVIDAD == activityId).ToList();
                List<BE_ASISTENCIA> assistance = db.BE_ASISTENCIA.Where(a => a.CODIGOACTIVIDAD == activityId).ToList();

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
            catch (InvalidOperationException)
            {
                Console.WriteLine("No se encontraron datos");
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
                    .Join(db.NIVELs, amin => amin.ami.am.m.NVLCODIGOI, n => n.NVLCODIGOI, (amin, n) => new { amin, n })
                    .Join(db.CARRERA_MODAL, amincm => amincm.amin.ami.am.m.CRRMODCODIGOI, cm => cm.CRRMODCODIGOI, (amincm, cm) => new { amincm, cm })
                    .Join(db.CARRERAs, amincmc => amincmc.cm.CRRCODIGOI, c => c.CRRCODIGOI, (amincmc, c) => new { amincmc, c })
                    .Select(s => new
                    {
                        CODIGO = s.amincmc.amincm.amin.ami.am.a.CODIGO,
                        ACTIVIDAD = s.amincmc.amincm.amin.ami.am.a.CODIGOACTIVIDAD,
                        NOTIFICACIONENVIDA = s.amincmc.amincm.amin.ami.am.a.NOTIFICACIONENVIADA,
                        CEDULA = s.amincmc.amincm.amin.d.DTPCEDULAC,
                        NOMBRE = s.amincmc.amincm.amin.d.DTPNOMBREC + s.amincmc.amincm.amin.d.DTPAPELLIC + s.amincmc.amincm.amin.d.DTPAPELLIC2,
                        ASISTENCIA = s.amincmc.amincm.amin.ami.am.a.ASISTENCIA,
                        CORREO = s.amincmc.amincm.amin.d.DTPEMAILC,
                        NIVEL = s.amincmc.amincm.n.NVLDESCRIPC,
                        CARRERA = s.c.CRRDESCRIPC,
                        PARALELO = s.amincmc.amincm.amin.ami.am.m.MTRPARALELO
                    }).Where(w => w.ACTIVIDAD == activityId).OrderBy(o => o.CARRERA).ToList();

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
        public void sendStudentsNotification(Estudiante[] studentsMails, int activityId)
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                BE_ACTIVIDAD activity = db.BE_ACTIVIDAD.Single(a => a.CODIGO == activityId);
                DateTime fecha = DateTime.Parse(activity.FECHA.ToString());

                for (int i = 0; i < studentsMails.Length; i++)
                {
                    Console.WriteLine(studentsMails[i]);
                    if (Utils.IsValidEmail(studentsMails[i].correo.Trim()))
                    {
                        string to = studentsMails[i].correo.Trim();
                        string subject = "Invitaci\u00F3n a la actividad (Bienestar Estudiantil)";
                        string body = "Estimado/a: \nDebe asistir a la actividad ''" + activity.NOMBRE + "'', que se efectuar\u00E1 en la fecha: " + fecha.ToString("dd/MM/yyyy") + " en el lugar: ''" + activity.OBSERVACION + "''. \nEsperamos su asistencia.";
                        Utils.sendMail(to, subject, body);
                        saveAssistanceNotification(studentsMails[i].codigo);
                    }
                }

                response = new Response(true, "info", "Actualizar", "La notificaci\u00F3n fu\u00E9 enviada a los estudiantes correctamente", null);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                Console.WriteLine(ex.Message);
                response = new Response(false, "error", "Error", "Error al obtener los datos para enviar la notificaci\u00F3n", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            catch (InvalidOperationException)
            {
                response = new Response(false, "error", "Error", "Error al obtener los datos para enviar la notificaci\u00F3n", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
            catch (Exception)
            {
                response = new Response(false, "error", "Error", "Error al enviar la notificaci\u00F3n a los estudiantes", null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }

            writeResponse(new JavaScriptSerializer().Serialize(response));
        }


        /**
         * guarda la notificacion en la asistencia del estudiante
         **/
        private void saveAssistanceNotification(int codigo)
        {
            bienestarEntities db = new bienestarEntities();
            // guarda la notificacion en la asistencia
            BE_ASISTENCIA assistance = db.BE_ASISTENCIA.Single(a => a.CODIGO == codigo);
            assistance.NOTIFICACIONENVIADA = assistance.NOTIFICACIONENVIADA == null ? 1 : assistance.NOTIFICACIONENVIADA + 1;
            db.SaveChanges();
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

        [WebMethod(EnableSession = true)]
        [ScriptMethod(UseHttpGet = true)]
        public void exportExcelReport(DateTime dateFrom, DateTime dateTo)
        {
            string[] header = { "FECHA", "ACTIVIDAD", "A.GENERAL", "ESTADO", "ASIST.", "NIVEL", "CARRERA", "MOD.", "DOC.ADJ." };
            List<string[]> rows = exportExcelReportData(dateFrom, dateTo);
            string response = Utils.MakeHtmlTable(header, rows);
            Utils.writeResponseExcel(response, "actividades.xls");
        }

        public List<string[]> exportExcelReportData(DateTime dateFrom, DateTime dateTo)
        {
            List<string[]> rows = new List<string[]>();

            try
            {
                bienestarEntities db = new bienestarEntities();

                int userCode = (int)Utils.getSession("userCode");
                bool isTeacher = Utils.isTeacher(userCode);
                DateTime lastDate = dateFrom.AddDays(-1);
                var actividades = db.BE_ACTIVIDAD.Join(db.BE_ACTIVIDAD_GENERAL, ac => ac.CODIGOACTIVIDADGENERAL, ag => ag.CODIGO, (ac, ag) => new { ac, ag }).
                    Select(s => new
                    {
                        CODIGOUSUARIO = s.ac.CODIGOUSUARIO,
                        CODIGO = s.ac.CODIGO,
                        ACTIVIDAD = s.ac.NOMBRE,
                        ACTIVIDADGENERAL = s.ag.NOMBRE,
                        FECHA = s.ac.FECHA,
                        ESTADO = s.ac.ESTADO,
                        DATOS = new Datos()
                    }).Where(w => w.FECHA > lastDate && w.FECHA <= dateTo).OrderBy(o => o.FECHA).ToList();

                foreach (var actividad in actividades)
                {
                    Datos datos = getNivelCarreraModalidad(actividad.CODIGO);
                    actividad.DATOS.ADJUNTOS = datos.ADJUNTOS;
                    actividad.DATOS.NIVELES = datos.NIVELES;
                    actividad.DATOS.MODALIDADES = datos.MODALIDADES;
                    actividad.DATOS.CARRERAS = datos.CARRERAS;
                    actividad.DATOS.ASISTENCIA = getActitityAssistance(actividad.CODIGO);
                }

                if (isTeacher)
                {
                    actividades.Where(w => w.CODIGOUSUARIO == userCode);
                }

                for (var i = 0; i < actividades.Count; i++)
                {
                    string[] row = new string[9];

                    row[0] = actividades[i].FECHA.ToString();
                    row[1] = actividades[i].ACTIVIDAD;
                    row[2] = actividades[i].ACTIVIDADGENERAL;
                    row[3] = actividades[i].ESTADO == 0 ? "Inactivo" : actividades[i].ESTADO == 1 ? "En proceso" : actividades[i].ESTADO == 2 ? "Procesado" : "Finalizado";
                    row[4] = actividades[i].DATOS.ASISTENCIA.ToString();

                    String niveles = "";

                    for (var j = 0; j < actividades[i].DATOS.NIVELES.Count; j++)
                    {
                        if (niveles != "")
                            niveles = niveles + "\n" + actividades[i].DATOS.NIVELES[j];
                        else
                            niveles = actividades[i].DATOS.NIVELES[j];
                    }
                    row[5] = niveles;

                    String carreras = "";

                    for (var j = 0; j < actividades[i].DATOS.CARRERAS.Count; j++)
                    {
                        if (carreras != "")
                            carreras = carreras + "\n" + actividades[i].DATOS.CARRERAS[j];
                        else
                            carreras = actividades[i].DATOS.CARRERAS[j];
                    }
                    row[6] = carreras;

                    String modalidades = "";

                    for (var j = 0; j < actividades[i].DATOS.MODALIDADES.Count; j++)
                    {
                        if (modalidades != "")
                            modalidades = modalidades + "\n" + actividades[i].DATOS.MODALIDADES[j];
                        else
                            modalidades = actividades[i].DATOS.MODALIDADES[j];
                    }
                    row[7] = modalidades;

                    String adjuntos = "";

                    for (var j = 0; j < actividades[i].DATOS.ADJUNTOS.Count; j++)
                    {
                        if (adjuntos != "")
                            adjuntos = adjuntos + "\n" + actividades[i].DATOS.ADJUNTOS[j];
                        else
                            adjuntos = actividades[i].DATOS.ADJUNTOS[j];
                    }
                    row[8] = adjuntos;
                    rows.Add(row);
                }
            }
            catch (Exception e)
            {
                // Error al eliminar el rol
                rows.Add(new string[] { e.ToString() });
            }

            return rows;
        }
    }

    //
    public class Estudiante
    {
        public Int32 codigo { set; get; }
        public string correo { set; get; }

        public Estudiante(Int32 codigo, string correo)
        {
            this.codigo = codigo;
            this.correo = correo;
        }
        public Estudiante() { }
    }

    //
    public class Datos
    {
        public List<string> NIVELES { set; get; }
        public List<string> CARRERAS { set; get; }
        public List<string> MODALIDADES { set; get; }
        public List<string> ADJUNTOS { set; get; }
        public int ASISTENCIA { set; get; }
        public Datos(List<string> n, List<string> c, List<string> m, List<string> a)
        {
            NIVELES = n;
            CARRERAS = c;
            MODALIDADES = m;
            ADJUNTOS = a;
        }
        public Datos() { }
    }

    //
    public class Asistencia
    {
        public int cantidad { set; get; }
    }
}
