<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SistemaBienestarEstudiantil.Models.LogOnModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <% if (@Session["userName"] == null) { %>
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
    <% } } else { %>

    <div>
        <fieldset>
            <legend>Información de cuenta</legend>

            <p>Ya se encuentra iniciada sesión con la cuenta <b><% =@Session["userName"] %></b></p> 
        </fieldset>
    </div>
    <% } %>
</asp:Content>
