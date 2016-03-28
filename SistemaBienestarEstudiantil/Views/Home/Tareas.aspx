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
                <button type="button" ng-click="grid.appScope.Main.removeActivity(COL_FIELD)" title="Elimiar actividad">
                <span class="ui-icon ui-icon-trash"></span></button>
                <button type="button" ng-click="grid.appScope.Main.editActivity(COL_FIELD)" title="Editar actividad">
                <span class="ui-icon ui-icon-pencil"></span></button>
                <button type="button" ng-click="grid.appScope.Main.getAssistance(COL_FIELD)" title="Asistencia alumnos">
                <span class="ui-icon ui-icon-person"></span></button>
                <button type="button" ng-click="grid.appScope.Main.editActivity(COL_FIELD)" title="Adjuntar archivos">
                <span class="ui-icon ui-icon-document"></span></button>
            </div>
        </script>

        <script type="text/ng-template" id="editActivity.html">
            <fieldset>
                <legend>Editar actividades</legend>
                <form name="activityForm" ng-submit="saveEditedActivity()">
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="generalActivityBox">Actividad general</label>
                        <div class="col-md-4">                            
                            <select ng-model="activityCopy.CODIGOACTIVIDAD" id="generalActivityBox" name="generalActivityBox" class="form-control" ng-options="o.value as o.name for o in allGeneralActivities"></select>
                            <br/><span class="help-block">Nombre de la actividad general</span>
                            <span ng-messages="activityForm.generalActivityBox.$error">
                                <span ng-message="required" class="help-block ng-message">Seleccione una actividad general</span>
                            </span>
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
                        <label class="col-md-4 control-label" for="responsableBox">Responsable</label>
                        <div class="col-md-4">                            
                            <select ng-model="activityCopy.CODIGOUSUARIO" id="responsableBox" name="responsableBox" class="form-control" ng-options="o.value as o.name for o in allResponsables">
                            </select>
                            <br/><span class="help-block">Responsable de ejecutar la actividad</span> 
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
                            <br/><span class="help-block">Estado en que se encuentra la actividad</span> 
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <table style="width:100%; font-size:16px">
                            <tr>
                                <th></th>
                                <th>Nivel</th>
                                <th>Paralelo</th>
                                <th>Modalidad</th>
                            </tr>
                            <tr ng-repeat="nivel in allGroupLevel">
                                <td><input type="checkbox" ng-checked="existGroupLevel(nivel.CODIGO)" 
                                    ng-click="setGroupLevel(nivel.CODIGO)"></td>
                                <td>{{ nivel.NIVEL }}</td>
                                <td>{{ nivel.PARALELO }}</td>
                                <td>{{ nivel.MODALIDAD }}</td>
                            </tr>
                        </table>
                    </div>

                    <br/>

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
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="generalActivityBox">Actividad general</label>
                        <div class="col-md-4">                            
                            <select required ng-model="activityCopy.CODIGOACTIVIDAD" id="generalActivityBox" name="generalActivityBox" class="form-control" ng-options="o.value as o.name for o in allGeneralActivities">
                            </select>
                            <br/><span class="help-block">Nombre de la actividad general</span>
                            <span ng-messages="newActivityForm.generalActivityBox.$error">
                                <span ng-message="required" class="help-block ng-message">* Campo obligatorio. Selecione una actividad general</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="nombre">Actividad</label>  
                        <div class="col-md-4">
                            <input required ng-model="activityCopy.NOMBRE" id="nombre" name="nombre" type="text" placeholder="Actividad" class="form-control input-md" style="text-transform:uppercase;">
                            <br/><span class="help-block">Nombre de la actividad</span>  
                            <span ng-messages="newActivityForm.nombre.$error">
                                <span ng-message="required" class="help-block ng-message">* Campo obligatorio. Ingrese el nombre de la actividad</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="responsableBox">Responsable</label>
                        <div class="col-md-4">                            
                            <select required ng-model="activityCopy.CODIGOUSUARIO" id="responsableBox" name="responsableBox" class="form-control" ng-options="o.value as o.name for o in allResponsables">
                            </select>
                            <br/><span class="help-block">Responsable de ejecutar la actividad</span>
                            <span ng-messages="newActivityForm.responsableBox.$error">
                                <span ng-message="required" class="help-block ng-message">* Campo obligatorio. Seleccione un responsable</span>
                            </span>
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="fecha">Fecha</label>  
                        <div class="col-md-4">
                            <input ng-model="activityCopy.FECHA" id="fecha" name="fecha" type="date" placeholder="Fecha" class="form-control input-md" required>
                            <br/><span class="help-block">Fecha de ejecución de la actividad</span> 
                            <span ng-messages="newActivityForm.fecha.$error">
                                <span ng-message="required" class="help-block ng-message">* Campo obligatorio. Seleccione una fecha</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="statusActivityBox">Estado</label>
                        <div class="col-md-4">
                            <select required ng-model="activityCopy.ESTADO" id="statusActivityBox" name="statusActivityBox" class="form-control"
                                ng-options="o.v as o.n for o in [{ n: 'Inactivo', v: 0 }, { n: 'En Proceso', v: 1 }, { n: 'Procesado', v: 2 }, { n: 'Finalizado', v: 3 }]">
                            </select>
                            <br/><span class="help-block">Estado en que se encuentra la actividad</span>
                            <span ng-messages="newActivityForm.statusActivityBox.$error">
                                <span ng-message="required" class="help-block ng-message">* Campo obligatorio. Seleccione un estado</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <table style="width:100%; font-size:16px">
                            <tr>
                                <th></th>
                                <th>Nivel</th>
                                <th>Paralelo</th>
                                <th>Modalidad</th>
                            </tr>
                            <tr ng-repeat="nivel in allGroupLevel">
                                <td><input type="checkbox" ng-checked="existGroupLevel(nivel.CODIGO)" 
                                    ng-click="setGroupLevel(nivel.CODIGO)"></td>
                                <td>{{ nivel.NIVEL }}</td>
                                <td>{{ nivel.PARALELO }}</td>
                                <td>{{ nivel.MODALIDAD }}</td>
                            </tr>
                        </table>
                    </div>

                    <br/>

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

        <script type="text/ng-template" id="assistanceActivity.html">
            <fieldset>
                <legend>Asistencia</legend>
                <form name="assistanceForm" ng-submit="saveAssistanceDB()">
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="levelBox">Nivel</label>
                        <div class="col-md-4">                            
                            <select ng-model="activityAssistanceCopy.CODIGO" id="levelBox" name="levelBox" class="form-control" ng-options="o.value as o.name for o in allLevelAssistance" ng-change="chargeStudents(activityAssistanceCopy.CODIGO)">
                            </select>
                            <br/><span class="help-block">Nivel para tomar asistencia</span>
                        </div>
                    </div>

                    <div class="form-group">
                        <table style="width:100%; font-size:16px">
                            <tr>
                                <th>Cédula</th>
                                <th>Nombre</th>
                                <th>Asistencia</th>
                            </tr>
                            <tr ng-repeat="student in studentsData">                                
                                <td>{{ student.ALUMNO.CEDULA }}</td>
                                <td>{{ student.ALUMNO.NOMBRE }}</td>
                                <td><input type="checkbox" ng-checked="student.ASISTENCIA1" 
                                    ng-click="setAssistanceStudents(student.CODIGO)"></td>
                            
                            </tr>
                        </table>
                    </div>

                    <br/>
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="saveAssistance"></label>
                        <div class="col-md-8">
                            <button type="submit" id="saveAssistance" name="saveAssistance" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>
    </div>
</asp:Content>