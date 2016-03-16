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

            ROL rol = db.ROLs.Single(r => r.CODIGO == id);

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

            ROL rol = db.ROLs.Single(r => r.CODIGO == rolId);

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

            var data = db.ACCESOes.Join(db.ROL_ACCESO, a => a.CODIGO, ra => ra.CODIGOACCESO,
                       (a, ra) => new { ACCESO = a, ROL_ACCESO = ra })
                       .Select(x => new { x.ACCESO.NOMBRE, x.ROL_ACCESO.ACCESO.CODIGO, x.ROL_ACCESO.CODIGOROL, x.ROL_ACCESO.VALIDO })
                       .Where(y => y.CODIGOROL == rolId).ToList();

            writeResponse(new JavaScriptSerializer().Serialize(data));
        }

        [WebMethod]
        public void getAccessById(int accessId)
        {
            bienestarEntities db = new bienestarEntities();

            writeResponse(new JavaScriptSerializer().Serialize(db.ACCESOes.Single(a => a.CODIGO == accessId)));
        }
    }
}
