<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Acerca de nosotros
</asp:Content>
<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <div ng-controller="UsuarioController">
        <table>
            <tr>
                <td>Código</td>
                <td>Nombre</td>
                <td>Estado</td>
            </tr>
            <tr ng-repeat="user in users">
                <td>{{user.CODIGOUSUARIO}}</td>
                <td>{{user.NOMBREUSUARIO}}</td>
                <td>{{user.ESTADOUSUARIO}}</td>
            </tr>
        </table>
        MICHEL PRUEBA<BR/>   
    </div>
    <p>
        <input type="submit" value="Iniciar sesión" style="float:right" />
    </p>
    <h2>
        Usuarios</h2>
    <p>
        Incluya aquí el contenido.
    </p>
</asp:Content>
