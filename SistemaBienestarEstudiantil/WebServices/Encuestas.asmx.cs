using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Data.Objects.DataClasses;
using System.Web.Script.Services;
using System.Data.Objects.SqlClient;
using SistemaBienestarEstudiantil.Models;
using SistemaBienestarEstudiantil.Class;

namespace SistemaBienestarEstudiantil.WebServices
{
    /// <summary>
    /// Descripción breve de Encuestas
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio Web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class Encuestas : System.Web.Services.WebService
    {
        private void writeResponse(String response)
        {
            Context.Response.Write(response);
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void getAllEncuestas()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            writeResponse(new JavaScriptSerializer().Serialize(db.BE_ENCUESTA.ToList()));
        }

        [WebMethod]
        public void getDefaultSurvey()
        {
            try {
                Models.bienestarEntities db = new Models.bienestarEntities();

                Models.BE_DATOS_SISTEMA datoSistema = db.BE_DATOS_SISTEMA.Single(ds => ds.NOMBRE == "ENCUESTA");

                int valor = datoSistema.VALOR != null ? Int32.Parse(datoSistema.VALOR) : 0;

                var encuesta = db.BE_ENCUESTA.Single(e => e.CODIGO == valor);
                writeResponse(
                    new JavaScriptSerializer().Serialize(new Class.Response(encuesta != null ? true : false, "info", null, null, encuesta))
                );
                }
            catch (Exception e)
            {
                Response response = new Response(false, "error", "Error", e.ToString(), null);
                writeResponse(new JavaScriptSerializer().Serialize(response));
            }
        }

        [WebMethod(EnableSession = true)]
        public void setDefaultSurvey(int surveyCode)
        {
            if (Utils.haveAccessTo(Utils.MODULOENCUESTAS))
            {
                Models.bienestarEntities db = new Models.bienestarEntities();

                Models.BE_DATOS_SISTEMA datosSistema = db.BE_DATOS_SISTEMA.Single(ds => ds.NOMBRE == "ENCUESTA");

                bool success = false;

                if (datosSistema != null)
                {
                    datosSistema.VALOR = surveyCode.ToString();
                    success = true;
                }

                db.SaveChanges();
                writeResponse(new JavaScriptSerializer().Serialize(new Class.Response(success, null, null, null, null)));
            }
        }

        [WebMethod(EnableSession = true)]
        public void saveEncuesta(Encuesta encuestaEdited)
        {
            Models.BE_ENCUESTA updatedEncuesta = convertToENCUESTA(encuestaEdited);

            if (Utils.haveAccessTo(Utils.MODULOENCUESTAS))
            {
                using (Models.bienestarEntities db = new Models.bienestarEntities())
                {
                    Models.BE_ENCUESTA currentEncuesta = db.BE_ENCUESTA.Single(e => e.CODIGO == updatedEncuesta.CODIGO);

                    currentEncuesta.BE_ENCUESTA_PREGUNTA.ToList().ForEach(ep => db.BE_ENCUESTA_PREGUNTA.DeleteObject(ep));
                    db.SaveChanges();

                    foreach (Models.BE_ENCUESTA_PREGUNTA ep in updatedEncuesta.BE_ENCUESTA_PREGUNTA)
                    {
                        ep.CODIGO = default(int);
                        foreach (Models.BE_ENCUESTA_RESPUESTA er in ep.BE_ENCUESTA_RESPUESTA)
                        {
                            er.CODIGO = default(int);
                        }
                    }
                }

                using (Models.bienestarEntities db = new Models.bienestarEntities())
                {

                    Models.BE_ENCUESTA currentEncuesta = db.BE_ENCUESTA.Single(e => e.CODIGO == updatedEncuesta.CODIGO);
                    currentEncuesta.DESCRIPCION = updatedEncuesta.DESCRIPCION;
                    currentEncuesta.TITULO = updatedEncuesta.TITULO;
                    foreach (Models.BE_ENCUESTA_PREGUNTA ep in updatedEncuesta.BE_ENCUESTA_PREGUNTA.ToList())
                    {
                        currentEncuesta.BE_ENCUESTA_PREGUNTA.Add(ep);
                    }
                    db.SaveChanges();
                    writeResponse(new JavaScriptSerializer().Serialize(currentEncuesta));
                }
            }
        }

        [WebMethod(EnableSession = true)]
        public void addNewEncuesta(Encuesta encuesta)
        {
            if (Utils.haveAccessTo(Utils.MODULOENCUESTAS))
            {
                Models.bienestarEntities db = new Models.bienestarEntities();
                Models.BE_ENCUESTA newEncuesta = convertToENCUESTA(encuesta);
                newEncuesta.FECHA = DateTime.Now;
                db.BE_ENCUESTA.AddObject(newEncuesta);
                db.SaveChanges();
                writeResponse(new JavaScriptSerializer().Serialize(newEncuesta));
            }
        }

        [WebMethod(EnableSession = true)]
        public void removeEncuestaByCode(int code)
        {
            if (Utils.haveAccessTo(Utils.MODULOENCUESTAS))
            {
                Models.bienestarEntities db = new Models.bienestarEntities();
                Models.BE_ENCUESTA encuesta = db.BE_ENCUESTA.Single(e => e.CODIGO == code);
                db.BE_ENCUESTA.DeleteObject(encuesta);
                db.SaveChanges();
                writeResponse("ok");
            }
        }

        /// convert Simple class Encuesta to object ENCUESTA entity
        /// all list in objects to entityCollection
        private Models.BE_ENCUESTA convertToENCUESTA(Encuesta encuesta)
        {
            Models.BE_ENCUESTA encuestaEntity = new Models.BE_ENCUESTA();

            if (encuesta.CODIGO != 0) encuestaEntity.CODIGO = encuesta.CODIGO;
            encuestaEntity.TITULO = encuesta.TITULO;
            encuestaEntity.DESCRIPCION = encuesta.DESCRIPCION;

            EntityCollection<Models.BE_ENCUESTA_PREGUNTA> encPreEntColl = new EntityCollection<Models.BE_ENCUESTA_PREGUNTA>();

            foreach (BE_ENCUESTA_PREGUNTA  ep in encuesta.BE_ENCUESTA_PREGUNTA)
            {
                Models.BE_ENCUESTA_PREGUNTA epEntity = new Models.BE_ENCUESTA_PREGUNTA();
                if (ep.CODIGO != 0) epEntity.CODIGO = ep.CODIGO;
                epEntity.TITULO = ep.TITULO;
                epEntity.TIPO = ep.TIPO;
                epEntity.REQUERIDO = ep.REQUERIDO;

                EntityCollection<Models.BE_ENCUESTA_RESPUESTA> encResEntColl = new EntityCollection<Models.BE_ENCUESTA_RESPUESTA>();
                foreach (BE_ENCUESTA_RESPUESTA er in ep.BE_ENCUESTA_RESPUESTA)
                {
                    Models.BE_ENCUESTA_RESPUESTA erEntity = new Models.BE_ENCUESTA_RESPUESTA();
                    if (er.CODIGO != 0) erEntity.CODIGO = er.CODIGO;
                    erEntity.TEXTO = er.TEXTO;
                    encResEntColl.Add(erEntity);
                }

                epEntity.BE_ENCUESTA_RESPUESTA = encResEntColl;
                encPreEntColl.Add(epEntity);
            }

            encuestaEntity.BE_ENCUESTA_PREGUNTA = encPreEntColl;
            return encuestaEntity;
        }

        /// metodo devuelve el alumno si puede hacer encuesta
        [WebMethod]
        public void getStudentByCedula(string cedula, int codigoEncuesta)
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            GRADUADO alumno = db.GRADUADOS.Where(a => a.DTPCEDULAC == cedula).FirstOrDefault();

            DateTime today = DateTime.Now;

            BE_ENCUESTA_RESPUESTA_ALUMNO be_era = db.BE_ENCUESTA_RESPUESTA_ALUMNO.Where(era => era.CODIGOGRADUADO == alumno.GRDCODIGOI && era.CODIGOENCUESTA == codigoEncuesta).FirstOrDefault();

            BE_ENCUESTA_RESPUESTA_TEXTO be_ert = db.BE_ENCUESTA_RESPUESTA_TEXTO.Where(ert => ert.CODIGOGRADUADO == alumno.GRDCODIGOI && ert.CODIGOENCUESTA == codigoEncuesta).FirstOrDefault();

            int periodo1 = 0;
            int periodo2 = 0;

            if (be_era != null)
            {
                periodo1 = Utils.getPeriodo(be_era.FECHA);
            }
            else if (be_ert != null)
            {
                periodo1 = Utils.getPeriodo(be_ert.FECHA);
            }

            periodo2 = Utils.getPeriodo(today);

            if (periodo1 == 0 || (periodo1 != periodo2))
            {
                writeResponse(new JavaScriptSerializer().Serialize(alumno));
            }
            else
            {
                writeResponse(new JavaScriptSerializer().Serialize(null));
            }
        }

        /// <summary>
        /// devuelve todos los periodos
        /// </summary>
        [WebMethod]
        public void getPeriodos()
        {
            bienestarEntities db = new bienestarEntities();
            Utils.writeResponseObject(db.PERIODOes.Where(p => p.TPECODIGOI == 1).ToList());
        }

        /// <summary>
        /// guarda la respuesta a la encuesta del estudiante
        /// </summary>
        /// <param name="listResponseSelect"></param>
        /// <param name="listResponseText"></param>
        [WebMethod]
        public void saveResponseStudent(List<BE_ENCUESTA_RESPUESTA_ALUMNO> listResponseSelect, List<BE_ENCUESTA_RESPUESTA_TEXTO> listResponseText)
        {
            bienestarEntities db = new bienestarEntities();
            foreach (BE_ENCUESTA_RESPUESTA_ALUMNO era in listResponseSelect)
            {
                //era.FECHA = DateTime.Now;
                db.BE_ENCUESTA_RESPUESTA_ALUMNO.AddObject(era);
            }

            foreach (BE_ENCUESTA_RESPUESTA_TEXTO ert in listResponseText)
            {
                //ert.FECHA = DateTime.Now;
                db.BE_ENCUESTA_RESPUESTA_TEXTO.AddObject(ert);
            }

            db.SaveChanges();
            writeResponse("ok");
        }

        /// <summary>
        /// devuelve json para el reporte de las respuestas de la encuesta
        /// </summary>
        /// <param name="surveyCode"></param>
        [WebMethod]
        public void surveysReport(int surveyCode, DateTime iniDate, DateTime? endDate)
        {
            bienestarEntities db = new bienestarEntities();
            BE_ENCUESTA encuesta = db.BE_ENCUESTA.Single(e => e.CODIGO == surveyCode);

            List<ResultadoSeleccione> resultadoSeleccione = new List<ResultadoSeleccione>();

            // cada pregunta
            foreach (Models.BE_ENCUESTA_PREGUNTA enpre in encuesta.BE_ENCUESTA_PREGUNTA)
            {
                ResultadoSeleccione rs = new ResultadoSeleccione(enpre.TIPO, enpre.TITULO, new List<Respuesta>());
                if (enpre.TIPO != 3)
                {
                    // cada respuesta
                    foreach (Models.BE_ENCUESTA_RESPUESTA enres in enpre.BE_ENCUESTA_RESPUESTA)
                    {
                        rs.respuestas.Add(new Respuesta(enres.TEXTO, enres.BE_ENCUESTA_RESPUESTA_ALUMNO.Where(era => (endDate != null ? iniDate <= era.FECHA && era.FECHA <= endDate : iniDate <= era.FECHA)).Count(), null));
                    }
                }
                else foreach (Models.BE_ENCUESTA_RESPUESTA_TEXTO ert in enpre.BE_ENCUESTA_RESPUESTA_TEXTO.Where(era => (endDate != null ? iniDate <= era.FECHA && era.FECHA <= endDate : iniDate <= era.FECHA)))
                {
                    rs.respuestas.Add(new Respuesta(null, 0, ert.TEXTO));
                }

                resultadoSeleccione.Add(rs);
            }

            // estudiantes que hicieron la encuesta
            List<int> studentCodesSelect = db.BE_ENCUESTA_RESPUESTA_ALUMNO.Where(era => era.CODIGOENCUESTA == surveyCode && (endDate != null ? iniDate <= era.FECHA && era.FECHA <= endDate : iniDate <= era.FECHA)).Select(a => a.CODIGOGRADUADO).Distinct().ToList();
            List<int> studentCodesParagr = db.BE_ENCUESTA_RESPUESTA_TEXTO.Where(era => era.CODIGOENCUESTA == surveyCode && (endDate != null ? iniDate <= era.FECHA && era.FECHA <= endDate : iniDate <= era.FECHA)).Select(a => a.CODIGOGRADUADO).Distinct().ToList();
            foreach (int studCode in studentCodesParagr)
            {
                if (!studentCodesSelect.Contains(studCode))
                {
                    studentCodesSelect.Add(studCode);
                }
            }

            List<GRADUADO> listAlumnos = db.GRADUADOS.Where(a => studentCodesSelect.Contains(a.GRDCODIGOI)).ToList();

            writeResponse(
                "{\"encuestados\": " + getSurveyAnsweredCount(surveyCode, iniDate, endDate) + ",\"" + "TITULO\":\"" + encuesta.TITULO + "\","
                + "\"" + "estudiantes\":" + new JavaScriptSerializer().Serialize(listAlumnos) + ","
                + "\"preguntas\":" + new JavaScriptSerializer().Serialize(resultadoSeleccione)
                + "}"
            );
        }

        //[ScriptMethod(UseHttpGet = true)]
        [WebMethod]
        public void surveyAnsweredCodesServices()
        {
            writeResponse(new JavaScriptSerializer().Serialize(getAnsweredSurveyCodes()));
        }

        /// <summary>
        /// devuelve la cantidad de estudiantes que hicieron la encuesta
        /// </summary>
        /// <param name="surveyCode"></param>
        /// <returns>cantidad</returns>
        private int getSurveyAnsweredCount(int surveyCode, DateTime iniDate, DateTime? endDate)
        {
            bienestarEntities db = new bienestarEntities();
            BE_ENCUESTA encuesta = db.BE_ENCUESTA.Single(e => e.CODIGO == surveyCode);

            int stuResp = db.BE_ENCUESTA_RESPUESTA_ALUMNO.Where(era => era.CODIGOENCUESTA == surveyCode && (endDate != null ? iniDate <= era.FECHA && era.FECHA <= endDate : iniDate <= era.FECHA)).Select(a => a.CODIGOGRADUADO).Distinct().Count();
            int stuText = db.BE_ENCUESTA_RESPUESTA_TEXTO.Where(era => era.CODIGOENCUESTA == surveyCode && (endDate != null ? iniDate <= era.FECHA && era.FECHA <= endDate : iniDate <= era.FECHA)).Select(a => a.CODIGOGRADUADO).Distinct().Count();

            return stuResp > stuText ? stuResp : stuText;
        }

        /// <summary>
        /// devuelve la cantidad de estudiantes que hicieron la encuesta
        /// </summary>
        /// <param name="surveyCode"></param>
        /// <returns>cantidad</returns>
        private int getSurveyAnsweredCount(int surveyCode)
        {
            bienestarEntities db = new bienestarEntities();
            BE_ENCUESTA encuesta = db.BE_ENCUESTA.Single(e => e.CODIGO == surveyCode);

            int stuResp = db.BE_ENCUESTA_RESPUESTA_ALUMNO.Where(era => era.CODIGOENCUESTA == surveyCode).Select(a => a.CODIGOGRADUADO).Distinct().Count();
            int stuText = db.BE_ENCUESTA_RESPUESTA_TEXTO.Where(era => era.CODIGOENCUESTA == surveyCode).Select(a => a.CODIGOGRADUADO).Distinct().Count();

            return stuResp > stuText ? stuResp : stuText;
        }

        private List<int> getAnsweredSurveyCodes() {

            bienestarEntities db = new bienestarEntities();
            List<BE_ENCUESTA> listEncuestas = db.BE_ENCUESTA.ToList();

            List<int> surveyCodes = new List<int>();

            foreach (BE_ENCUESTA e in listEncuestas)
            {
                if (getSurveyAnsweredCount((int)e.CODIGO) > 0) {
                    surveyCodes.Add((int)e.CODIGO);
                }
            }

            return surveyCodes;
        }

    }

    public class ResultadoSeleccione
    {
        public ResultadoSeleccione(int tipo, string pregunta, List<Respuesta> respuestas)
        {
            this.tipo = tipo;
            this.pregunta = pregunta;
            this.respuestas = respuestas;
        }
        public int tipo { get; set; }
        public string pregunta { get; set; }
        public List<Respuesta> respuestas { get; set; }
    }

    public class Respuesta
    {
        public Respuesta(string nombre, int cantidad, string parrafo)
        {
            this.nombre = nombre;
            this.cantidad = cantidad;
            this.parrafo = parrafo;
        }
        public string nombre { get; set; }
        public int cantidad { get; set; }
        public string parrafo { get; set; }
    }

    public class Encuesta
    {
        public int CODIGO { get; set; }
        public String TITULO { get; set; }
        public String DESCRIPCION { get; set; }
        public List<BE_ENCUESTA_PREGUNTA> BE_ENCUESTA_PREGUNTA { get; set; }
    }

    public class BE_ENCUESTA_PREGUNTA
    {
        public int CODIGO { get; set; }
        public String TITULO { get; set; }
        public int TIPO { get; set; }
        public bool REQUERIDO { get; set; }
        public List<BE_ENCUESTA_RESPUESTA> BE_ENCUESTA_RESPUESTA { get; set; }
    }

    public class BE_ENCUESTA_RESPUESTA
    {
        public int CODIGO { get; set; }
        public String TEXTO { get; set; }
    }
}