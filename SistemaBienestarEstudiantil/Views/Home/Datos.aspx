<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Datos generales
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>

    <script type="text/javascript" src="../../Scripts/Utils/angular-messages.js"></script>
    <script type="text/javascript" src="../../Scripts/Controllers/rols.js?nocache=<%=RandomNumber%>"></script>

    <h2>Datos generales</h2>

    <div id="messages"></div>

    <div ng-controller="DatosController">
    <div ng-app="components">
 <tabs>
  <pane title="Pestaña 1">
   Aquí va el contenido de la primera pestaña.
  </pane>
  <pane title="Pestaña 2">
   Este es el contenido de la pestaña número dos.
  </pane>
 </tabs>
</div>
    </div>
</asp:Content>

