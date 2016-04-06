using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.IO;
using System.Web.Script.Services;
using System.Net.Mail;
using SistemaBienestarEstudiantil.Class;
using SistemaBienestarEstudiantil.Models;

namespace SistemaBienestarEstudiantil.WebServices
{
    /// <summary>
    /// Descripción breve de Becas
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio Web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class Becas : System.Web.Services.WebService
    {

        private void writeResponse(String response)
        {
            Context.Response.Write(response);
            Context.Response.Flush();
            Context.Response.End();
        }

        private void writeResponseObject(Object response)
        {
            Context.Response.Write(new JavaScriptSerializer().Serialize(response));
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void getTipos()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            writeResponseObject(db.BE_BECA_TIPO.ToList());
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void getBecas()
        {
            Response response = new Response(true, "", "", "", null);
            bienestarEntities db = new bienestarEntities();

            try
            {
                var becas = db.BE_BECA_SOLICITUD.Select(be => new { be.CODIGO, be.APROBADA, be.CEDULA, BECA = be.BE_BECA_TIPO.NOMBRE, NOMBRE = be.DATOSPERSONALE.DTPNOMBREC + be.DATOSPERSONALE.DTPAPELLIC + be.DATOSPERSONALE.DTPAPELLIC2 }).ToList();

                if (becas != null && becas.Count > 0)
                    response = new Response(true, "", "", "", becas);
                else
                    response = new Response(false, "info", "Informaci\u00F3n", "No se han encontrado usuarios registrados", null);
            }
            catch (Exception e)
            {
                response = new Response(false, "error", "Error", "Error al obtener los usuarios", e);
                writeResponseObject(response);
            }

            writeResponseObject(response);
        }

        [WebMethod]
        public void addUploadedFileDataBase()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();

            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

            int codigoSolicitud = Int32.Parse(System.Web.HttpContext.Current.Request.Params.Get("codigoSolicitud"));

            string descripcion = System.Web.HttpContext.Current.Request.Params.Get("descripcion");

            if (hfc.Count > 0 && codigoSolicitud != 0)
            {
                Models.BE_BECA_ADJUNTO[] becaAdjunto = new Models.BE_BECA_ADJUNTO[hfc.Count];
                // CHECK THE FILE COUNT.
                for (int i = 0; i < hfc.Count; i++)
                {
                    System.Web.HttpPostedFile hpf = hfc[i];
                    if (hpf.ContentLength > 0)
                    {
                        // SAVE THE FILES IN THE FOLDER.
                        using (var memoryStream = new MemoryStream())
                        {
                            hpf.InputStream.CopyTo(memoryStream);
                            byte[] fileBytes = memoryStream.ToArray();

                            becaAdjunto[i] = new Models.BE_BECA_ADJUNTO();
                            becaAdjunto[i].CODIGOSOLICITUD = codigoSolicitud;
                            becaAdjunto[i].CONTENTTYPE = hfc[i].ContentType;
                            becaAdjunto[i].ADJUNTO = fileBytes;
                            becaAdjunto[i].NOMBRE = hfc[i].FileName;
                            becaAdjunto[i].DESCRIPCION = hfc.AllKeys[i] == "documentoSolicitud" ? "Solicitud personal dirigida al Coordinador del Departamento de Bienestar Universitario" : descripcion;
                            becaAdjunto[i].DOCUMENTOSOLICITUD = hfc.AllKeys[i] == "documentoSolicitud";
                        }
                    }
                }

                int inserted = 0;
                for (int i = 0; i < becaAdjunto.Length; i++)
                {
                    if (becaAdjunto[i] != null && becaAdjunto[i].ADJUNTO != null && becaAdjunto[i].ADJUNTO.Length > 0)
                    {
                        db.BE_BECA_ADJUNTO.AddObject(becaAdjunto[i]);
                        inserted++;
                    }
                }
                
                if (inserted > 0)
                    db.SaveChanges();
            }

            var beca_solicitud = db.BE_BECA_SOLICITUD.Single(bs => bs.CODIGO == codigoSolicitud);

            writeResponseObject(new {
                beca_solicitud, 
                System.Web.HttpContext.Current.Request.Params
            });
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void getAttach(int code)
        {
            Models.bienestarEntities db = new Models.bienestarEntities();

            Models.BE_BECA_ADJUNTO ba = db.BE_BECA_ADJUNTO.Where(a => a.CODIGO == code).First();

            byte[] response = null;

            if (ba != null && ba.ADJUNTO != null)
                response = ba.ADJUNTO;

            Context.Response.ContentType = ba.CONTENTTYPE;
            Context.Response.AddHeader("content-disposition", "attachment; filename=" + ba.NOMBRE);
            Context.Response.BinaryWrite(response);
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void getListAttach()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            writeResponseObject(db.BE_BECA_ADJUNTO.Select(a => new { a.CODIGO, a.DOCUMENTOSOLICITUD }).ToList());
        }

        [WebMethod]
        public void removeAttach(int attachCode)
        {
            Models.bienestarEntities db = new Models.bienestarEntities();

            Models.BE_BECA_ADJUNTO ba = db.BE_BECA_ADJUNTO.Where(a => a.CODIGO == attachCode).First();
            if (ba != null)
            {
                db.BE_BECA_ADJUNTO.DeleteObject(ba);
                db.SaveChanges();
            }
            writeResponse("ok");
        }

        [WebMethod]
        public void removeBeca(int codeBeca)
        {
            Models.bienestarEntities db = new Models.bienestarEntities();

            Models.BE_BECA_SOLICITUD ba = db.BE_BECA_SOLICITUD.Where(b => b.CODIGO == codeBeca).First();
            if (ba != null)
            {
                db.BE_BECA_SOLICITUD.DeleteObject(ba);
                db.SaveChanges();
            }
            writeResponse("ok");
        }

        [WebMethod]
        public void getStudentSolicitud(string cedula)
        {
            Models.bienestarEntities db = new Models.bienestarEntities();

            /*
             * restringir estudiantes que pueden pedir la beca
             */

            /* listar codigos periodos donde buscar */
            List<int> periodos = db.PERIODOes.Where(p => p.TPECODIGOI == 1 && (p.PRDHABILMAT == "1" || p.PRDESTADOC == "1")).Select(p => p.PRDCODIGOI).ToList();

            /* listar codigos matriculas donde buscar */
            List<long> matriculas = db.MATRICULAs.Where(m => periodos.Contains(m.PRDCODIGOI) && m.NVLCODIGOI > 0).Select(m => m.INSCODIGOI).ToList();
            
            /* listar cedulas de inscripciones donde buscar */
            List<DATOSPERSONALE> datosPersonalesInscripciones = db.INSCRIPCIONs.Where(i => matriculas.Contains(i.INSCODIGOI) && i.DTPCEDULAC == cedula).Select(m => m.DATOSPERSONALE).ToList();
            
            DATOSPERSONALE alumno = null;
            if (datosPersonalesInscripciones.Count > 0)
            {
                alumno = datosPersonalesInscripciones.First();
            }
            
            Models.BE_BECA_SOLICITUD beca_solicitud = null;
            var becas_solicitud = db.BE_BECA_SOLICITUD.Where(bs => bs.CEDULA == alumno.DTPCEDULAC);
            if (alumno != null && becas_solicitud.Count() > 0) beca_solicitud = becas_solicitud.First();

            writeResponseObject(new { alumno, beca_solicitud, datosPersonalesInscripciones.Count });
        }

        [WebMethod]
        public void getBecaSolicitud(int CODIGO)
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            
            Models.BE_BECA_SOLICITUD beca_solicitud = null;
            var becas_solicitud = db.BE_BECA_SOLICITUD.Where(bs => bs.CODIGO == CODIGO);
            if (becas_solicitud.Count() > 0) beca_solicitud = becas_solicitud.First();

            writeResponseObject(beca_solicitud);
        }

        /**
         * pendiente 0
         * aprobada 1
         * rechazada 2
         */
        [WebMethod]
        public void saveBecaSolicitud(Models.BE_BECA_SOLICITUD beca_solicitud)
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            Models.BE_BECA_SOLICITUD editBS = null;

            if (beca_solicitud.CODIGO == 0)
            {
                db.BE_BECA_SOLICITUD.AddObject(beca_solicitud);
            }
            else
            {
                editBS = db.BE_BECA_SOLICITUD.Where(bs => bs.CODIGO == beca_solicitud.CODIGO).First();
                editBS.CODIGOTIPO = beca_solicitud.CODIGOTIPO;
                editBS.APROBADA = beca_solicitud.APROBADA;
            }
            
            db.SaveChanges();

            if (editBS == null)
            {
                string becaMailNotification = db.BE_DATOS_SISTEMA.Where(d => d.NOMBRE == "becaMailNotification").Select(d => d.VALOR).First();
                string body = "Se le notifica que ha sido ingresada una nueva solicitud de beca para el estudiante " + beca_solicitud.DATOSPERSONALE.DTPNOMBREC + beca_solicitud.DATOSPERSONALE.DTPAPELLIC + " con CI: " + beca_solicitud.CEDULA;
                Class.Utils.sendMail(becaMailNotification, "Nueva solicitud de beca ingresada", body);
            }

            writeResponseObject(editBS == null ? beca_solicitud : editBS);
        }

    }
}
