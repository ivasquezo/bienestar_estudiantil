﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Data.Objects.DataClasses;

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
        public void saveEncuesta2(Encuesta encuestaEdited)
        {
            /*Models.ENCUESTA updatedEncuesta = convertToENCUESTA(encuestaEdited);

            using (Models.bienestarEntities db = new Models.bienestarEntities())
            {

                Models.ENCUESTA removeEncuesta = db.ENCUESTAs.Single(e => e.CODIGO == updatedEncuesta.CODIGO);

                db.ENCUESTAs.DeleteObject(removeEncuesta);
                db.SaveChanges();

                updatedEncuesta.CODIGO = default(int);
                db.ENCUESTAs.AddObject(updatedEncuesta);
                db.SaveChanges();
                writeResponse(new JavaScriptSerializer().Serialize(updatedEncuesta));

            }*/
        }

        [WebMethod]
        public void saveEncuesta1(Encuesta encuestaEdited)
        {
            Models.ENCUESTA updatedEncuesta = convertToENCUESTA(encuestaEdited);

            using (Models.bienestarEntities db = new Models.bienestarEntities())
            {

                // get current Encuesta in db
                var encuestaInDB = db.ENCUESTAs.Single(e => e.CODIGO == updatedEncuesta.CODIGO);

                var codigosPreguntasEditadas = updatedEncuesta.ENCUESTA_PREGUNTA.Select(p => p.CODIGO).ToList();

                // all questions in database from encuesta is editing
                var preguntas = db.ENCUESTA_PREGUNTA.Where(p => p.ENCUESTA.CODIGO == updatedEncuesta.CODIGO).ToList<Models.ENCUESTA_PREGUNTA>();

                // get questions for be removed, diferent to edited
                var preguntasBorrar = encuestaInDB.ENCUESTA_PREGUNTA.Where(p => !codigosPreguntasEditadas.Contains(p.CODIGO)).ToList();
                // mark for remove each question
                preguntasBorrar.ForEach(p => db.ENCUESTA_PREGUNTA.DeleteObject(p));
                db.SaveChanges();

                // get code responses edited
                List<int> codigoRespuestasEditadas = new List<int>();
                foreach (Models.ENCUESTA_PREGUNTA ep in updatedEncuesta.ENCUESTA_PREGUNTA)
                {
                    codigoRespuestasEditadas.AddRange(ep.ENCUESTA_RESPUESTA.Select(r => r.CODIGO).ToList());
                    // mark as modificated
                    /*
                    foreach(Models.ENCUESTA_RESPUESTA er in ep.ENCUESTA_RESPUESTA){
                        db.ENCUESTA_RESPUESTA.Attach(er);
                        db.ObjectStateManager.ChangeObjectState(er, System.Data.EntityState.Modified);
                    }
                    db.ENCUESTA_PREGUNTA.Attach(ep);
                    db.ObjectStateManager.ChangeObjectState(ep, System.Data.EntityState.Modified);
                    */
                }

                // get responses for be removed
                foreach (Models.ENCUESTA_PREGUNTA ep in encuestaInDB.ENCUESTA_PREGUNTA)
                {
                    var respuestasBorrar = ep.ENCUESTA_RESPUESTA.Where(r => !codigoRespuestasEditadas.Contains(r.CODIGO)).ToList();
                    respuestasBorrar.ForEach(r => db.ENCUESTA_RESPUESTA.DeleteObject(r));
                }
                db.SaveChanges();

            }

            using (Models.bienestarEntities db = new Models.bienestarEntities())
            {
                db.ENCUESTAs.Attach(updatedEncuesta);
                db.ObjectStateManager.ChangeObjectState(updatedEncuesta, System.Data.EntityState.Modified);
                db.SaveChanges();
                writeResponse(new JavaScriptSerializer().Serialize(updatedEncuesta));
            }

            /*writeResponse("["
                + new JavaScriptSerializer().Serialize(preguntas) + ","
                + new JavaScriptSerializer().Serialize(preguntasBorrar) + ","
                + new JavaScriptSerializer().Serialize(updatedEncuesta.ENCUESTA_PREGUNTA)
                + "]");
             */
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