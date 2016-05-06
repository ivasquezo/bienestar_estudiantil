﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SistemaBienestarEstudiantil.Models.LogOnModel>" %>
<%@ Import Namespace="SistemaBienestarEstudiantil.Class" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Iniciar sesi&oacute;n
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <% if (@Session["userName"] == null) { %>
    <h2>Iniciar sesi&oacute;n</h2>

    <% using (Html.BeginForm()) { %>
    <%: Html.ValidationSummary(true, "No se ha iniciado la sesi\u00F3n. Corrija los errores e int\u00E9ntelo de nuevo.")%>
    <div style="min-width:100%">
        <div style="float:left;">
            Especifique su nombre de usuario y contrase&ntilde;a.
        </div>
        <div style="float:right;width:150px;background:#5C87B2;text-align: center;padding:6px;margin-left:3px;">
            <a href="<%=Utils.APP_CONTEXT%>/Home/BecaSolicitud" target="_blank" style="text-decoration: initial;font-weight:bold;">
                <div style="color:white;">
                    Solicitud de becas
                </div>
            </a>
        </div>
        <div style="float:right;width:190px;background:#5C87B2;text-align:center;padding:6px;margin-left:3px;">
            <a href="<%=Utils.APP_CONTEXT%>/Home/Encuesta" target="_blank" style="text-decoration: initial;font-weight:bold;">
                <div style="color:white;">
                    Seguimiento graduados
                </div>
            </a>
        </div>
    </div>
    <br/>
    <div style="position:relative;min-width:100%;">
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