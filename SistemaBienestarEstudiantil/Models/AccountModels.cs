using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.SqlClient;

namespace SistemaBienestarEstudiantil.Models
{

    #region Modelos
    public class LogOnModel
    {
        [Required]
        [DisplayName("Nombre de usuario")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Contrase\u00F1a")]
        public string Password { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Contrase\u00F1a actual")]
        public string OldPassword { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("Nueva contrase\u00F1a")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirmar la nueva contrase\u00F1a")]
        public string ConfirmPassword { get; set; }
    }

    [PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessage = "La contrase\u00F1a y la contrase\u00F1a de confirmaci\u00F3n no coinciden.")]







    public class RegisterModel
    {
        [Required]
        [DisplayName("Nombre de usuario")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Dirección de correo electrónico")]
        public string Email { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [DisplayName("Contraseña")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirmar contraseña")]
        public string ConfirmPassword { get; set; }
    }
    #endregion

    #region Services
    // El tipo FormsAuthentication está sellado y contiene miembros estáticos, por lo que es difícil
    // realizar pruebas unitarias en el código que llama a sus miembros. La interfaz y la clase auxiliar siguientes muestran
    // cómo crear un contenedor abstracto en torno a un tipo como este para que puedan realizarse pruebas unitarias en el código de AccountController
    // .

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        BE_USUARIO ValidateUser(string userName, string password);

        void ChangePassword(int userId, string newPassword);

        List<BE_ROL_ACCESO> GetAccessRol(int rolId);

        MembershipCreateStatus CreateUser(string userName, string password, string email);

    }

    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;
        private bienestarEntities db;

        public AccountMembershipService() : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public BE_USUARIO ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) { throw new ArgumentException("El valor no puede ser NULL ni estar vac\u00EDo.", "userName"); }
            if (String.IsNullOrEmpty(password)) { throw new ArgumentException("El valor no puede ser NULL ni estar vac\u00EDo.", "password"); }

            db = new Models.bienestarEntities();
            BE_USUARIO usuario = null;

            try
            {
                usuario = db.BE_USUARIO.Single(u => u.NOMBREUSUARIO == userName && u.CONTRASENAACTUAL == password && u.ESTADO == true);
            }
            catch (InvalidOperationException e) // catch too Win32Exception (error en coneccion) System.Data.EntityException
            {
                Console.Write(e);
            }

            return usuario;
        }

        public void ChangePassword(int userId, string newPassword)
        {
            if (String.IsNullOrEmpty(userId.ToString())) { throw new ArgumentException("El valor no puede ser NULL ni estar vac\u00EDo.", "userId"); }
            if (String.IsNullOrEmpty(newPassword)) { throw new ArgumentException("El valor no puede ser NULL ni estar vac\u00EDo.", "newPassword"); }

            try
            {
                db = new bienestarEntities();

                BE_USUARIO usuario = db.BE_USUARIO.Single(u => u.CODIGO == userId);
                usuario.CONTRASENAACTUAL = newPassword;
                db.SaveChanges();
            }
            catch (InvalidOperationException e)
            {
                Console.Write(e);
            }
        }

        public List<BE_ROL_ACCESO> GetAccessRol(int rolId)
        {
            if (String.IsNullOrEmpty(rolId.ToString())) { throw new ArgumentException("El valor no puede ser NULL ni estar vac\u00EDo.", "rolId"); }

            List<BE_ROL_ACCESO> accessRol = null;

            try
            {
                db = new bienestarEntities();

                accessRol = db.BE_ROL_ACCESO.Where(u => u.CODIGOROL == rolId && u.VALIDO == true).ToList();
            }
            catch (InvalidOperationException e)
            {
                Console.Write(e);
            }

            return accessRol;
        }




        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("El valor no puede ser NULL ni estar vac\u00EDo.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("El valor no puede ser NULL ni estar vac\u00EDo.", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("El valor no puede ser NULL ni estar vac\u00EDo.", "email");

            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("El valor no puede ser NULL ni estar vacío.", "userName");

            FormsAuthentication.SetAuthCookie(userName, false);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
    #endregion

    #region Validation
    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Vaya a http://go.microsoft.com/fwlink/?LinkID=177550 para
            // obtener una lista completa de códigos de estado.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "El nombre de usuario ya existe. Escriba otro nombre de usuario.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Ya existe un nombre de usuario para esa dirección de correo electrónico. Especifique otra dirección de correo electrónico.";

                case MembershipCreateStatus.InvalidPassword:
                    return "La contraseña especificada no es válida. Escriba un valor de contraseña válido.";

                case MembershipCreateStatus.InvalidEmail:
                    return "La dirección de correo electrónico especificada no es válida. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "La respuesta de recuperación de la contraseña especificada no es válida. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "La pregunta de recuperación de la contraseña especificada no es válida. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.InvalidUserName:
                    return "El nombre de usuario especificado no es válido. Compruebe el valor e inténtelo de nuevo.";

                case MembershipCreateStatus.ProviderError:
                    return "El proveedor de autenticación devolvió un error. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema.";

                case MembershipCreateStatus.UserRejected:
                    return "La solicitud de creación de usuario se ha cancelado. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema.";

                default:
                    return "Error desconocido. Compruebe los datos especificados e inténtelo de nuevo. Si el problema continúa, póngase en contacto con el administrador del sistema.";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PropertiesMustMatchAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' y '{1}' no coinciden.";
        private readonly object _typeId = new object();

        public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty)
            : base(_defaultErrorMessage)
        {
            OriginalProperty = originalProperty;
            ConfirmProperty = confirmProperty;
        }

        public string ConfirmProperty { get; private set; }
        public string OriginalProperty { get; private set; }

        public override object TypeId
        {
            get
            {
                return _typeId;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                OriginalProperty, ConfirmProperty);
        }

        public override bool IsValid(object value)
        {
            return true;
            //PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            //object originalValue = properties.Find(OriginalProperty, true /* ignoreCase */).GetValue(value);
            //object confirmValue = properties.Find(ConfirmProperty, true /* ignoreCase */).GetValue(value);
            //return Object.Equals(originalValue, confirmValue);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "'{0}' debe tener al menos {1} caracteres.";
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }
    }
    #endregion

}
