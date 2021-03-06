﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SistemaBienestarEstudiantil.Models;
using System.Net.Mail;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace SistemaBienestarEstudiantil.Class
{
    public class Utils
    {
        public const string DOCENTE = "DOCENTE";
        public const string ADMINISTRADOR = "ADMINISTRADOR";
        public const string MODULOUSUARIO = "EDITAR USUARIOS";
        public const string MODULOROLES = "EDITAR ROLES";
        public const string MODULOBECAS = "EDITAR BECAS";
        public const string MODULOENCUESTAS = "EDITAR ENCUESTAS";
        public const string MODULOTAREAS = "EDITAR TAREAS";
        public const string USERCODE = "userCode";
        public const string BECANOTIFICACION = "becaMailNotification";
        public const int DOCUMENTMAXCANT = 3;

        //public const string APP_CONTEXT = "/BienestarEstudiantil";
        public const string APP_CONTEXT = "../..";
        
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

            if (userPassword.CompareTo(Utils.Encripta(oldPassword)) == 0 && Utils.Encripta(newPassword).CompareTo(Utils.Encripta(confirmPassword)) == 0) { return true; }

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

        static public void writeResponseObject(Object response)
        {
            System.Web.HttpContext.Current.Response.Write(new JavaScriptSerializer().Serialize(response));
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
        }

        static public void writeResponseExcel(string response, string fileName)
        {
            System.Web.HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            System.Web.HttpContext.Current.Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            System.Web.HttpContext.Current.Response.BinaryWrite(Encoding.ASCII.GetBytes(response));
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();
        }

        static public int getPeriodo(DateTime date)
        {
            Models.bienestarEntities db = new Models.bienestarEntities();
            var periodos = db.PERIODOes.Where(p => p.TPECODIGOI == 1).OrderByDescending(o => o.PRDCODIGOI).ToList();

            foreach (var periodo in periodos)
            {
                if (date.CompareTo(periodo.PRDFECINIF) >= 0)
                {
                    return periodo.PRDCODIGOI;
                }
            }

            return -1;
        }

        static public object getSession(string attribute)
        {
            HttpSessionStateBase session = new HttpSessionStateWrapper(HttpContext.Current.Session);
            return session[attribute];
        }

        static public void terminateSession()
        {
            HttpSessionStateBase session = new HttpSessionStateWrapper(HttpContext.Current.Session);
            session.Clear();
            session.Abandon();
        }

        static public bool isTeacher(int userCode)
        {
            bienestarEntities db = new bienestarEntities();
            var user = db.BE_USUARIO.Join(db.BE_ROL, u => u.CODIGOROL, r => r.CODIGO, (u, r) => new { u, r }).
                    Join(db.BE_ROL_ACCESO, rra => rra.r.CODIGO, ra => ra.CODIGOROL, (rra, ra) => new { rra, ra }).
                    Join(db.BE_ACCESO, raa => raa.ra.CODIGOACCESO, a => a.CODIGO, (raa, a) => new { raa, a }).Select(x => new
                    {
                        NOMBREACCESO = x.a.NOMBRE,
                        VALIDO = x.raa.ra.VALIDO,
                        CODIGO = x.raa.rra.u.CODIGO
                    })
                    .Where(y => y.NOMBREACCESO == Utils.MODULOTAREAS && y.VALIDO == true && y.CODIGO == userCode).FirstOrDefault();
            return user != null && isAdministator(userCode) == false;
        }

        static public bool isAdministator(int userCode)
        {
            bienestarEntities db = new bienestarEntities();
            var user = db.BE_USUARIO.Join(db.BE_ROL, u => u.CODIGOROL, r => r.CODIGO, (u, r) => new { u, r }).
                          Where(w => w.r.NOMBRE == Utils.ADMINISTRADOR && w.u.CODIGO == userCode).Select(s => s.u).FirstOrDefault();
            return user != null;
        }

        /// <summary>
        /// return true is exist user logued
        /// </summary>
        /// <returns></returns>
        static public bool isLogged()
        {

            object userCodeSession = getSession(USERCODE);
            if (userCodeSession != null)
            {
                int userCode = (int)userCodeSession;

                bienestarEntities db = new bienestarEntities();
                var user = db.BE_USUARIO.Join(db.BE_ROL, u => u.CODIGOROL, r => r.CODIGO, (u, r) => new { u, r }).
                              Where(w => w.u.CODIGO == userCode && w.u.ESTADO == true).
                              Select(s => s.u).FirstOrDefault();

                // is valid logued user
                if (user != null) return true;
            }
            return false;
        }

        /// <summary>
        /// return true if user is logued and have access to module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        static public bool haveAccessTo(string module)
        {
            object userCodeSession = getSession(USERCODE);
            if (userCodeSession != null)
            {
                int userCode = (int)userCodeSession;
                bienestarEntities db = new bienestarEntities();
                var user = db.BE_USUARIO.Join(db.BE_ROL, u => u.CODIGOROL, r => r.CODIGO, (u, r) => new { u, r }).
                              Join(db.BE_ROL_ACCESO, ur => ur.r.CODIGO, ra => ra.CODIGOROL, (ur, ra) => new { ur, ra }).
                              Join(db.BE_ACCESO, urra => urra.ra.CODIGOACCESO, a => a.CODIGO, (urra, a) => new { urra, a }).
                              Where(w => w.urra.ur.u.CODIGO == userCode && w.urra.ur.u.ESTADO == true && w.a.NOMBRE.ToUpper() == module.ToUpper()).
                              Select(s => s.urra.ur.u).FirstOrDefault();

                // is valid logued user
                if (user != null) return true;
            }
            return false;
        }

        static public string Encripta(string Cadena)
        {
            byte[] Clave = Encoding.ASCII.GetBytes("Tu Clave");
            byte[] IV = Encoding.ASCII.GetBytes("Devjoker7.37hAES");

            byte[] inputBytes = Encoding.ASCII.GetBytes(Cadena);
            byte[] encripted;
            RijndaelManaged cripto = new RijndaelManaged();
            using (MemoryStream ms = new MemoryStream(inputBytes.Length))
            {
                using (CryptoStream objCryptoStream = new CryptoStream(ms, cripto.CreateEncryptor(Clave, IV), CryptoStreamMode.Write))
                {
                    objCryptoStream.Write(inputBytes, 0, inputBytes.Length);
                    objCryptoStream.FlushFinalBlock();
                    objCryptoStream.Close();
                }
                encripted = ms.ToArray();
            }
            return Convert.ToBase64String(encripted);
        }

        static public string Desencripta(string Cadena)
        {
            byte[] Clave = Encoding.ASCII.GetBytes("Tu Clave");
            byte[] IV = Encoding.ASCII.GetBytes("Devjoker7.37hAES");

            byte[] inputBytes = Convert.FromBase64String(Cadena);
            byte[] resultBytes = new byte[inputBytes.Length];
            string textoLimpio = String.Empty;
            RijndaelManaged cripto = new RijndaelManaged();
            using (MemoryStream ms = new MemoryStream(inputBytes))
            {
                using (CryptoStream objCryptoStream = new CryptoStream(ms, cripto.CreateDecryptor(Clave, IV), CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(objCryptoStream, true))
                    {
                        textoLimpio = sr.ReadToEnd();
                    }
                }
            }
            return textoLimpio;
        }

        static public string MakeHtmlTable(string [] headers, List<string[]> rows)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<html><body><p>");
            html.Append("<table border=1 style=\"font-size:12px;color:black;\">");

            html.Append("<tr style=\"font-weight:bold;font-size:12px;\">");
            for (int i = 0; i < headers.Length; i++)
            {
                html.AppendFormat("<th bgcolor=\"LightBlue\">{0}</th>", headers[i]);
            }
            html.Append("</tr>");

            string colSpan = "";

            foreach (string [] columns in rows)
            {
                html.Append("<tr>");

                if (columns.Length == 1) colSpan = "colspan=" + headers.Length;
                else colSpan = "colspan=1";

                for (int i = 0; i < columns.Length; i++)
                {
                    html.AppendFormat("<td " + colSpan + ">{0}</td>", columns[i]);
                }
                html.Append("</tr>");
            }

            html.Append("</table>");
            html.Append("</p></body></html>");

            return html.ToString();
        }
    }
}