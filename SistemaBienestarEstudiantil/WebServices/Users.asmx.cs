using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace SistemaBienestarEstudiantil.WebServices
{
    /// <summary>
    /// Descripción breve de WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio Web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    [System.Web.Script.Services.ScriptService]
    public class Users : System.Web.Services.WebService
    {
        [WebMethod]
        public void Login(String user, String pass)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conexionBienestar"].ConnectionString;
            SqlConnection conexion = new SqlConnection(connectionString);
            conexion.Open();
            String sql = "select nombreusuario from usuario where usuario = '" + user + "' and contrasenaactual = '" + pass + "'";
            SqlCommand comando = new SqlCommand(sql, conexion);
            SqlDataReader registro = comando.ExecuteReader();
            Boolean valueReturn = false;
            if (registro.Read())
            {
                // set values in session
                Session["nombreusuario"] = registro["nombreusuario"];
                valueReturn = true;
            }
            conexion.Close();
            Context.Response.Write(valueReturn.ToString().ToLower());
        }

        [WebMethod]
        public void allUser()
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            Context.Response.Write(new JavaScriptSerializer().Serialize(db.USUARIOs.ToList()));
            Context.Response.Flush();
            Context.Response.End();
        }
    }
}
