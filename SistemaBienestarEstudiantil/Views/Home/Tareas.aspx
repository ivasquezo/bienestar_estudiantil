<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="activityTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Actividades
</asp:Content>

<asp:Content ID="activityContent" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>

    <link href="../../Content/activities.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../../Scripts/Utils/angular-messages.js"></script>
    <script type="text/javascript" src="../../Scripts/Controllers/activities.js?nocache=<%=RandomNumber%>"></script>

    <h2>Actividades</h2>

    <div id="messages"></div>

    <div ng-controller="ActivitiesController as Main">
        <div cg-busy="{promise:promise, message:message, backdrop:backdrop, delay:delay, minDuration:minDuration}"></div>

        <button ng-click="addNewActivityDialog()" style="margin-bottom:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" title="Agregar Actividad">
            <span class="ui-button-icon-primary ui-icon ui-icon-circle-plus"></span><span class="ui-button-text">Nuevo</span>
        </button>

        <div ui-grid="gridOptions"></div>

        <script type="text/ng-template" id="actionsActivities.html">
            <div class="ui-grid-cell-contents">
                <button type="button" ng-click="grid.appScope.Main.removeActivity(COL_FIELD)" title="Elimiar actividad">
                <span class="ui-icon ui-icon-trash"></span></button>
                <button type="button" ng-click="grid.appScope.Main.editActivity(COL_FIELD)" title="Editar actividad">
                <span class="ui-icon ui-icon-pencil"></span></button>
                <button type="button" ng-click="grid.appScope.Main.getLevelActivity(COL_FIELD)" title="Grupos asignados">
                <span class="ui-icon ui-icon-script"></span></button>
                <button type="button" ng-click="grid.appScope.Main.getAssistance(COL_FIELD)" title="Asistencia alumnos">
                <span class="ui-icon ui-icon-person"></span></button>
                <button type="button" ng-click="grid.appScope.Main.getAttachedActivity(COL_FIELD)" title="Adjuntar archivos">
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
                            <input valid-activity-name required ng-model="activityCopy.NOMBRE" id="nombre" name="nombre" type="text" placeholder="Actividad" class="form-control input-md" style="text-transform:uppercase;">
                            <br/><span class="help-block">Nombre de la actividad</span>
                            <span ng-messages="activityForm.nombre.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese una actividad</span>
                                <span ng-message="activityNameExist" class="help-block ng-message">Existe una actividad con este nombre</span>
                                <span ng-message="activityNameValidator" class="help-block ng-message">Debe ingresar un nombre de actividad v&aacute;lido</span>
                                <span ng-message="activityNameChecking" class="help-block ng-message">Chequeando la base de datos...</span>
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
                            <br/><span class="help-block">Fecha de ejecuci&oacute;n de la actividad</span>  
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
                        <label class="col-md-4 control-label" for="observacion">Observaci&oacute;n</label>  
                        <div class="col-md-4">
                            <textarea id="observacion" ng-model="activityCopy.OBSERVACION" class="title" placeholder="Observaci&oacute;n" row="1" ng-maxlength="150" maxlength="150" style="text-transform:uppercase;"></textarea>
                            <br/><span class="help-block">Observaci&oacute;n sobre la actividad</span>
                        </div>
                    </div>

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
                                <span ng-message="required" class="help-block ng-message">Selecione una actividad general</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="nombre">Actividad</label>  
                        <div class="col-md-4">
                            <input valid-activity-name required ng-model="activityCopy.NOMBRE" id="nombre" name="nombre" type="text" placeholder="Actividad" class="form-control input-md" style="text-transform:uppercase;">
                            <br/><span class="help-block">Nombre de la actividad</span>  
                            <span ng-messages="newActivityForm.nombre.$error">
                                <span ng-message="required" class="help-block ng-message">Ingrese el nombre de la actividad</span>
                                <span ng-message="activityNameExist" class="help-block ng-message">Existe una actividad con este nombre</span>
                                <span ng-message="activityNameValidator" class="help-block ng-message">Debe ingresar un nombre de actividad v&aacute;lido</span>
                                <span ng-message="activityNameChecking" class="help-block ng-message">Chequeando la base de datos...</span>
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
                                <span ng-message="required" class="help-block ng-message">Seleccione un responsable</span>
                            </span>
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="fecha">Fecha</label>  
                        <div class="col-md-4">
                            <input ng-model="activityCopy.FECHA" id="fecha" name="fecha" type="date" placeholder="Fecha" class="form-control input-md" required>
                            <br/><span class="help-block">Fecha de ejecuci&oacute;n de la actividad</span> 
                            <span ng-messages="newActivityForm.fecha.$error">
                                <span ng-message="required" class="help-block ng-message">Seleccione una fecha</span>
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
                                <span ng-message="required" class="help-block ng-message">Seleccione un estado</span>
                            </span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="observacion">Observaci&oacute;n</label>  
                        <div class="col-md-4">
                            <textarea id="observacion" ng-model="activityCopy.OBSERVACION" class="title" placeholder="Observaci&oacute;n" row="1" ng-maxlength="150" maxlength="150" style="text-transform:uppercase;"></textarea>
                            <br/><span class="help-block">Observaci&oacute;n sobre la actividad</span>
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="saveActivity"></label>
                        <div class="col-md-8">
                            <button type="submit" id="saveActivity" name="saveActivity" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>

        <script type="text/ng-template" id="getLevel.html">
            <form name="groupActivityForm" ng-submit="saveGroupActivity()" class="activities">
                <button type="submit" id="saveActivity" name="saveActivity" class="btn btn-success">Asignar grupo</button><br/><br/>

                <div style="font-size:16px;" ng-click="cambiarVista('faculty')" ng-class="view == 'faculty' ? 'selected' : '' " class="title-report">Facultas</div>
                <div style="font-size:16px;" ng-click="cambiarVista('school')" ng-class="view == 'school' ? 'selected' : '' " class="title-report">Escuela</div>
                <div style="font-size:16px;" ng-click="cambiarVista('career')" ng-class="view == 'career' ? 'selected' : '' " class="title-report">Carrera</div>
                <div style="font-size:16px;" ng-click="cambiarVista('modality')" ng-class="view == 'modality' ? 'selected' : '' " class="title-report">Modalidad</div>
                <div style="font-size:16px;" ng-click="cambiarVista('level')" ng-class="view == 'level' ? 'selected' : '' " class="title-report">Nivel</div>
                <div style="font-size:16px;" ng-click="cambiarVista('assigned')" ng-class="view == 'assigned' ? 'selected' : '' " class="title-report">Asignados</div>

                <br/><br/>

                <div ng-show="view == 'faculty'">
                    <table>
                        <tr>
                            <th></th>
                            <th>Nombre</th> 
                        </tr>
                        <tr ng-repeat="faculty in allFaculties">
                            <td style="width:50px"><input type="checkbox" ng-click="setSelectedFaculties(faculty.FCLCODIGOI)"></td>
                            <td>{{ faculty.FCLNOMBREC }}</td>
                        </tr>
                    </table>
                </div>

                <div ng-show="view == 'school'">
                    <table>
                        <tr>
                            <th></th>
                            <th>Nombre</th> 
                        </tr>
                        <tr ng-repeat="school in allSchools">
                            <td style="width:50px"><input type="checkbox" ng-click="setSelectedSchools(school.ESCCODIGOI)"></td>
                            <td>{{ school.ESCNOMBREC }}</td>
                        </tr>
                    </table>
                </div>

                <div ng-show="view == 'career'">
                    <table>
                        <tr>
                            <th></th>
                            <th>Nombre</th> 
                        </tr>
                        <tr ng-repeat="career in allCareers">
                            <td style="width:50px"><input type="checkbox" ng-click="setSelectedCareers(career.CRRCODIGOI)"></td>
                            <td>{{ career.CRRDESCRIPC }}</td>
                        </tr>
                    </table>                 
                </div>

                <div ng-show="view == 'modality'">
                    <table>
                        <tr>
                            <th></th>
                            <th>Nombre</th> 
                        </tr>
                        <tr ng-repeat="modality in allModalities">
                            <td style="width:50px"><input type="checkbox" ng-click="setSelectedModalities(modality.MDLCODIGOI)"></td>
                            <td>{{ modality.MDLDESCRIPC }}</td>
                        </tr>
                    </table>
                </div>

                <div ng-show="view == 'level'">
                    <table>
                        <tr>
                            <th></th>
                            <th>Nombre</th> 
                        </tr>
                        <tr ng-repeat="level in allLevels">
                            <td style="width:50px"><input type="checkbox" ng-click="setSelectedLevels(level.NVLCODIGOI)"></td>
                            <td>{{ level.NVLDESCRIPC }}</td>
                        </tr>
                    </table>
                </div>

                <div ng-show="view == 'assigned'">
                    <table>
                        <tr>
                            <th></th>
                            <th>Carrera</th> 
                            <th>Modalidad</th> 
                            <th>Nivel</th> 
                        </tr>
                        <tr ng-repeat="level in allLevels">
                            <td style="width:50px"><input type="checkbox" ng-checked="existAccess(level.NVLCODIGOI)" ng-click="setSelectedLevels(level.NVLCODIGOI)"></td>
                            <td>{{ level.NVLDESCRIPC }}</td>
                        </tr>
                    </table>
                </div>
            </form>
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

        <script type="text/ng-template" id="attachedActivity.html">
            <fieldset>
                <legend>Adjuntos</legend>
                <form id="attachedForm" name="attachedForm" ng-submit="saveAttachedDB()">
                    <ng-form name="innerForm">
                        <input valid-file-input ng-model="fileName" type="file" name="fileName" id="fileName" accept="image/*, application/pdf"/>
                        <span ng-show="innerForm.fileName.$error.validFileSize" class="help-block ng-message" style="font-size: 18px;">* Solo se permiten documentos hasta 2MB</span>
                        <span ng-show="innerForm.fileN9ame.$error.validFileEmpty" class="help-block ng-message" style="font-size: 18px;">* El fichero está vacío</span>
                        <span ng-show="innerForm.fileName.$error.validFileType" class="help-block ng-message" style="font-size: 18px;">* No se admite el tipo de archivo</span>
                    </ng-form>

                    <br/>
                    <div class="form-group">
                        <label class="col-md-4 control-label" for="saveAttached"></label>
                        <div class="col-md-8">
                            <button type="submit" id="saveAttached" name="saveAttached" class="btn btn-success">Guardar</button>
                        </div>
                    </div>

                    <br/>
                    <div class="form-group">
                        <table style="width:100%; font-size:16px">
                            <tr>
                                <th>Adjunto</th>
                                <th>Nombre</th>
                                <th>Descripción</th>
                                <th></th>
                            </tr>
                            <tr >                                
                                <td></td>
                                <td></td>
                                <td></td>
                                <td><button type="button" ng-click="grid.appScope.Main.removeActivity(COL_FIELD)" title="Elimiar actividad">
                <span class="ui-icon ui-icon-trash"></span></button></td>                            
                            </tr>
                        </table>
                    </div>
                </form>
            </fieldset>
        </script>
    </div>
</asp:Content>