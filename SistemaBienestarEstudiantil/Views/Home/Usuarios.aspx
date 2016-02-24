<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Acerca de nosotros
</asp:Content>
<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <%--<div ng-controller="UsuarioController">
        {{Message}}
    </div>--%>
    <p>
        <input type="submit" value="Iniciar sesión" style="float:right" />
    </p>
    <h2>
        Usuarios</h2>
    <p>
        Incluya aquí el contenido.
    </p>
</asp:Content>
