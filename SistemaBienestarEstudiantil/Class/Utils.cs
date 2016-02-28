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

        static public bool ValidateFirstPassword(string passwordCurrent, string passwordOld)
        {
            if (String.IsNullOrEmpty(passwordCurrent)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "passwordCurrent");
            if (String.IsNullOrEmpty(passwordOld)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "passwordOld");

            if (passwordCurrent.CompareTo(passwordOld) == 0)
                return true;

            return false;
        }

        static public bool ValidateConfirmPassword(string userPassword, string oldPassword, string newPassword, string confirmPassword)
        {
            if (String.IsNullOrEmpty(userPassword)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "userPassword");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "newPassword");
            if (String.IsNullOrEmpty(confirmPassword)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "confirmPassword");

            if (userPassword.CompareTo(oldPassword) == 0 && newPassword.CompareTo(confirmPassword) == 0)
                return true;

            return false;
        }

        static public bool ValidateOldPassword(string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "newPassword");

            if (oldPassword.CompareTo(newPassword) == 0)
                return true;

            return false;
        }
    }
}