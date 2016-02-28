<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Acerca de nosotros
</asp:Content>
<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <div ng-controller="UsuarioController as Main">
        <div ui-grid="gridOptions"></div>
    </div>
    <script type="text/ng-template" id="actionsUsers.html">
          <div class="ui-grid-cell-contents">
            <button class="btn" type="button" ng-click="grid.appScope.Main.removeUser(COL_FIELD)">Borrar</button>
            <button class="btn" type="button" ng-click="grid.appScope.Main.editUser(COL_FIELD)">Editar</button>
          </div>
    </script>
    <p>
        <input type="submit" value="Iniciar sesión" style="float:right" />
    </p>
    <h2>
        Usuarios</h2>
    <p>
        Incluya aquí el contenido.
    </p>
</asp:Content>
