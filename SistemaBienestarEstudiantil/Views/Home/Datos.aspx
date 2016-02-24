<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Acerca de nosotros
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <div ng-controller="DatosController">
    {{Message}}
    </div>
    <h2>Datos</h2>
    <p>
        Usuario logueado: <%=@Session["userName"]%>
    </p>
</asp:Content>

