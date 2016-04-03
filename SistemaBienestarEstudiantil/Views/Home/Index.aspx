<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SistemaBienestarEstudiantil.Models.LogOnModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Iniciar sesi&oacute;n
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <% if (@Session["userName"] == null) { %>
    <h2>Iniciar sesi&oacute;n</h2>

    <p>Especifique su nombre de usuario y contrase&ntilde;a.</p>

    <% using (Html.BeginForm()) { %>
    <%: Html.ValidationSummary(true, "No se ha iniciado la sesi\u00F3n. Corrija los errores e int\u00E9ntelo de nuevo.")%>
    <div>
        <fieldset>
            <legend>Informaci&oacute;n de cuenta</legend>
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
                <input type="submit" value="Iniciar sesi&oacute;n" />
            </p>
        </fieldset>
    </div>
    <% } } else { %>
    <div>
        <fieldset>
            <legend>Informaci&oacute;n de cuenta</legend>
            <p>Ya se encuentra iniciada sesi&oacute;n con la cuenta <b><% =@Session["userName"] %></b></p>
        </fieldset>
    </div>
    <% } %>
</asp:Content>