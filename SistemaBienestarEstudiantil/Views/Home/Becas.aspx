<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Becas
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>
    <script type="text/javascript" src="../../Scripts/Controllers/becas.js?nocache=<%=RandomNumber%>"></script>

	<div id="messages"></div>

    <h2>Becas</h2>
    <a href="/Home/BecaSolicitud" style="vertical-align:middle;font-size:18px;">Solicitar BECA</a>
    <br/>
    <div ng-controller="BecasController as Main">
    	<div cg-busy="{promise:promise,message:message,backdrop:backdrop,delay:delay,minDuration:minDuration}"></div>
	    <div ui-grid="gridOptions"></div>

    </div>

</asp:Content>
