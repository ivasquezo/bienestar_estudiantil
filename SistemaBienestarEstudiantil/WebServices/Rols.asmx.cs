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

            removeRolAccesByRolId(id);

            db.ROLs.DeleteObject(rol);
            db.SaveChanges();

            Context.Response.Write("ok");
            Context.Response.Flush();
            Context.Response.End();
        }

        [WebMethod]
        private void removeRolAccesByRolId(int id)
        {
            bienestarEntities db = new bienestarEntities();

            List<ROL_ACCESO> rolAccess = db.ROL_ACCESO.Where(ra => ra.CODIGOROL == id).ToList();

            foreach (ROL_ACCESO rol in rolAccess)
            {
                db.ROL_ACCESO.DeleteObject(rol);
            }
           
            db.SaveChanges();
        }

        [WebMethod]
        public void saveRolData(int rolId, String rolName, int[] accessRols)
        {
            bienestarEntities db = new bienestarEntities();

            ROL rol = db.ROLs.Single(r => r.CODIGO == rolId);

            rol.NOMBRE = rolName;

            for (int accRol=0; accRol < accessRols.Length; accRol++) {
                ROL_ACCESO rolAccess = getStatusRolAccesById(rolId, accessRols[accRol]);
                if (rolAccess != null)
                    updateRolAccess(rolId, accessRols[accRol]);
                else
                    createRolAccess(rolId, accessRols[accRol]);
            }

            db.SaveChanges();

            writeResponse("ok");
        }

        [WebMethod]
        public void addNewRol(String rolName, int[] accessRols)
        {
            bienestarEntities db = new bienestarEntities();

            ROL newRol = new ROL();

            newRol.NOMBRE = rolName;

            db.ROLs.AddObject(newRol);
            db.SaveChanges();

            for (int accRol = 0; accRol < accessRols.Length; accRol++)
                createRolAccess(Decimal.ToInt32(newRol.CODIGO), accessRols[accRol]);

            writeResponse(new JavaScriptSerializer().Serialize(newRol));
        }

        [WebMethod]
        public void getAllAccess()
        {
            bienestarEntities db = new bienestarEntities();

            writeResponse(new JavaScriptSerializer().Serialize(db.ACCESOes.ToList()));
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
        private ROL_ACCESO getStatusRolAccesById(int idRol, int idAccess)
        {
            bienestarEntities db = new bienestarEntities();

            ROL_ACCESO rolAccess = null;

            try
            {
                rolAccess = db.ROL_ACCESO.Single(ra => ra.CODIGOROL == idRol && ra.CODIGOACCESO == idAccess);
            }
            catch (System.InvalidOperationException)
            {
                Console.WriteLine("No se encontró un elemento con los filtros ingresados");
            }

            return rolAccess;
        }

        [WebMethod]
        private void updateRolAccess(int idRol, int idAccess)
        {
            bienestarEntities db = new bienestarEntities();

            ROL_ACCESO rolAccess = db.ROL_ACCESO.Single(ra => ra.CODIGOROL == idRol && ra.CODIGOACCESO == idAccess);

            if (rolAccess.VALIDO)
                rolAccess.VALIDO = false;
            else
                rolAccess.VALIDO = true;
            
            db.SaveChanges();
        }

        [WebMethod]
        private void createRolAccess(int rolId, int accessId)
        {
            bienestarEntities db = new bienestarEntities();

            ROL_ACCESO newRolAccess = new ROL_ACCESO();

            newRolAccess.CODIGOROL = rolId;
            newRolAccess.CODIGOACCESO = accessId;
            newRolAccess.VALIDO = true;

            db.ROL_ACCESO.AddObject(newRolAccess);

            db.SaveChanges();
        }

    }
}
