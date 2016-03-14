<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Roles
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>

    <script type="text/javascript" src="../../Scripts/Utils/angular-messages.js"></script>
    <script type="text/javascript" src="../../Scripts/Controllers/rols.js?nocache=<%=RandomNumber%>"></script>

    <h2>Roles</h2>

    <div id="messages"></div>

    <div ng-controller="RolsController as Main">
        <button type="button" ng-click="addNewRolDialog()">Nuevo Rol</button><br /><br />

        <div ui-grid="gridOptions"></div>

        <script type="text/ng-template" id="actionsRols.html">
              <div class="ui-grid-cell-contents">
                <button type="button" ng-click="grid.appScope.Main.removeRol(COL_FIELD)">Borrar</button>
                <button type="button" ng-click="grid.appScope.Main.editRol(COL_FIELD)">Editar</button>
              </div>
        </script>

        <script type="text/ng-template" id="editRol.html">
            <fieldset>
                <legend>Editar roles</legend>
                <form name="rolForm" ng-submit="saveEditedRol()">
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="nombre">Nombre del rol</label>  
                        <div class="col-md-4">
                            <input required ng-model="rolCopy.NOMBRE" id="nombre" name="nombre" type="text" placeholder="Nombre rol" class="form-control input-md">
                            <br/><span class="help-block">Nombre del rol</span>  
                            <span ng-messages="rolForm.nombre.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese un nombre de rol</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" >Módulos</label>
                        <table style="width:100%">
                            <tr>
                                <th></th>
                                <th>Código</th>
                                <th>Nombre</th> 
                                <th>Estado</th>
                            </tr>
  <tr>
    <td>Eve</td>
    <td>Eve</td>
    <td>Jackson</td> 
    <td>94</td>
  </tr>
</table>
                    </div>

                     <div class="form-group">
                        <label class="col-md-4 control-label" for="activeRolBox">Estado</label>
                        <div class="col-md-4">
                            <select ng-model="rolCopy.ESTADO" id="activeRolBox" name="activeRolBox" class="form-control"
                                ng-options="o.v as o.n for o in [{ n: 'Inactivo', v: false }, { n: 'Activo', v: true }]">
                            </select>
                        </div>
                    </div>

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
                            <input required ng-model="rolCopy.NOMBRE" id="nombre" name="nombre" type="text" placeholder="Nombre rol" class="form-control input-md">
                            <span ng-messages="newRolForm.nombre.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese un nombre de rol</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="activeRolBox">Estado</label>
                        <div class="col-md-4">
                            <select ng-model="rolCopy.ESTADO" id="activeRolBox" name="activeRolBox" class="form-control"
                                ng-options="o.v as o.n for o in [{ n: 'Inactivo', v: false }, { n: 'Activo', v: true }]">
                            </select>
                        </div>
                    </div>

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