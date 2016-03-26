<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="activityTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Actividades
</asp:Content>

<asp:Content ID="activityContent" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>

    <script type="text/javascript" src="../../Scripts/Utils/angular-messages.js"></script>
    <script type="text/javascript" src="../../Scripts/Controllers/activities.js?nocache=<%=RandomNumber%>"></script>

    <h2>Actividades</h2>

    <div id="messages"></div>

    <div ng-controller="ActivitiesController as Main">
        <button type="button" ng-click="addNewActivityDialog()">Nueva Actividad</button><br /><br />

        <div ui-grid="gridOptions"></div>

        <script type="text/ng-template" id="actionsActivities.html">
            <div class="ui-grid-cell-contents">
                <button type="button" ng-click="grid.appScope.Main.removeActivity(COL_FIELD)">
                <span class="ui-icon ui-icon-trash"></span></button>
                <button type="button" ng-click="grid.appScope.Main.editActivity(COL_FIELD)">
                <span class="ui-icon ui-icon-pencil"></span></button>
            </div>
        </script>

        <script type="text/ng-template" id="editActivity.html">
            <fieldset>
                <legend>Editar actividades</legend>
                <form name="activityForm" ng-submit="saveEditedActivity()">
                	<div class="form-group">
                        <label class="col-md-4 control-label" for="generalActivityBox">Actividad general</label>
                        <div class="col-md-4">                            
                            <select ng-model="activityCopy.CODIGOACTIVIDAD" id="generalActivityBox" name="generalActivityBox" class="form-control"
                                ng-options="o.value as o.name for o in allGeneralActivities">
                            </select>
                            <br/><span class="help-block">Nombre de la actividad general</span> 
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="nombre">Actividad</label>  
                        <div class="col-md-4">
                            <input required ng-model="activityCopy.NOMBRE" id="nombre" name="nombre" type="text" placeholder="Actividad" class="form-control input-md" style="text-transform:uppercase;">
                            <br/><span class="help-block">Nombre de la actividad</span>
                            <span ng-messages="activityForm.nombre.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese una actividad</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="fecha">Fecha</label>  
                        <div class="col-md-4">
                            <input ng-model="activityCopy.FECHA" id="fecha" name="fecha" type="date" placeholder="Fecha" class="form-control input-md">
                            <br/><span class="help-block">Fecha de ejecución de la actividad</span>  
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="statusActivityBox">Estado</label>
                        <div class="col-md-4">
                            <select ng-model="activityCopy.ESTADO" id="statusActivityBox" name="statusActivityBox" class="form-control" ng-options="o.v as o.n for o in [{ n: 'Inactivo', v: 0 }, { n: 'En Proceso', v: 1 }, { n: 'Procesado', v: 2 }, { n: 'Finalizado', v: 3 }]">
                            </select>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="observacion">Observación</label>  
                        <div class="col-md-4">
                        	<textarea id="observacion" ng-model="activityCopy.OBSERVACION" class="title" placeholder="Observación" row="1" ng-maxlength="150" maxlength="150" style="text-transform:uppercase;"></textarea>
                            <br/><span class="help-block">Observación sobre la actividad</span>
                        </div>
                    </div>

                    <br/>
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="saveActivity"></label>
                        <div class="col-md-8">
                            <button type="submit" id="saveActivity" name="saveActivity" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>

        <script type="text/ng-template" id="newActivity.html">
            <fieldset>
                <legend>Nueva actividad</legend>
                <form name="newActivityForm" ng-submit="addNewActivityDB()">
                	<table style="width:100%">
                	<tr>
                	<td>
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="nombre">Actividad</label>  
                        <div class="col-md-4">
                            <input required ng-model="activityCopy.NOMBRE" id="nombre" name="nombre" type="text" placeholder="Actividad" class="form-control input-md" style="text-transform:uppercase;">
                            <br/><span class="help-block">Nombre de la actividad</span>  
                            <span ng-messages="activityForm.nombre.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese una actividad</span>
                            </span>
                        </div>
                    </div>
</td>
<td>
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="fecha">Fecha</label>  
                        <div class="col-md-4">
                            <input ng-model="activityCopy.FECHA" id="fecha" name="fecha" type="date" placeholder="Fecha" class="form-control input-md">
                            <br/><span class="help-block">Fecha de ejecución de la actividad</span>  
                        </div>
                    </div>
                    </td>
</tr>
</table>
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="activeUserBox">Estado</label>
                        <div class="col-md-4">
                            <select ng-model="activityCopy.ESTADO" id="activeUserBox" name="activeUserBox" class="form-control"
                                ng-options="o.v as o.n for o in [{ n: 'Inactivo', v: false }, { n: 'Activo', v: true }]">
                            </select>
                        </div>
                    </div>

					<div class="form-group">
                        <label class="col-md-4 control-label" for="observacion">Observación</label>  
                        <div class="col-md-4">
                        	<textarea id="observacion" ng-model="activityCopy.OBSERVACION" class="title" placeholder="Observación" row="1" ng-maxlength="150" maxlength="150" style="text-transform:uppercase;"></textarea>
                            <br/><span class="help-block">Observación sobre la actividad</span>
                        </div>
                    </div>

                    <br/>
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="saveActivity"></label>
                        <div class="col-md-8">
                            <button type="submit" id="saveActivity" name="saveActivity" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>
    </div>
</asp:Content>