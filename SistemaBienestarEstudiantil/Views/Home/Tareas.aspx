<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="SistemaBienestarEstudiantil.Class" %>

<asp:Content ID="activityTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Actividades
</asp:Content>

<asp:Content ID="activityHeader" ContentPlaceHolderID="HeaderContent" runat="server">
    <link href="<%=Utils.APP_CONTEXT%>/Scripts/ng-dialog/ngDialog-report.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="activityContent" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>
    
    <script type="text/javascript" src="<%=Utils.APP_CONTEXT%>/Scripts/Utils/angular-messages.js"></script>
    <script type="text/javascript" src="<%=Utils.APP_CONTEXT%>/Scripts/file-saver/FileSaver.min.js"></script>
    <script type="text/javascript" src="<%=Utils.APP_CONTEXT%>/Scripts/Controllers/activities.js?nocache=<%=RandomNumber%>"></script>
    <script type="text/javascript" src="<%=Utils.APP_CONTEXT%>/Scripts/Controllers/utils.js?nocache=<%=RandomNumber%>"></script>

    <h2>Actividades</h2>

    <div id="messages"></div>

    <div ng-controller="ActivitiesController as Main">
        <div style="position:fixed;top:0px;left:50%;margin-left:-85px;">
            <div cg-busy="{promise:promise,message:message,backdrop:backdrop,delay:delay,minDuration:minDuration}"></div>
        </div>

        <button ng-click="addNewActivityDialog()" style="margin-bottom:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" title="Agregar actividad">
            <span class="ui-button-icon-primary ui-icon ui-icon-circle-plus"></span><span class="ui-button-text">Nuevo</span>
        </button>
        <button ng-click="openDialogAtivitiesReport()" style="margin-bottom:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" title="Imprimir reporte">
            <span class="ui-button-icon-primary ui-icon ui-icon-print"></span><span class="ui-button-text">Imprimir</span>
        </button>

        <div ui-grid="gridOptions"></div>

        <script type="text/ng-template" id="actionsActivities.html">
            <div class="ui-grid-cell-contents" style="text-align:center">
                <button type="button" ng-click="grid.appScope.Main.removeActivity(COL_FIELD)" title="Elimiar actividad">
                <span class="ui-icon ui-icon-trash"></span></button>
                <button type="button" ng-click="grid.appScope.Main.editActivity(COL_FIELD)" title="Editar actividad" ng-hide="grid.appScope.Main.getIsTeacher()">
                <span class="ui-icon ui-icon-pencil"></span></button>
                <button type="button" ng-click="grid.appScope.Main.getLevelActivity(COL_FIELD)" title="Grupos asignados">
                <span class="ui-icon ui-icon-script"></span></button>
                <button type="button" ng-click="grid.appScope.Main.getAssistance(COL_FIELD)" title="Asistencia alumnos">
                <span class="ui-icon ui-icon-person"></span></button>
                <button type="button" ng-click="grid.appScope.Main.getAttachedActivity(COL_FIELD)" title="Adjuntar archivos">
                <span class="ui-icon ui-icon-document"></span></button>
            </div>
        </script>

        <style type="text/css">
            .dialogClassTable,
            table.dialogClassTable tr,
            table.dialogClassTable td
            {
                border: none !important;
            }
        </style>

        <script type="text/ng-template" id="editActivity.html">
            <fieldset>
                <legend>Editar actividades</legend>
                <form name="activityForm" ng-submit="saveEditedActivity()">
                    <table class="dialogClassTable">
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="generalActivityBox">Actividad general</label>
                                    <div class="col-md-4">                            
                                        <select ng-model="activityCopy.CODIGOACTIVIDAD" id="generalActivityBox" name="generalActivityBox" class="form-control" ng-options="o.value as o.name for o in allGeneralActivities"></select>
                                        <br/><span class="help-block">Nombre de la actividad general</span>
                                        <span ng-messages="activityForm.generalActivityBox.$error">
                                            <span ng-message="required" class="help-block ng-message">Seleccione una actividad general</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
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
                            </td>
                        </tr>
                        <tr>
                            <td>

                                <div>
                                    <label class="col-md-4 control-label" for="responsableBox">Responsable
                                        <input type="checkbox" ng-checked="sendMail" ng-click="setSendMail()" style="height:18px; width:18px; vertical-align:middle;">
                                        <div style="font-size:12px; color:red; line-height:normal;">Seleccionar para enviar al correo del responsable</div>
                                    </label>
                                    <div class="col-md-4">                            
                                        <select ng-model="activityCopy.CODIGOUSUARIO" id="responsableBox" name="responsableBox" class="form-control" ng-options="o.value as o.name for o in allResponsables">
                                        </select>
                                        <br/><span class="help-block">Responsable de ejecutar la actividad</span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="fecha">Fecha</label>  
                                    <div class="col-md-4">
                                        <input ng-model="activityCopy.FECHA" id="fecha" name="fecha" type="date" placeholder="aaaa-mm-dd" form-control input-md>
                                        <br/><span class="help-block">Fecha de ejecuci&oacute;n de la actividad</span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="statusActivityBox">Estado</label>
                                    <div class="col-md-4">
                                        <select ng-model="activityCopy.ESTADO" id="statusActivityBox" name="statusActivityBox" class="form-control" ng-options="o.v as o.n for o in [{ n: 'Inactivo', v: 0 }, { n: 'En Proceso', v: 1 }, { n: 'Procesado', v: 2 }, { n: 'Finalizado', v: 3 }]">
                                        </select>
                                        <br/><span class="help-block">Estado en que se encuentra la actividad</span> 
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>                                
                                <div>
                                    <label class="col-md-4 control-label" for="observacion">Observaci&oacute;n</label>  
                                    <div class="col-md-4">
                                        <textarea id="observacion" ng-model="activityCopy.OBSERVACION" class="title" placeholder="Observaci&oacute;n" row="1" ng-maxlength="150" maxlength="150" style="text-transform:uppercase;max-width:250px;max-height:70px;min-width:250px;min-height:70px;"></textarea>
                                        <br/><span class="help-block">Observaci&oacute;n sobre la actividad (Lugar del evento)</span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>

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
                    <table class="dialogClassTable">
                        <tr>
                            <td>
                                <div>
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
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
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
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="responsableBox">Responsable
                                        <input type="checkbox" ng-checked="sendMail" ng-click="setSendMail()" style="height:18px; width:18px; vertical-align:middle;">
                                        <div style="font-size:12px; color:red; line-height:normal;">Seleccionar para enviar al correo del responsable</div>
                                    </label>
                                    <div class="col-md-4">                            
                                        <select required ng-model="activityCopy.CODIGOUSUARIO" id="responsableBox" name="responsableBox" class="form-control" ng-options="o.value as o.name for o in allResponsables">
                                        </select>
                                        <br/><span class="help-block">Responsable de ejecutar la actividad</span>
                                        <span ng-messages="newActivityForm.responsableBox.$error">
                                            <span ng-message="required" class="help-block ng-message">Seleccione un responsable</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>

                        <tr>
                            <td>                    
                                <div>
                                    <label class="col-md-4 control-label" for="fecha">Fecha</label>  
                                    <div class="col-md-4">
                                        <input ng-model="activityCopy.FECHA" id="fecha" name="fecha" type="date" placeholder="aaaa-mm-dd" class="form-control input-md" required>
                                        <br/><span class="help-block">Fecha de ejecuci&oacute;n de la actividad</span> 
                                        <span ng-messages="newActivityForm.fecha.$error">
                                            <span ng-message="required" class="help-block ng-message">Seleccione una fecha</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <div>
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
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="observacion">Observaci&oacute;n</label>  
                                    <div class="col-md-4">
                                        <textarea id="observacion" ng-model="activityCopy.OBSERVACION" class="title" placeholder="Observaci&oacute;n" row="1" ng-maxlength="150" maxlength="150" style="text-transform:uppercase;max-width:160px;max-height:70px;min-width:250px;min-height:70px;"></textarea>
                                        <br/><span class="help-block">Observaci&oacute;n sobre la actividad (Lugar del evento)</span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>

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
            <form name="groupActivityForm">
                <div ng-show="view == 'faculty'">
                    <fieldset>
                        <legend>Facultades</legend>
                        <button class="btn btn-success" style="margin-bottom:5px" ng-click="cambiarVista('school')">Siguiente</button>
                        <table>
                            <tr>
                                <th></th>
                                <th>Nombre</th> 
                            </tr>
                            <tr ng-repeat="faculty in allFaculties">
                                <td style="width:50px"><input type="checkbox" ng-checked="existFacultyData(faculty.FCLCODIGOI)" ng-click="setSelectedFaculties(faculty.FCLCODIGOI)"></td>
                                <td style="width:100%">{{ faculty.FCLNOMBREC }}</td>
                            </tr>
                        </table>
                    </fieldset>
                </div>

                <div ng-show="view == 'school'">
                    <fieldset>
                        <legend>Escuelas</legend>
                        <div style="display:inline">
                            <button class="btn btn-success" style="margin-bottom:5px" ng-click="cambiarVista('career')">Siguiente</button>
                            <button class="btn btn-success" style="margin-bottom:5px" ng-click="cambiarVista('faculty')">Anterior</button>
                        </div>
                        <table>
                            <tr>
                                <th></th>
                                <th>Nombre</th> 
                            </tr>
                            <tr ng-repeat="school in allSchools">
                                <td style="width:50px"><input type="checkbox" ng-checked="existSchoolData(school.ESCCODIGOI)" ng-click="setSelectedSchools(school.ESCCODIGOI)"></td>
                                <td style="width:100%">{{ school.ESCNOMBREC }}</td>
                            </tr>
                        </table>
                    </fieldset>
                </div>

                <div ng-show="view == 'career'">
                    <fieldset>
                        <legend>Carreras</legend>
                        <div style="display:inline">
                            <button class="btn btn-success" style="margin-bottom:5px" ng-click="cambiarVista('modality')">Siguiente</button>
                            <button class="btn btn-success" style="margin-bottom:5px" ng-click="cambiarVista('school')">Anterior</button>
                        </div>
                        <table>
                            <tr>
                                <th></th>
                                <th>Nombre</th> 
                            </tr>
                            <tr ng-repeat="career in allCareers">
                                <td style="width:50px"><input type="checkbox" ng-checked="existCareerData(career.CRRCODIGOI)" ng-click="setSelectedCareers(career.CRRCODIGOI)"></td>
                                <td style="width:100%">{{ career.CRRDESCRIPC }}</td>
                            </tr>
                        </table> 
                    </fieldset>
                </div>

                <div ng-show="view == 'modality'">
                    <fieldset>
                        <legend>Modalidades</legend>
                        <div style="display:inline">
                            <button class="btn btn-success" style="margin-bottom:5px" ng-click="cambiarVista('level')">Siguiente</button>
                            <button class="btn btn-success" style="margin-bottom:5px" ng-click="cambiarVista('career')">Anterior</button>
                        </div>
                        <table>
                            <tr>
                                <th></th>
                                <th>Nombre</th> 
                            </tr>
                            <tr ng-repeat="modality in allModalities">
                                <td style="width:50px"><input type="checkbox" ng-checked="existModalityData(modality.MDLCODIGOI)" ng-click="setSelectedModalities(modality.MDLCODIGOI)"></td>
                                <td style="width:100%">{{ modality.MDLDESCRIPC }}</td>
                            </tr>
                        </table>
                    </fieldset>
                </div>

                <div ng-show="view == 'level'">
                    <fieldset>
                        <legend>Niveles</legend>
                        <button class="btn btn-success" style="margin-bottom:5px" ng-click="cambiarVista('modality')">Anterior</button>
                        <table>
                            <tr>
                                <th></th>
                                <th>Nombre</th> 
                            </tr>
                            <tr ng-repeat="level in allLevels">
                                <td style="width:50px"><input type="checkbox" ng-checked="existLevelData(level.NVLCODIGOI)" ng-click="setSelectedLevels(level.NVLCODIGOI)"></td>
                                <td style="width:100%">{{ level.NVLDESCRIPC }}</td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
                
                <div class="form-group">
                    <label class="col-md-4 control-label" for="saveLevel"></label>
                    <div class="col-md-8">
                        <button type="submit" id="saveLevel" name="saveLevel" class="btn btn-success" ng-click="saveGroupActivity()">Guardar</button>
                    </div>
                </div>
            </form>
        </script>

        <script type="text/ng-template" id="assistanceActivity.html">
            <div style="font-size:12px; color:red; line-height:normal; display:inline;">Enviar notificaci&oacute;n a los alumnos</div>
            <button class="btn btn-success" style="margin-bottom:5px" ng-click="notifyActivityStudents()">Enviar</button>
            <fieldset>
                <legend>Asistencia</legend>
                <form name="assistanceForm" ng-submit="saveAssistanceDB()">
                    <div class="form-group">
                        <table style="width:100%; font-size:12px">
                            <tr>
                                <th style="padding:2px; text-align:center"><input type="checkbox" ng-checked="checkedAll" ng-click="setAllStudents()"></th>
                                <th>Nombre</th>
                                <th>C&eacute;dula</th>
                            </tr>
                            <tr ng-repeat="student in allLevelAssistance">
                                <td style="padding:2px; text-align: center"><input type="checkbox" ng-checked="student.ASISTENCIA" 
                                    ng-click="setAssistanceStudents(student.CODIGO)"></td>                               
                                <td style="padding:2px; padding-left:5px">{{ student.NOMBRE }}</td>
                                <td style="padding:2px">{{ student.CEDULA }}</td>
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
                    <div ng-if="allAttaches.length < 10">
                        <div>
                            <textarea required name="observacion" id="observacion" ng-model="descripcion" class="title" placeholder="Descripci&oacute;n" row="1" ng-maxlength="150" maxlength="240" style="text-transform:uppercase; width:450px"></textarea><br/>
                            <input valid-file-input required ng-model="attachedActivity" type="file" name="attachedActivity" id="attachedActivity" accept="image/*, application/pdf"/>
                            <span ng-show="attachedForm.attachedActivity.$error.validFile" class="help-block ng-message" style="font-size: 18px;">* Debe adjuntar documento</span>
                            <span ng-show="attachedForm.attachedActivity.$error.validFileSize" class="help-block ng-message" style="font-size: 18px;">* Solo se permiten documentos hasta 2MB</span>
                            <span ng-show="attachedForm.attachedActivity.$error.validFileEmpty" class="help-block ng-message" style="font-size: 18px;">* El fichero est&aacute; vac&iacute;o</span>
                            <span ng-show="attachedForm.attachedActivity.$error.validFileType" class="help-block ng-message" style="font-size: 18px;">* No se admite el tipo de archivo</span>
                        </div>
                    
                        <div class="form-group">
                            <label class="col-md-4 control-label" for="saveAttached"></label>
                            <div class="col-md-8">
                                <button type="submit" id="saveAttached" name="saveAttached" class="btn btn-success">A&ntilde;adir</button>
                            </div>
                        </div>

                        <br/><br/>
                    </div>
                    <div class="form-group" ng-if="allAttaches.length > 0">
                        <table style="width:100%; font-size:12px">
                            <tr>
                                <th>Descripci&oacute;n</th>
                                <th>Adjunto</th>
                                <th>Acci&oacute;n</th>
                            </tr>
                            <tr ng-repeat="attach in allAttaches">                                
                                <td style="width:200px">{{ attach.DESCRIPCION }}</td>
                                <td><a href="<%=Utils.APP_CONTEXT%>/WebServices/Activities.asmx/getAttach?code={{attach.CODIGO}}" target="_blank" title="Click para descargar documento">
                                        {{attach.NOMBRE}}
                                </a></td>
                                <td style="text-align:center"><button title="Eliminar" type="button" ng-click="removeAttach(attach.CODIGO)"><span class="ui-icon ui-icon-trash"></span></button></td>
                            </tr>
                        </table>
                    </div>
                </form>
            </fieldset>
        </script>

        <script type="text/ng-template" id="activitiesReport.html">
            <div class="content_print" style="line-height: 14px;">
            <fieldset>
                <div class="form-group" style="font-size:12px;">
                    <div class="noprint">
                        Desde: <input name="date1" id="date1" ng-model="date.dateFrom" type="date" />
                        hasta: <input name="date2" id="date2" ng-model="date.dateTo" type="date" />
                        <button onclick="printElement('.content_print', 'Imprimir Reporte Actividades')"
                            ng-disabled="(reportActivitiesData | rangeDateFilter:date.dateFrom:date.dateTo).length == 0"
                            style="margin-bottom:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button">
                            <span class="ui-button-icon-primary ui-icon ui-icon-print"></span>
                            <span class="ui-button-text">Imprimir</span>
                        </button>
                        <button ng-click="exportExcelReport()" style="margin-bottom:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" title="Imprimir reporte">
                            <span class="ui-button-icon-primary ui-icon ui-icon-print"></span>
                            <span class="ui-button-text">Excel</span>
                        </button>
                    </div>
                    <table cellspacing=0>
                        <thead><tr>
                                <th>FECHA</th>
                                <th>ACTIVIDAD</th>
                                <th>A. GENERAL</th>
                                <th>ESTADO</th>
                                <th>ASIST.</th>
                                <th>NIVEL</th>
                                <th>CARRERA</th>
                                <th>MOD.</th>
                                <th>DOC. ADJ.</th>
                        </tr></thead>
                        <tbody>
                            <tr ng-if="(reportActivitiesData | rangeDateFilter:date.dateFrom:date.dateTo).length == 0">
                                <td colspan="9">No existen actividades en este rango de fechas</td>
                            </tr>
                            <tr ng-if="reportActivitiesData == null || reportActivitiesData == undefined">
                                <td colspan="9">No existen actividades</td>
                            </tr>
                            <tr ng-repeat="activitiesReport in reportActivitiesData | rangeDateFilter:date.dateFrom:date.dateTo" style="color:#508ECC;font-size:11px;">
                                <td>{{activitiesReport.FECHA | date:"MM/dd/yyyy"}}</td>
                                <td>{{activitiesReport.ACTIVIDAD}}</td>
                                <td>{{activitiesReport.ACTIVIDADGENERAL}}</td>
                                <td>{{ESTADOS[activitiesReport.ESTADO].name}}</td>
                                <td style="text-align:center;">{{activitiesReport.DATOS.ASISTENCIA}}</td>
                                <td><div class="item-report" ng-repeat="nivel in activitiesReport.DATOS.NIVELES">{{nivel}}</div></td>
                                <td><div class="item-report" ng-repeat="carrera in activitiesReport.DATOS.CARRERAS">-{{carrera}}</div></td>
                                <td><div class="item-report" ng-repeat="modalidad in activitiesReport.DATOS.MODALIDADES">{{modalidad}}</div></td>
                                <td><div class="item-report" ng-repeat="adjunto in activitiesReport.DATOS.ADJUNTOS">{{adjunto}};</div></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </fieldset>
            </div>
        </script>

    </div>
</asp:Content>