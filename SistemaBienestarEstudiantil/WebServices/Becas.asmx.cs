using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.IO;
using System.Web.Script.Services;

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
        public void addUploadedFileDataBase()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();

            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

            // get codes type document for insert
            //string[] codesTypesDocuments = System.Web.HttpContext.Current.Request.Params.Get("codesTypesDocuments").Split(',');
            string[] codesTypesDocuments = null;

            //decimal codigoSolicitud = Decimal.Parse(System.Web.HttpContext.Current.Request.Params.Get("codigoSolicitud"));
            int codigoSolicitud = 1;

            if (hfc.Count > 0 && codesTypesDocuments != null && codesTypesDocuments.Length > 0)
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
                        }
                    }
                }

                int inserted = 0;
                for (int i = 0; i < becaAdjunto.Length; i++)
                {
                    if (becaAdjunto[i].ADJUNTO.Length > 0)
                    {
                        db.BE_BECA_ADJUNTO.AddObject(becaAdjunto[i]);
                        inserted++;
                    }
                }
                
                if (inserted > 0)
                    db.SaveChanges();
            }

            writeResponseObject(new {
                System.Web.HttpContext.Current.Request.Params
            });
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void getImage(int codigoAdjunto)
        {
           Models.bienestarEntities db = new Models.bienestarEntities();

           Models.BE_BECA_ADJUNTO ba = db.BE_BECA_ADJUNTO.Where(a => a.CODIGO == codigoAdjunto).First();

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
        public void getStudentSolicitud(string cedula)
        {
            Models.bienestarEntities db = new Models.bienestarEntities();

            Models.GRADUADO alumno = db.GRADUADOS.Where(a => a.DTPCEDULAC == cedula).First();

            Models.BE_BECA_SOLICITUD beca_solicitud = null;
            var becas_solicitud = db.BE_BECA_SOLICITUD.Where(bs => bs.CEDULA == alumno.DTPCEDULAC);
            if (alumno != null && becas_solicitud.Count() > 0) beca_solicitud = becas_solicitud.First();

            writeResponseObject(new { alumno, beca_solicitud });            
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
            writeResponseObject(beca_solicitud);
        }

    }
}
