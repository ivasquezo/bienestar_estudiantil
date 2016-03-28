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
            writeResponseObject(db.BECA_TIPO.ToList());
        }

        [WebMethod]
        public void saveFileTest()
        {
            int iUploadedCnt = 0;
            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
            byte[] fileBytes = null;
            // CHECK THE FILE COUNT.
            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                System.Web.HttpPostedFile hpf = hfc[iCnt];
                string sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");

                if (hpf.ContentLength > 0)
                {
                    // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                    if (!File.Exists(sPath + Path.GetFileName(hpf.FileName)))
                    {
                        // SAVE THE FILES IN THE FOLDER.
                        using (var memoryStream = new MemoryStream())
                        {
                             hpf.InputStream.CopyTo(memoryStream);
                             fileBytes = memoryStream.ToArray();
                        }
                        iUploadedCnt = iUploadedCnt + 1;
                    }
                }
            }

            Models.bienestarEntities db = new Models.bienestarEntities();
            Models.BECA_ADJUNTO ba = new Models.BECA_ADJUNTO();

            ba.CODIGOSOLICITUD = 1;
            ba.CONTENTTYPE = hfc[0].ContentType;
            ba.ADJUNTO = fileBytes;
            ba.NOMBRE = hfc[0].FileName;

            db.BECA_ADJUNTO.AddObject(ba);
            db.SaveChanges();
            writeResponse("ok");
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = true)]
        public string getImage()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();

            Models.BECA_ADJUNTO ba = db.BECA_ADJUNTO.First();

            byte[] response = ba.ADJUNTO;

            Context.Response.ContentType = ba.CONTENTTYPE;
            Context.Response.BinaryWrite(response);
            Context.Response.Flush();
            Context.Response.End();
            return "as";
        }
    }
}
