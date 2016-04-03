<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SistemaBienestarEstudiantil.Models.ChangePasswordModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Cambiar contrase&ntilde;a
</asp:Content>

<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Cambiar contrase&ntilde;a</h2>

    <p>Use el siguiente formulario para cambiar la contrase&ntilde;a.</p>

    <p>Las nuevas contrase&ntilde;as deben tener una longitud de <%: ViewData["PasswordLength"] %> caracteres.</p>

    <% using (Html.BeginForm()) { %>
    <%: Html.ValidationSummary(true, "No se realiz\u00F3 el cambio de contrase\u00F1a. Corrija los errores e int\u00E9ntelo de nuevo.")%>
    <div>
        <fieldset>
            <legend>Informaci&oacute;n de cuenta</legend>
            <div class="editor-label">
                <%: Html.LabelFor(m => m.OldPassword) %>
            </div>
            <div class="editor-field">
                <%: Html.PasswordFor(m => m.OldPassword) %>
                <%: Html.ValidationMessageFor(m => m.OldPassword) %>
            </div>
            <div class="editor-label">
                <%: Html.LabelFor(m => m.NewPassword) %>
            </div>
            <div class="editor-field">
                <%: Html.PasswordFor(m => m.NewPassword) %>
                <%: Html.ValidationMessageFor(m => m.NewPassword) %>
            </div>
            <div class="editor-label">
                <%: Html.LabelFor(m => m.ConfirmPassword) %>
            </div>
            <div class="editor-field">
                <%: Html.PasswordFor(m => m.ConfirmPassword) %>
                <%: Html.ValidationMessageFor(m => m.ConfirmPassword) %>
            </div>
            <p>
                <input type="submit" value="Cambiar contrase&ntilde;a" />
            </p>
        </fieldset>
    </div>
    <% } %>
</asp:Content>