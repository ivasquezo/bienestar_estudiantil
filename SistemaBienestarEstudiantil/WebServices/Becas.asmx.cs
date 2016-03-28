﻿using System;
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
            writeResponseObject(db.BECA_TIPO.ToList());
        }

        [WebMethod]
        public void addUploadedFileDataBase()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();

            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

            if (hfc.Count > 0)
            {
                Models.BECA_ADJUNTO[] becaAdjunto = new Models.BECA_ADJUNTO[hfc.Count];
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

                            becaAdjunto[i] = new Models.BECA_ADJUNTO();
                            becaAdjunto[i].CODIGOSOLICITUD = 1;
                            becaAdjunto[i].CONTENTTYPE = hfc[0].ContentType;
                            becaAdjunto[i].ADJUNTO = fileBytes;
                            becaAdjunto[i].NOMBRE = hfc[0].FileName;
                        }
                    }
                }

                int inserted = 0;
                for (int i = 0; i < becaAdjunto.Length; i++)
                {
                    if (becaAdjunto[i].ADJUNTO.Length > 0)
                    {
                        db.BECA_ADJUNTO.AddObject(becaAdjunto[i]);
                        inserted++;
                    }
                }
                
                if (inserted > 0)
                    db.SaveChanges();
            }

            writeResponse(System.Web.HttpContext.Current.Request.Params.Get("cedulaSolicitud"));
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public void getImage(int codigoAdjunto)
        {
           Models.bienestarEntities db = new Models.bienestarEntities();

            Models.BECA_ADJUNTO ba = db.BECA_ADJUNTO.Where(a => a.CODIGO == codigoAdjunto).First();

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
        public void getListCodeAttach()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            writeResponseObject(db.BECA_ADJUNTO.Select(a => a.CODIGO).ToList());
        }

        [WebMethod]
        public void removeAttach(int attachCode)
        {
            Models.bienestarEntities db = new Models.bienestarEntities();

            Models.BECA_ADJUNTO ba = db.BECA_ADJUNTO.Where(a => a.CODIGO == attachCode).First();
            if (ba != null)
            {
                db.BECA_ADJUNTO.DeleteObject(ba);
                db.SaveChanges();
            }
            writeResponse("ok");
        }


    }
}
