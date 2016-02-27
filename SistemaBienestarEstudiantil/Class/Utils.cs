using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaBienestarEstudiantil.Class
{
    public class Utils
    {
        static public Boolean validateFirstPasswordAccess()
        {
            HttpSessionStateBase session = new HttpSessionStateWrapper(HttpContext.Current.Session);
            return session["firstPasswordAccess"] != null && session["firstPasswordAccess"].ToString() == Boolean.TrueString;
        }

        static public Boolean validateAccess()
        {
            HttpSessionStateBase session = new HttpSessionStateWrapper(HttpContext.Current.Session);
            return session["usuarioValidado"] != null && session["usuarioValidado"].ToString() == Boolean.TrueString;
        }
    }
}