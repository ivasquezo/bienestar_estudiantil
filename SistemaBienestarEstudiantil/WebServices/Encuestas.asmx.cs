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
    /// WebService para la administracion de encuestas
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio Web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class Encuestas : System.Web.Services.WebService
    {
        /// <summary>
        /// Metodo comun para presentar los metodos del webservice en la vista
        /// </summary>
        /// <param name="response"></param>
        private void writeResponse(String response)
        {
            Context.Response.Write(response);
            Context.Response.Flush();
            Context.Response.End();
        }

        /// <summary>
        /// Obtiene todas las encuestas existentes en base
        /// </summary>
        [WebMethod]
        public void getAllEncuestas()
        {
            bienestarEntities db = new bienestarEntities();
            // Obtiene todas las encuestas existentes en base
            writeResponse(new JavaScriptSerializer().Serialize(db.BE_ENCUESTA.ToList()));
        }

        /// <summary>
        /// Obtiene la encuesta predeterminada
        /// </summary>
        [WebMethod]
        public void getDefaultSurvey()
        {
            bienestarEntities db = new bienestarEntities();
            // Obtiene los datos de la encuesta predeterminada
            BE_DATOS_SISTEMA datoSistema = db.BE_DATOS_SISTEMA.Single(ds => ds.NOMBRE == "ENCUESTA");
            // Obtiene el codigo de la encuesta predeterminada
            int valor = datoSistema.VALOR != null ? Int32.Parse(datoSistema.VALOR) : 0;
            // Obtiene la encuesta a partir del codigo de la encuesta predeterminada
            var encuesta = db.BE_ENCUESTA.Where(e => e.CODIGO == valor).FirstOrDefault();

            Utils.writeResponseObject(new Class.Response(encuesta != null ? true : false, "info", null, null, encuesta));
        }

        /// <summary>
        /// Actualiza la encuesta predeterminada
        /// </summary>
        /// <param name="surveyCode"></param>
        [WebMethod(EnableSession = true)]
        public void setDefaultSurvey(int surveyCode)
        {
            // Valida si el usuario logueado tiene acceso al modulo
            if (Utils.haveAccessTo(Utils.MODULOENCUESTAS))
            {
                bienestarEntities db = new bienestarEntities();
                // Obtiene los datos de la encuesta predeterminada
                BE_DATOS_SISTEMA datosSistema = db.BE_DATOS_SISTEMA.Single(ds => ds.NOMBRE == "ENCUESTA");

                bool success = false;
                // Si se encuentra datos de una encuesta predetermnada
                if (datosSistema != null)
                {
                    // Actualiza el codigo de la encuesta predeterminada
                    datosSistema.VALOR = surveyCode.ToString();
                    success = true;
                }
                // Guarda la encuesta predeterminada
                db.SaveChanges();
                writeResponse(new JavaScriptSerializer().Serialize(new Class.Response(success, null, null, null, null)));
            }
        }

        /// <summary>
        /// Convierte una simple clase Encuesta a una entidad ENCUESTA
        /// </summary>
        /// <param name="encuesta"></param>
        /// <returns></returns>
        private Models.BE_ENCUESTA convertToENCUESTA(Encuesta encuesta)
        {
            BE_ENCUESTA encuestaEntity = new BE_ENCUESTA();
            // Actualiza la encuesta en objeto con los datos de la clase
            if (encuesta.CODIGO != 0) encuestaEntity.CODIGO = encuesta.CODIGO;
            encuestaEntity.TITULO = encuesta.TITULO;
            encuestaEntity.DESCRIPCION = encuesta.DESCRIPCION;

            EntityCollection<Models.BE_ENCUESTA_PREGUNTA> encPreEntColl = new EntityCollection<Models.BE_ENCUESTA_PREGUNTA>();
            // Actualiza las preguntas de la encuesta en objeto con los datos de la clase
            foreach (BE_ENCUESTA_PREGUNTA ep in encuesta.BE_ENCUESTA_PREGUNTA)
            {
                Models.BE_ENCUESTA_PREGUNTA epEntity = new Models.BE_ENCUESTA_PREGUNTA();

                if (ep.CODIGO != 0) epEntity.CODIGO = ep.CODIGO;
                epEntity.TITULO = ep.TITULO;
                epEntity.TIPO = ep.TIPO;
                epEntity.REQUERIDO = ep.REQUERIDO;

                EntityCollection<Models.BE_ENCUESTA_RESPUESTA> encResEntColl = new EntityCollection<Models.BE_ENCUESTA_RESPUESTA>();
                // Actualiza las respuestas de las preguntas en objeto con los datos de la clase
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

        /// <summary>
        /// Actualiza una encuesta determinada
        /// </summary>
        /// <param name="encuestaEdited"></param>
        [WebMethod(EnableSession = true)]
        public void saveEncuesta(Encuesta encuestaEdited)
        {
            // Convierte la clase encuesta a una entidad Encuesta
            BE_ENCUESTA updatedEncuesta = convertToENCUESTA(encuestaEdited);
            // Valida si el usuario logueado tiene acceso al modulo
            if (Utils.haveAccessTo(Utils.MODULOENCUESTAS))
            {
                using (bienestarEntities db = new bienestarEntities())
                {
                    BE_ENCUESTA currentEncuesta = db.BE_ENCUESTA.Single(e => e.CODIGO == updatedEncuesta.CODIGO);

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

                using (bienestarEntities db = new bienestarEntities())
                {
                    BE_ENCUESTA currentEncuesta = db.BE_ENCUESTA.Single(e => e.CODIGO == updatedEncuesta.CODIGO);

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

        /// <summary>
        /// Agrega una nueva encuesta
        /// </summary>
        /// <param name="encuesta"></param>
        [WebMethod(EnableSession = true)]
        public void addNewEncuesta(Encuesta encuesta)
        {
            if (Utils.haveAccessTo(Utils.MODULOENCUESTAS))
            {
                bienestarEntities db = new bienestarEntities();
                // Convierte la clase encuesta a una entidad Encuesta
                BE_ENCUESTA newEncuesta = convertToENCUESTA(encuesta);
                newEncuesta.FECHA = DateTime.Now;
                db.BE_ENCUESTA.AddObject(newEncuesta);

                db.SaveChanges();
                writeResponse(new JavaScriptSerializer().Serialize(newEncuesta));
            }
        }

        /// <summary>
        /// Eliminar una encuesta por codigo
        /// </summary>
        /// <param name="code"></param>
        [WebMethod(EnableSession = true)]
        public void removeEncuestaByCode(int code)
        {
            // Valida si el usuario logueado tiene acceso al modulo
            if (Utils.haveAccessTo(Utils.MODULOENCUESTAS))
            {
                bienestarEntities db = new bienestarEntities();
                // Obtener la encuesta que se va a eliminar
                BE_ENCUESTA encuesta = db.BE_ENCUESTA.Single(e => e.CODIGO == code);
                db.BE_ENCUESTA.DeleteObject(encuesta);
                db.SaveChanges();
                writeResponse("ok");
            }
        }

        /// <summary>
        /// Verifica si el alumno es graduado y puede realizar la encuesta
        /// </summary>
        /// <param name="cedula"></param>
        /// <param name="codigoEncuesta"></param>
        [WebMethod]
        public void getStudentByCedula(string cedula, int codigoEncuesta)
        {
            bienestarEntities db = new bienestarEntities();
            // Verifica si el estudiante es graduado mediante su numero de cedula
            GRADUADO alumno = db.GRADUADOS.Where(a => a.DTPCEDULAC == cedula).FirstOrDefault();
            // Fecha actual
            DateTime today = DateTime.Now;
            // Obtiene las respuestas de la encuesta del alumno que ingreso el numero de cedula
            BE_ENCUESTA_RESPUESTA_ALUMNO be_era = db.BE_ENCUESTA_RESPUESTA_ALUMNO.Where(era => era.CODIGOGRADUADO == alumno.GRDCODIGOI && era.CODIGOENCUESTA == codigoEncuesta).FirstOrDefault();
            // Obtiene las respuestas tipo parrafo del alumnoque ingreso el numero de cedula
            BE_ENCUESTA_RESPUESTA_TEXTO be_ert = db.BE_ENCUESTA_RESPUESTA_TEXTO.Where(ert => ert.CODIGOGRADUADO == alumno.GRDCODIGOI && ert.CODIGOENCUESTA == codigoEncuesta).FirstOrDefault();

            int periodo1 = 0;
            int periodo2 = 0;
            // Verifica los periodos en los que el alumno contesto la encuesta si lo hizo
            if (be_era != null)
            {
                periodo1 = Utils.getPeriodo(be_era.FECHA);
            }
            else if (be_ert != null)
            {
                periodo1 = Utils.getPeriodo(be_ert.FECHA);
            }

            periodo2 = Utils.getPeriodo(today);
            // Verifica si la encuesta fue contestada en el periodo actual o en un periodo pasado
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
        /// Devuelve todos los periodos
        /// </summary>
        [WebMethod]
        public void getPeriodos()
        {
            bienestarEntities db = new bienestarEntities();
            // Obtiene todos los periodos registrados
            Utils.writeResponseObject(db.PERIODOes.Where(p => p.TPECODIGOI == 1).ToList());
        }

        /// <summary>
        /// Guarda la respuesta a la encuesta del estudiante
        /// </summary>
        /// <param name="listResponseSelect"></param>
        /// <param name="listResponseText"></param>
        [WebMethod]
        public void saveResponseStudent(List<BE_ENCUESTA_RESPUESTA_ALUMNO> listResponseSelect, List<BE_ENCUESTA_RESPUESTA_TEXTO> listResponseText)
        {
            bienestarEntities db = new bienestarEntities();
            // Agregar las respuestas del alumno
            foreach (BE_ENCUESTA_RESPUESTA_ALUMNO era in listResponseSelect)
            {
                db.BE_ENCUESTA_RESPUESTA_ALUMNO.AddObject(era);
            }
            // Agragar las respuestas tipo texto del alumno
            foreach (BE_ENCUESTA_RESPUESTA_TEXTO ert in listResponseText)
            {
                db.BE_ENCUESTA_RESPUESTA_TEXTO.AddObject(ert);
            }

            db.SaveChanges();
            writeResponse("ok");
        }

        /// <summary>
        /// Devuelve json para el reporte de las respuestas de la encuesta
        /// </summary>
        /// <param name="surveyCode"></param>
        /// <param name="iniDate"></param>
        /// <param name="endDate"></param>
        [WebMethod]
        public void surveysReport(int surveyCode, DateTime iniDate, DateTime? endDate)
        {
            bienestarEntities db = new bienestarEntities();
            // Obtiene la encuesta seleccionada
            BE_ENCUESTA encuesta = db.BE_ENCUESTA.Single(e => e.CODIGO == surveyCode);

            List<ResultadoSeleccione> resultadoSeleccione = new List<ResultadoSeleccione>();
            // Obtiene el reporte de cada pregunta
            foreach (Models.BE_ENCUESTA_PREGUNTA enpre in encuesta.BE_ENCUESTA_PREGUNTA)
            {
                ResultadoSeleccione rs = new ResultadoSeleccione(enpre.TIPO, enpre.TITULO, new List<Respuesta>());
                // Si las respuestas son distinta a parrafo
                if (enpre.TIPO != 3)
                {
                    // Realiza el conteo de las respuestas de cada pregunta
                    foreach (Models.BE_ENCUESTA_RESPUESTA enres in enpre.BE_ENCUESTA_RESPUESTA)
                    {
                        rs.respuestas.Add(new Respuesta(enres.TEXTO, enres.BE_ENCUESTA_RESPUESTA_ALUMNO.Where(era => (endDate != null ? iniDate <= era.FECHA && era.FECHA <= endDate : iniDate <= era.FECHA)).Count(), null));
                    }
                }
                else
                {
                    // Setea la lista de los textos
                    foreach (Models.BE_ENCUESTA_RESPUESTA_TEXTO ert in enpre.BE_ENCUESTA_RESPUESTA_TEXTO.Where(era => (endDate != null ? iniDate <= era.FECHA && era.FECHA <= endDate : iniDate <= era.FECHA)))
                    {
                        rs.respuestas.Add(new Respuesta(null, 0, ert.TEXTO));
                    }
                }

                resultadoSeleccione.Add(rs);
            }
            // Estudiantes que hicieron la encuesta
            List<int> studentCodesSelect = db.BE_ENCUESTA_RESPUESTA_ALUMNO.Where(era => era.CODIGOENCUESTA == surveyCode && (endDate != null ? iniDate <= era.FECHA && era.FECHA <= endDate : iniDate <= era.FECHA)).Select(a => a.CODIGOGRADUADO).Distinct().ToList();
            List<int> studentCodesParagr = db.BE_ENCUESTA_RESPUESTA_TEXTO.Where(era => era.CODIGOENCUESTA == surveyCode && (endDate != null ? iniDate <= era.FECHA && era.FECHA <= endDate : iniDate <= era.FECHA)).Select(a => a.CODIGOGRADUADO).Distinct().ToList();
            foreach (int studCode in studentCodesParagr)
            {
                if (!studentCodesSelect.Contains(studCode))
                {
                    studentCodesSelect.Add(studCode);
                }
            }
            // Lista de los alumnos que contestaron la encuesta
            List<GRADUADO> listAlumnos = db.GRADUADOS.Where(a => studentCodesSelect.Contains(a.GRDCODIGOI)).ToList();

            writeResponse("{\"encuestados\": " + getSurveyAnsweredCount(surveyCode, iniDate, endDate) + ",\"" + "TITULO\":\""
                + encuesta.TITULO + "\"," + "\"" + "estudiantes\":" + new JavaScriptSerializer().Serialize(listAlumnos) + ","
                + "\"preguntas\":" + new JavaScriptSerializer().Serialize(resultadoSeleccione) + "}");
        }

        /// <summary>
        /// Devuelve la cantidad de estudiantes que hicieron la encuesta en un periodo determinado
        /// </summary>
        /// <param name="surveyCode"></param>
        /// <returns>cantidad</returns>
        private int getSurveyAnsweredCount(int surveyCode, DateTime iniDate, DateTime? endDate)
        {
            bienestarEntities db = new bienestarEntities();
            // Contabiliza los estudiantes que respondieron una encuesta determinada en un periodo determinado
            int stuResp = db.BE_ENCUESTA_RESPUESTA_ALUMNO.Where(era => era.CODIGOENCUESTA == surveyCode && (endDate != null ? iniDate <= era.FECHA && era.FECHA <= endDate : iniDate <= era.FECHA)).Select(a => a.CODIGOGRADUADO).Distinct().Count();
            int stuText = db.BE_ENCUESTA_RESPUESTA_TEXTO.Where(era => era.CODIGOENCUESTA == surveyCode && (endDate != null ? iniDate <= era.FECHA && era.FECHA <= endDate : iniDate <= era.FECHA)).Select(a => a.CODIGOGRADUADO).Distinct().Count();

            return stuResp > stuText ? stuResp : stuText;
        }

        /// <summary>
        /// Servicio que obtiene los codigos de las encuestas respondidas
        /// </summary>
        [WebMethod]
        public void surveyAnsweredCodesServices()
        {
            writeResponse(new JavaScriptSerializer().Serialize(getAnsweredSurveyCodes()));
        }

        /// <summary>
        /// Obtiene los codigos de las encuestas respondidas
        /// </summary>
        /// <returns></returns>
        private List<int> getAnsweredSurveyCodes()
        {
            bienestarEntities db = new bienestarEntities();
            // Obtiene las encuestas
            List<BE_ENCUESTA> listEncuestas = db.BE_ENCUESTA.ToList();

            List<int> surveyCodes = new List<int>();

            foreach (BE_ENCUESTA e in listEncuestas)
            {
                // Si la encuesta ra fue respondida por los estudiantes
                if (getSurveyAnsweredCount((int)e.CODIGO) > 0)
                {
                    // Codigos de las encuestas contestadas
                    surveyCodes.Add((int)e.CODIGO);
                }
            }

            return surveyCodes;
        }

        /// <summary>
        /// Devuelve la cantidad de estudiantes que hicieron la encuesta
        /// </summary>
        /// <param name="surveyCode"></param>
        /// <returns></returns>
        private int getSurveyAnsweredCount(int surveyCode)
        {
            bienestarEntities db = new bienestarEntities();
            // Contabiliza los estudiantes que respondieron una encuesta determinada
            int stuResp = db.BE_ENCUESTA_RESPUESTA_ALUMNO.Where(era => era.CODIGOENCUESTA == surveyCode).Select(a => a.CODIGOGRADUADO).Distinct().Count();
            int stuText = db.BE_ENCUESTA_RESPUESTA_TEXTO.Where(era => era.CODIGOENCUESTA == surveyCode).Select(a => a.CODIGOGRADUADO).Distinct().Count();

            return stuResp > stuText ? stuResp : stuText;
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