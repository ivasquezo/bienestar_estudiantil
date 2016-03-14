using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data.Objects.DataClasses;
using System.Web.Script.Serialization;
using SistemaBienestarEstudiantil.Models;

namespace SistemaBienestarEstudiantil.WebServices
{
    /// <summary>
    /// Descripción breve de Rols
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio Web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class Rols : System.Web.Services.WebService
    {
        private void writeResponse(String response)
        {
            Context.Response.Write(response);
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void getAllRols()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            writeResponse(new JavaScriptSerializer().Serialize(db.ROLs.ToList()));
        }

        [WebMethod]
        public void removeRolById(int id)
        {
            bienestarEntities db = new bienestarEntities();

            ROL rol = db.ROLs.Single(u => u.CODIGO == id);

            db.ROLs.DeleteObject(rol);
            db.SaveChanges();

            Context.Response.Write("ok");
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        public void saveRolData(int rolId, String rolName)
        {
            bienestarEntities db = new bienestarEntities();

            ROL rol = db.ROLs.Single(u => u.CODIGO == rolId);

            rol.NOMBRE = rolName;

            db.SaveChanges();

            writeResponse("ok");
        }

        [WebMethod]
        public void addNewRol(String rolName)
        {
            bienestarEntities db = new bienestarEntities();

            ROL newRol = new ROL();

            newRol.NOMBRE = rolName;

            db.ROLs.AddObject(newRol);
            db.SaveChanges();

            writeResponse(new JavaScriptSerializer().Serialize(newRol));
        }

        [WebMethod]
        public void getAccessByRolId(int rolId)
        {
            bienestarEntities db = new bienestarEntities();

            ROL rol = db.ROLs.Single(r => r.CODIGO == rolId);

            var access = rol.ROL_ACCESO.Select(ra => ra.CODIGOACCESO).ToList();

            writeResponse(new JavaScriptSerializer().Serialize(db.ACCESOes.Where(a => access.Contains(a.CODIGO))));
        }
    }
}
