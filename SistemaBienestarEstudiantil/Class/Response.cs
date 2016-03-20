using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaBienestarEstudiantil.Class
{
    public class Response
    {
        public Boolean success { get; set; }
        public String severity { get; set; }
        public String summary { get; set; }
        public String message { get; set; }        
        public Object response { get; set; }

        public Response(Boolean success, String severity, String summary, String message, Object response)
        {
            this.success = success;
            this.severity = severity;
            this.summary = summary;
            this.message = message;
            this.response = response;
        }
    }
}