using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SistemaBienestarEstudiantil.Models;
using System.Net.Mail;

namespace SistemaBienestarEstudiantil.Class
{
    public class Utils
    {
        /// <summary>
        /// Valida si la clave actual es igual a la anterior para solicitar cambio de clave
        /// </summary>
        /// <param name="passwordCurrent"></param>
        /// <param name="passwordOld"></param>
        static public bool ValidateFirstPassword(string passwordCurrent, string passwordOld)
        {
            if (String.IsNullOrEmpty(passwordCurrent)) { throw new ArgumentException("El valor no puede ser NULL ni estar vac\u00EDo.", "passwordCurrent"); }
            if (String.IsNullOrEmpty(passwordOld)) { throw new ArgumentException("El valor no puede ser NULL ni estar vac\u00EDo.", "passwordOld"); }

            if (passwordCurrent.CompareTo(passwordOld) == 0) { return true; }

            return false;
        }

        /// <summary>
        /// Valida cambio de contrasena
        /// </summary>
        /// <param name="userPassword"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="confirmPassword"></param>
        static public bool ValidateConfirmPassword(string userPassword, string oldPassword, string newPassword, string confirmPassword)
        {
            if (String.IsNullOrEmpty(userPassword)) { throw new ArgumentException("El valor no puede ser NULL ni estar vac\u00EDo.", "userPassword"); }
            if (String.IsNullOrEmpty(oldPassword)) { throw new ArgumentException("El valor no puede ser NULL ni estar vac\u00EDo.", "oldPassword"); }
            if (String.IsNullOrEmpty(newPassword)) { throw new ArgumentException("El valor no puede ser NULL ni estar vac\u00EDo.", "newPassword"); }
            if (String.IsNullOrEmpty(confirmPassword)) { throw new ArgumentException("El valor no puede ser NULL ni estar vac\u00EDo.", "confirmPassword"); }

            if (userPassword.CompareTo(oldPassword) == 0 && newPassword.CompareTo(confirmPassword) == 0) { return true; }

            return false;
        }

        static public Boolean ValidateAccess()
        {
            HttpSessionStateBase session = new HttpSessionStateWrapper(HttpContext.Current.Session);
            return session["userValidated"] != null && session["userValidated"].ToString() == Boolean.TrueString;
        }

        static public Boolean ValidateAccessUsuario()
        {
            HttpSessionStateBase session = new HttpSessionStateWrapper(HttpContext.Current.Session);
            return session["userValidated"] != null && session["userValidated"].ToString() == Boolean.TrueString
                && session["editarUsuario"] != null && session["editarUsuario"].ToString() == Boolean.TrueString;
        }

        static public Boolean ValidateAccessRol()
        {
            HttpSessionStateBase session = new HttpSessionStateWrapper(HttpContext.Current.Session);
            return session["userValidated"] != null && session["userValidated"].ToString() == Boolean.TrueString
                && session["editarRol"] != null && session["editarRol"].ToString() == Boolean.TrueString;
        }

        static public Boolean ValidateAccessTarea()
        {
            HttpSessionStateBase session = new HttpSessionStateWrapper(HttpContext.Current.Session);
            return session["userValidated"] != null && session["userValidated"].ToString() == Boolean.TrueString
                && session["editarTarea"] != null && session["editarTarea"].ToString() == Boolean.TrueString;
        }

        static public Boolean ValidateAccessEncuesta()
        {
            HttpSessionStateBase session = new HttpSessionStateWrapper(HttpContext.Current.Session);
            return session["userValidated"] != null && session["userValidated"].ToString() == Boolean.TrueString
                && session["editarEncuesta"] != null && session["editarEncuesta"].ToString() == Boolean.TrueString;
        }

        static public Boolean ValidateAccessBeca()
        {
            HttpSessionStateBase session = new HttpSessionStateWrapper(HttpContext.Current.Session);
            return session["userValidated"] != null && session["userValidated"].ToString() == Boolean.TrueString
                && session["editarBeca"] != null && session["editarBeca"].ToString() == Boolean.TrueString;
        }

        static public Boolean validateFirstPasswordAccess()
        {
            HttpSessionStateBase session = new HttpSessionStateWrapper(HttpContext.Current.Session);
            return session["firstPasswordAccess"] != null && session["firstPasswordAccess"].ToString() == Boolean.TrueString;
        }

        static public void sendMail(string to, string subject, string body)
        {
            // consulting data, credentials from database
            Models.bienestarEntities db = new Models.bienestarEntities();
            string mailCredentials = db.BE_DATOS_SISTEMA.Where(d => d.NOMBRE == "userMailCredentials").Select(d => d.VALOR).First();
            string passwordCredentials = db.BE_DATOS_SISTEMA.Where(d => d.NOMBRE == "passwordCredentials").Select(d => d.VALOR).First();
            string server = db.BE_DATOS_SISTEMA.Where(d => d.NOMBRE == "smtpServer").Select(d => d.VALOR).First();
            int port = Int32.Parse(db.BE_DATOS_SISTEMA.Where(d => d.NOMBRE == "smtpPort").Select(d => d.VALOR).First());

            SmtpClient smtpClient = new SmtpClient(server, port);

            smtpClient.Credentials = new System.Net.NetworkCredential(mailCredentials, passwordCredentials);
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();

            mail.Subject = subject;
            mail.Body = body;
            mail.From = new MailAddress(mailCredentials);
            mail.To.Add(new MailAddress(to));

            smtpClient.Send(mail);
        }

        static public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}