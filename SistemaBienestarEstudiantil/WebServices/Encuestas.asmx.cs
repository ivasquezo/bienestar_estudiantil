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
            writeResponse(new JavaScriptSerializer().Serialize(db.ENCUESTAs.ToList()));
        }

        [WebMethod]
        public void pruebaJoins()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();

            /*
            var data = db.Categorie
            .Join(db.CategoryMap,
            cat => cat.CategoryId,
            catmap => catmap.ChildCategoryId,
            (cat, catmap) => new { Category = cat, CategoryMap = catmap })
            .Select(x => x.Category);
             */
            int code = 1;
            var data = db.ACCESOes.Join(db.ROL_ACCESO, a => a.CODIGO, ra => ra.CODIGOACCESO,
                       (a, ra) => new { ACCESO = a, ROL_ACCESO = ra })
                       .Select(x => new { x.ACCESO.NOMBRE, x.ROL_ACCESO.ACCESO.CODIGO, x.ROL_ACCESO.CODIGOROL, x.ROL_ACCESO.VALIDO })
                       .Where(y => y.CODIGOROL == code).ToList();

            writeResponse(new JavaScriptSerializer().Serialize(data));
        }

        [WebMethod]
        public void getDefaultSurvey()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();

            Models.DATOS_SISTEMA datoSistema = db.DATOS_SISTEMA.Single(ds => ds.NOMBRE == "ENCUESTA");

            int valor = datoSistema.VALOR != null ? Int32.Parse(datoSistema.VALOR) : 0;

            var encuesta = db.ENCUESTAs.Single(e => e.CODIGO == valor);
            writeResponse(
                new JavaScriptSerializer().Serialize(new Class.Response(encuesta != null ? true : false, "info", null, null, encuesta))
            );
        }

        [WebMethod]
        public void setDefaultSurvey(int surveyCode)
        {
            Models.bienestarEntities db = new Models.bienestarEntities();

            Models.DATOS_SISTEMA datosSistema = db.DATOS_SISTEMA.Single(ds => ds.NOMBRE == "ENCUESTA");

            bool success = false;

            if (datosSistema != null)
            {
                datosSistema.VALOR = surveyCode.ToString();
                success = true;
            }

            db.SaveChanges();

            writeResponse(new JavaScriptSerializer().Serialize(new Class.Response(success, null, null, null, null)));
        }

        [WebMethod]
        public void saveEncuesta(Encuesta encuestaEdited)
        {
            Models.ENCUESTA updatedEncuesta = convertToENCUESTA(encuestaEdited);

            using (Models.bienestarEntities db = new Models.bienestarEntities())
            {
                Models.ENCUESTA currentEncuesta = db.ENCUESTAs.Single(e => e.CODIGO == updatedEncuesta.CODIGO);

                currentEncuesta.ENCUESTA_PREGUNTA.ToList().ForEach(ep => db.ENCUESTA_PREGUNTA.DeleteObject(ep));
                db.SaveChanges();

                foreach (Models.ENCUESTA_PREGUNTA ep in updatedEncuesta.ENCUESTA_PREGUNTA)
                {
                    ep.CODIGO = default(int);
                    foreach (Models.ENCUESTA_RESPUESTA er in ep.ENCUESTA_RESPUESTA)
                    {
                        er.CODIGO = default(int);
                    }
                }
            }

            using(Models.bienestarEntities db = new Models.bienestarEntities()){

                Models.ENCUESTA currentEncuesta = db.ENCUESTAs.Single(e => e.CODIGO == updatedEncuesta.CODIGO);
                currentEncuesta.DESCRIPCION = updatedEncuesta.DESCRIPCION;
                currentEncuesta.TITULO = updatedEncuesta.TITULO;
                foreach (Models.ENCUESTA_PREGUNTA ep in updatedEncuesta.ENCUESTA_PREGUNTA.ToList())
                {
                    currentEncuesta.ENCUESTA_PREGUNTA.Add(ep);
                }
                db.SaveChanges();
                writeResponse(new JavaScriptSerializer().Serialize(currentEncuesta));
            }
        }

        [WebMethod]
        public void addNewEncuesta(Encuesta encuesta)
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            Models.ENCUESTA newEncuesta = convertToENCUESTA(encuesta);
            db.ENCUESTAs.AddObject(newEncuesta);
            db.SaveChanges();
            writeResponse(new JavaScriptSerializer().Serialize(newEncuesta));
        }

        [WebMethod]
        public void removeEncuestaByCode(int code)
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            Models.ENCUESTA encuesta = db.ENCUESTAs.Single(e => e.CODIGO == code);
            db.ENCUESTAs.DeleteObject(encuesta);
            db.SaveChanges();
            writeResponse("ok");
        }

        /// convert Simple class Encuesta to object ENCUESTA entity
        /// all list in objects to entityCollection
        private Models.ENCUESTA convertToENCUESTA(Encuesta encuesta)
        {
            Models.ENCUESTA encuestaEntity = new Models.ENCUESTA();

            if (encuesta.CODIGO != 0) encuestaEntity.CODIGO = encuesta.CODIGO;
            encuestaEntity.TITULO = encuesta.TITULO;
            encuestaEntity.DESCRIPCION = encuesta.DESCRIPCION;

            EntityCollection<Models.ENCUESTA_PREGUNTA> encPreEntColl = new EntityCollection<Models.ENCUESTA_PREGUNTA>();

            foreach (ENCUESTA_PREGUNTA  ep in encuesta.ENCUESTA_PREGUNTA)
            {
                Models.ENCUESTA_PREGUNTA epEntity = new Models.ENCUESTA_PREGUNTA();
                if (ep.CODIGO != 0) epEntity.CODIGO = ep.CODIGO;
                epEntity.TITULO = ep.TITULO;
                epEntity.TIPO = ep.TIPO;
                epEntity.REQUERIDO = ep.REQUERIDO;
                
                EntityCollection<Models.ENCUESTA_RESPUESTA> encResEntColl = new EntityCollection<Models.ENCUESTA_RESPUESTA>();
                foreach (ENCUESTA_RESPUESTA er in ep.ENCUESTA_RESPUESTA)
                {
                    Models.ENCUESTA_RESPUESTA erEntity = new Models.ENCUESTA_RESPUESTA();
                    if (er.CODIGO != 0) erEntity.CODIGO = er.CODIGO;
                    erEntity.TEXTO = er.TEXTO;
                    encResEntColl.Add(erEntity);
                }

                epEntity.ENCUESTA_RESPUESTA = encResEntColl;
                encPreEntColl.Add(epEntity);
            }

            encuestaEntity.ENCUESTA_PREGUNTA = encPreEntColl;
            return encuestaEntity;
        }

        /// metodo devuelve el alumno si no ha respondido encuesta
        [WebMethod]
        public void getStudentByCedula(Decimal cedula, int codigoEncuesta)
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            ALUMNO alumno = db.ALUMNOes.Single(a => a.CEDULA == cedula);

            int count = db.ENCUESTA_RESPUESTA_ALUMNO.Where(era => era.CODIGOALUMNO == alumno.CODIGO && era.CODIGOENCUESTA == codigoEncuesta).Count();
            count += db.ENCUESTA_RESPUESTA_TEXTO.Where(ert => ert.CODIGOALUMNO == alumno.CODIGO && ert.CODIGOENCUESTA == codigoEncuesta).Count();

            if (count == 0)
                writeResponse(new JavaScriptSerializer().Serialize(alumno));
            else
                writeResponse(new JavaScriptSerializer().Serialize(null));
        }

        /// <summary>
        /// guarda la respuesta a la encuesta del estudiante
        /// </summary>
        /// <param name="listResponseSelect"></param>
        /// <param name="listResponseText"></param>
        [WebMethod]
        public void saveResponseStudent(List<ENCUESTA_RESPUESTA_ALUMNO> listResponseSelect, List<ENCUESTA_RESPUESTA_TEXTO> listResponseText)
        {

            bienestarEntities db = new bienestarEntities();
            foreach (ENCUESTA_RESPUESTA_ALUMNO era in listResponseSelect)
            {
                db.ENCUESTA_RESPUESTA_ALUMNO.AddObject(era);
            }

            foreach (ENCUESTA_RESPUESTA_TEXTO ert in listResponseText)
            {
                db.ENCUESTA_RESPUESTA_TEXTO.AddObject(ert);
            }

            db.SaveChanges();
            writeResponse("ok");
        }

        /// <summary>
        /// devuelve json para el reporte de las respuestas de la encuesta
        /// </summary>
        /// <param name="surveyCode"></param>
        [WebMethod]
        public void surveysReport(int surveyCode)
        {
            bienestarEntities db = new bienestarEntities();
            ENCUESTA encuesta = db.ENCUESTAs.Single(e => e.CODIGO == surveyCode);

            List<ResultadoSeleccione> resultadoSeleccione = new List<ResultadoSeleccione>();

            // cada pregunta
            foreach (Models.ENCUESTA_PREGUNTA enpre in encuesta.ENCUESTA_PREGUNTA)
            {
                ResultadoSeleccione rs = new ResultadoSeleccione(enpre.TIPO, enpre.TITULO, new List<Respuesta>());
                if (enpre.TIPO != 3)
                {
                    // cada respuesta
                    foreach (Models.ENCUESTA_RESPUESTA enres in enpre.ENCUESTA_RESPUESTA)
                    {
                        rs.respuestas.Add(new Respuesta(enres.TEXTO, enres.ENCUESTA_RESPUESTA_ALUMNO.Count(), null));
                    }
                }
                else foreach (Models.ENCUESTA_RESPUESTA_TEXTO ert in enpre.ENCUESTA_RESPUESTA_TEXTO)
                {
                    rs.respuestas.Add(new Respuesta(null, 0, ert.TEXTO));
                }

                resultadoSeleccione.Add(rs);
            }

            // estudiantes que hicieron la encuesta
            List<Decimal> studentCodesSelect = db.ENCUESTA_RESPUESTA_ALUMNO.Where(era => era.CODIGOENCUESTA == surveyCode).Select(a => a.CODIGOALUMNO).Distinct().ToList();
            List<Decimal> studentCodesParagr = db.ENCUESTA_RESPUESTA_TEXTO.Where(era => era.CODIGOENCUESTA == surveyCode).Select(a => a.CODIGOALUMNO).Distinct().ToList();
            foreach (Decimal studCode in studentCodesParagr)
            {
                if (!studentCodesSelect.Contains(studCode))
                {
                    studentCodesSelect.Add(studCode);
                }
            }

            List<ALUMNO> listAlumnos = db.ALUMNOes.Where(a => studentCodesSelect.Contains(a.CODIGO)).ToList();

            writeResponse(
                "{\"encuestados\": " + getSurveyAnsweredCount(surveyCode) + ",\"" + "TITULO\":\"" + encuesta.TITULO + "\","
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
        private int getSurveyAnsweredCount(int surveyCode)
        {
            bienestarEntities db = new bienestarEntities();
            ENCUESTA encuesta = db.ENCUESTAs.Single(e => e.CODIGO == surveyCode);
            
            int stuResp = db.ENCUESTA_RESPUESTA_ALUMNO.Where(era => era.CODIGOENCUESTA == surveyCode).Select(a => a.CODIGOALUMNO).Distinct().Count();
            int stuText = db.ENCUESTA_RESPUESTA_TEXTO.Where(era => era.CODIGOENCUESTA == surveyCode).Select(a => a.CODIGOALUMNO).Distinct().Count();
            
            return stuResp > stuText ? stuResp : stuText;
        }

        private List<int> getAnsweredSurveyCodes() {

            bienestarEntities db = new bienestarEntities();
            List<ENCUESTA> listEncuestas = db.ENCUESTAs.ToList();

            List<int> surveyCodes = new List<int>();

            foreach (ENCUESTA e in listEncuestas)
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

        public List<ENCUESTA_PREGUNTA> ENCUESTA_PREGUNTA { get; set; }

    }

    public class ENCUESTA_PREGUNTA
    {
        public int CODIGO { get; set; }

        public String TITULO { get; set; }

        public int TIPO { get; set; }

        public bool REQUERIDO { get; set; }

        public List<ENCUESTA_RESPUESTA> ENCUESTA_RESPUESTA { get; set; }
    }

    public class ENCUESTA_RESPUESTA
    {
        public int CODIGO { get; set; }

        public String TEXTO { get; set; }
    }
}
