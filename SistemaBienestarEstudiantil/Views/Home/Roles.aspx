<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="rolTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Roles
</asp:Content>

<asp:Content ID="rolContent" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>

    <script type="text/javascript" src="../../Scripts/Utils/angular-messages.js"></script>
    <script type="text/javascript" src="../../Scripts/Controllers/rols.js?nocache=<%=RandomNumber%>"></script>

    <h2>Roles</h2>

    <div id="messages"></div>

    <div ng-controller="RolsController as Main">
        <div style="position:fixed;top:0px;left:50%;margin-left:-85px;">
            <div cg-busy="{promise:promise,message:message,backdrop:backdrop,delay:delay,minDuration:minDuration}"></div>
        </div>

        <button ng-click="addNewRolDialog()" style="margin-bottom:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button">
            <span class="ui-button-icon-primary ui-icon ui-icon-circle-plus"></span><span class="ui-button-text">Nuevo</span>
        </button>

        <div ui-grid="gridOptions"></div>

        <script type="text/ng-template" id="actionsRols.html">
            <div class="ui-grid-cell-contents">
                <button type="button" ng-click="grid.appScope.Main.removeRol(COL_FIELD)" ng-hide="grid.appScope.Main.getRolStatus(COL_FIELD)" title="Eliminar rol"><span class="ui-icon ui-icon-trash"></span></button>
                <button type="button" ng-click="grid.appScope.Main.editRol(COL_FIELD)" title="Editar rol"><span class="ui-icon ui-icon-pencil"></span></button>
            </div>
        </script>

        <script type="text/ng-template" id="editRol.html">
            <fieldset>
                <legend>Editar roles</legend>
                <form name="rolForm" ng-submit="saveEditedRol()">
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="nombre">Nombre del rol</label>  
                        <div class="col-md-4">
                            <input valid-rol-name required ng-model="rolCopy.NOMBRE" id="nombre" name="nombre" type="text" placeholder="Nombre rol" class="form-control input-md" style="text-transform:uppercase;" ng-disabled="rolCopy.ACTIVO">
                            <br/><span class="help-block">Nombre del rol</span>  
                            <span ng-messages="rolForm.nombre.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese un nombre de rol</span>
                                <span ng-message="rolNameExist" class="help-block ng-message">Existe un rol con este nombre</span>
                                <span ng-message="rolNameValidator" class="help-block ng-message">Debe ingresar un nombre de rol v&aacute;lido</span>
                                <span ng-message="rolNameChecking" class="help-block ng-message">Chequeando la base de datos...</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" >Permisos</label>
                        <table style="width:100%; font-size:16px">
                            <tr>
                                <th></th>
                                <th>Nombre</th> 
                            </tr>
                            <tr ng-repeat="access in allAccess">
                                <td><input type="checkbox" ng-checked="existAccess(access.CODIGO)" ng-click="setAccessRol(access.CODIGO)"></td>
                                <td>{{ access.NOMBRE }}</td>
                            </tr>
                        </table>
                    </div>

                    <br/>
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="saveRol"></label>
                        <div class="col-md-8">
                            <button type="submit" id="saveRol" name="saveRol" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>

        <script type="text/ng-template" id="newRol.html">
            <fieldset>
                <legend>Nuevo rol</legend>
                <form name="newRolForm" ng-submit="addNewRolDB()">
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="nombre">Nombre de rol</label>  
                        <div class="col-md-4">
                            <input valid-rol-name required ng-model="rolCopy.NOMBRE" id="nombre" name="nombre" type="text" placeholder="Nombre rol" class="form-control input-md" style="text-transform:uppercase;">
                            <br/><span class="help-block">Nombre del rol</span>
                            <span ng-messages="newRolForm.nombre.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese un nombre de rol</span>
                                <span ng-message="rolNameExist" class="help-block ng-message">Existe un rol con este nombre</span>
                                <span ng-message="rolNameValidator" class="help-block ng-message">Debe ingresar un nombre de rol v&aacute;lido</span>
                                <span ng-message="rolNameChecking" class="help-block ng-message">Chequeando la base de datos...</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" >Permisos</label>
                        <table style="width:100%; font-size:16px">
                            <tr>
                                <th></th>
                                <th>Nombre</th> 
                            </tr>
                            <tr ng-repeat="access in allAccess">
                                <td><input type="checkbox" ng-click="setAccessRol(access.CODIGO)"></td>
                                <td>{{ access.NOMBRE }}</td>
                            </tr>
                        </table>
                    </div>

                    <br/>
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="saveRol"></label>
                        <div class="col-md-8">
                            <button type="submit" id="saveRol" name="saveRol" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>
    </div>
</asp:Content>