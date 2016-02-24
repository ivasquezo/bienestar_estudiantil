<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SistemaBienestarEstudiantil.Models.LogOnModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <% if (Session["userName"] == null) { %>Iniciar sesión
    <% } else { %> Bienvenido al Sistema Bienestar Estudiantil <% } %>
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <div ng-controller="HomeController">
        {{Message}}<br/>
        Prueba valor: {{prueba}}<br/>
        <input type="text" ng-model="prueba" />
    </div>

    <% if (@Session["userName"] == null)
       { %>
        <h2>Iniciar sesión</h2>
        <p>Especifique su nombre de usuario y contraseña.</p>
        <% using (Html.BeginForm())
       { %>
        <%: Html.ValidationSummary(true, "No se ha iniciado la sesión. Corrija los errores e inténtelo de nuevo.")%>
        <div>
            <fieldset>
                <legend>Información de cuenta</legend>

                <div class="editor-label">
                    <%: Html.LabelFor(m => m.UserName)%>
                </div>

                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.UserName)%>
                    <%: Html.ValidationMessageFor(m => m.UserName)%>
                </div>

                <div class="editor-label">
                    <%: Html.LabelFor(m => m.Password)%>
                </div>

                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.Password)%>
                    <%: Html.ValidationMessageFor(m => m.Password)%>
                </div>

                <p>
                    <input type="submit" value="Iniciar sesión" />
                </p>
            </fieldset>
        </div>
        <% }
       } else
           Response.Write("Usuario logueado: " + Session["userName"]);
       %>
</asp:Content>
