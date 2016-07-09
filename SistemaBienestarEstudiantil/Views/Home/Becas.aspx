<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="SistemaBienestarEstudiantil.Class" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Becas
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContent" runat="server">
    <link href="<%=Utils.APP_CONTEXT%>/Scripts/ng-dialog/ngDialog-report.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <%
        Random rand = new Random((int)DateTime.Now.Ticks);
        int RandomNumber = rand.Next(100000, 999999);
    %>
    <script type="text/javascript" src="<%=Utils.APP_CONTEXT%>/Scripts/Controllers/becas.js?nocache=<%=RandomNumber%>"></script>
    <script type="text/javascript" src="<%=Utils.APP_CONTEXT%>/Scripts/Controllers/utils.js?nocache=<%=RandomNumber%>"></script>

    <div id="messages"></div>

    <h2>Becas</h2>
    <div ng-controller="BecasController as Main" ng-init='CODIGOUSUARIO=<%=@Session["userCode"]%>' class="main">
        <div style="font-size:18px;font-weight: bold;">Administrar Solicitudes de <a href="/Home/BecaSolicitud" target="_blank">Becas</a></div>
        <div style="position:fixed;top:0px;left:50%;margin-left:-85px;">
            <div cg-busy="{promise:promise,message:message,backdrop:backdrop,delay:delay,minDuration:minDuration}"></div>
        </div>
        <button ng-click="printBecas()" style="margin-bottom:5px;margin-top:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" title="Agregar tipo de beca">
            <span class="ui-button-icon-primary ui-icon ui-icon-print"></span><span class="ui-button-text">Imprimir</span>
        </button>
        <div ui-grid="gridOptions"></div>
        <br/><div style="font-size:18px;font-weight: bold;">Administrar Tipos de Becas y Documentos</div>
        <button ng-click="addTipoBecaDialog()" style="margin-bottom:5px;margin-top:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" title="Agregar tipo de beca">
            <span class="ui-button-icon-primary ui-icon ui-icon-circle-plus"></span><span class="ui-button-text">Nuevo</span>
        </button>
        <div ui-grid="gridOptionsTipos"></div>

        <script type="text/ng-template" id="actionsBecas.html">
              <div class="ui-grid-cell-contents" style="text-align:center">
                <button type="button" ng-click="grid.appScope.Main.removeBeca(COL_FIELD)" title="Eliminar solicitud de beca"><span class="ui-icon ui-icon-trash"></span></button>
                <button type="button" ng-click="grid.appScope.Main.editBeca(COL_FIELD)" title="Editar solicitud de beca"><span class="ui-icon ui-icon-pencil"></span></button>
                <button type="button" ng-click="grid.appScope.Main.viewHistoryChanges(COL_FIELD)" title="Ver historial de cambios"><span class="ui-icon ui-icon-note"></span></button>
              </div>
        </script>

        <script type="text/ng-template" id="actionsTiposBecas.html">
              <div class="ui-grid-cell-contents" style="text-align:center">
                <button type="button" ng-click="grid.appScope.Main.removeTipoBeca(COL_FIELD)" title="Eliminar tipo de beca"><span class="ui-icon ui-icon-trash"></span></button>
                <button type="button" ng-click="grid.appScope.Main.editTipoBeca(COL_FIELD)" title="Editar tipo de beca"><span class="ui-icon ui-icon-pencil"></span></button>
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

        <script type="text/ng-template" id="editBecas.html">
            <fieldset>
                <legend>Cambiar datos de la beca</legend>
                <form name="becaForm">
                    <table class="dialogClassTable">
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="nombreusuario">Nombre de usuario</label>  
                                    <div class="col-md-4">
                                        <div style="font-size:15px;width:250px;color:#508ECC;font-weight:bold;">{{solicitudbeca.NOMBRE}}</div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="beca">Tipo de Beca</label>  
                                    <div class="col-md-4">
                                        <div style="font-size:15px;width:250px;color:#508ECC;font-weight:bold;">{{solicitudbeca.BECA}}</div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="OTORGADO">% Otorgado</label>  
                                    <div class="col-md-4">
                                        <input ng-required="solicitudbeca.RUBRO != null" ng-model="solicitudbeca.OTORGADO" id="OTORGADO" name="OTORGADO" type="number" class="form-control input-md" max="100" min="0" style="height:20px;font-size:16px;padding-left:3px;">%
                                        <span ng-messages="becaForm.OTORGADO.$error">
                                            <span ng-message="required" class="help-block ng-message">Ingrese el porcentaje otorgado del rubro seleccionado</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="RUBRO">Rubro</label>  
                                    <div class="col-md-4">
                                        <select ng-required="solicitudbeca.OTORGADO != null" ng-model="solicitudbeca.RUBRO" id="RUBRO" name="RUBRO" class="form-control"
                                            ng-options="o.v as o.n for o in RUBROS" style="height:20px; width:150px;margin-top:5px;"></select>
                                        <span ng-messages="becaForm.RUBRO.$error">
                                            <span ng-message="required" class="help-block ng-message">Ingrese el rubro otorgado</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="estadobeca">Estado</label>  
                                    <div class="col-md-4">
                                        <select required ng-model="solicitudbeca.APROBADA" id="estadobeca" name="estadobeca" class="form-control"
                                            ng-options="o.v as o.n for o in ESTADOS" style="height:20px; width:150px;margin-top:5px;"></select>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="OBSERVACIONBECA">
                                        Observaci&oacute;n
                                        <input type="checkbox" ng-model="solicitudbeca.ENVIARNOTIFICACION" style="height:18px;width:18px;vertical-align:middle;" />
                                        <div style="font-size:12px;color:red;line-height:normal;">Seleccionar para enviar al correo del estudiante</div>
                                    </label>  
                                    <div class="col-md-4">
                                        <textarea ng-model="solicitudbeca.OBSERVACION" id="OBSERVACIONBECA" name="OBSERVACIONBECA" class="form-control"
                                            style="margin-top:5px;padding:3px;height:80px;width:250px;max-width:250px;font-size:10px;"></textarea>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label">Documentos que debe ingresar</label>  
                                    <div class="col-md-4">
                                        <div style="font-size:11px;margin-bottom:5px;width:250px;line-height:normal;">
                                            * Solicitud personal dirigida al Coordinador del Departamento de Bienestar Universitario
                                        </div>
                                        <div ng-repeat="documento in getTipoBecaByCodeSelected(solicitudbeca.TIPOCODIGO).BE_BECA_TIPO_DOCUMENTO"
                                            style="font-size:11px;margin-bottom:5px;width:250px;line-height:normal;">
                                            * {{documento.NOMBRE}}
                                        </div>
                                        <div style="width:250px;">
                                            <hr/>
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label">Documentos ingresados</label>  
                                    <div class="col-md-4">
                                        <div ng-if="solicitudbeca.ADJUNTOS == null || solicitudbeca.ADJUNTOS == undefined"
                                            style="font-size:12px;width:250px;color:red;">
                                            Cargando documentos ingresados...
                                        </div>
                                        <div ng-repeat="adjunto in solicitudbeca.ADJUNTOS"
                                            style="font-size:11px;width:250px;color:blue;line-height:normal;margin-bottom:5px;">
                                            * <a href="<%=Utils.APP_CONTEXT%>/WebServices/Becas.asmx/getAttach?code={{adjunto.CODIGO}}" target="_blank" title="Click para descargar documento">
                                                    {{adjunto.DESCRIPCION}}
                                            </a>
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="buttonsave1"></label>
                        <div class="col-md-8" style="margin-top:10px;">
                            <button ng-click="saveSolicitudBeca()" id="buttonsave1" name="buttonsave1" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>

        <script type="text/ng-template" id="addTipoBeca.html">
            <fieldset>
                <legend>Cambiar tipo de beca</legend>
                <form name="addtipoBecaForm">
                    <table class="dialogClassTable">
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="tipoBeca">Descripci&oacute;n del Tipo de Beca</label>  
                                    <div class="col-md-4">
                                        <textarea required ng-model="tipoBeca.NOMBRE" id="tipoBeca" name="tipoBeca"
                                            placeholder="Ingrese descripci&oacute;n" class="form-control input-md"
                                            style="margin: 3px; height: 76px; width: 226px;"></textarea>
                                        <span ng-messages="becaForm.tipoBeca.$error">
                                            <span ng-message="required" class="help-block ng-message">Ingrese nombre tipo</span>
                                        </span>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <label class="col-md-4 control-label" for="tipoBeca">Documentos solicitados</label>
                                    <div class="col-md-4">
                                        <button ng-click="addDocumento()" style="margin-bottom:5px;margin-top:5px;border:0px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" title="Agregar documento">
                                            <span class="ui-button-icon-primary ui-icon ui-icon-circle-plus"></span>
                                        </button>
                                        <div>
                                            <table style="border:0px;">
                                                <tr ng-repeat="documento in tipoBeca.BE_BECA_TIPO_DOCUMENTO" style="border:0px;">
                                                    <td style="border:0px;">
                                                        <textarea required ng-model="documento.NOMBRE" id="tipoBeca" name="tipoBeca"
                                                            placeholder="Ingrese descripci&oacute;n del documento" class="form-control input-md"
                                                            style="margin:3px;height:50px;max-height:50px;width:226px;max-width:226px;"></textarea>
                                                    </td>
                                                    <td style="border:0px;">
                                                        <button type="button" ng-click="removeDocumento(documento.ID)" title="Eliminar documento"><span class="ui-icon ui-icon-trash"></span></button>
                                                        </button>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>

                    <div class="form-group">
                        <label class="col-md-4 control-label" for="buttonsave1"></label>
                        <div class="col-md-8" style="margin-top:10px;">
                            <button ng-click="saveTipoBeca()" id="buttonsave1" name="buttonsave1" class="btn btn-success">Guardar</button>
                        </div>
                    </div>
                </form>
            </fieldset>
        </script>

        <script type="text/ng-template" id="viewHistoryChanges.html">
            <fieldset>
                <legend>Historial de cambios</legend>
                <div class="form-group">
                    <table>
                        <tr>
                            <td>USUARIO</td>
                            <td>FECHA</td>
                            <td>RUBRO</td>
                            <td>OTORGADO</td>
                        </tr>
                        <tr ng-if="solicitudbeca.BE_BECA_SOLICITUD_HISTORIAL.length == 0">
                            <td colspan="4">No tiene historial de cambios</td>
                        </tr>
                        <tr ng-repeat="historial in solicitudbeca.BE_BECA_SOLICITUD_HISTORIAL" style="color:#508ECC;font-size:15px;">
                            <td>{{historial.BE_USUARIO.NOMBREUSUARIO}}</td>
                            <td>{{convertDate(historial.FECHA) | date:"MM/dd/yyyy ' ' h:mma"}}</td>
                            <td>{{RUBROS[historial.RUBRO].n}}</td>
                            <td style="text-align:center;">{{historial.OTORGADO}}%</td>
                        </tr>
                    </table>
                </div>
            </fieldset>
        </script>

        <style type="text/css">
            .floatingDiv {width: 150px;float:left;margin:2px;margin-right:5px;margin-left:0;}
        </style>

        <script type="text/ng-template" id="printBecas.html">
            <div class="content_print" style="line-height: 14px;">
            <fieldset>
                <div class="form-group" style="font-size:12px;">
                    <div class="noprint">
                        <div class="floatingDiv">Carreras: <select ng-model="selectedSolicitud.NIVELCARRERA.CARRERA" style="height:20px; width:150px;margin-top:5px;"
                                ng-options="n as n for n in CARRERAS"></select></div>
                        <div class="floatingDiv">Nivel: <select ng-model="selectedSolicitud.NIVELCARRERA.NIVEL" style="height:20px; width:150px;margin-top:5px;"
                                ng-options="n as n for n in NIVELES"></select></div>
                        <div class="floatingDiv">Tipo de beca: <select ng-model="selectedSolicitud.BECA" style="height:20px; width:150px;margin-top:5px;"
                                ng-options="p.NOMBRE as p.NOMBRE for p in gridOptionsTipos.data"></select></div>
                        <div class="floatingDiv">Estado: <select ng-model="selectedSolicitud.ESTADO" style="height:20px; width:150px;margin-top:5px;"
                                ng-options="p.n as p.n for p in ESTADOS"></select></div>
                        <div class="floatingDiv">Rubro: <select ng-model="selectedSolicitud.RUBRO" style="height:20px; width:150px;margin-top:5px;"
                                ng-options="p.v as p.n for p in RUBROS"></select></div>
                        <div class="floatingDiv">Periodo: <select ng-model="selectedSolicitud.PERIODO.ID" style="height:20px; width:150px;margin-top:5px;"
                                ng-options="p.PRDCODIGOI as p.PERIODLABEL for p in PERIODOS"></select></div>
                        <div class="floatingDiv" style="width:100%;">
                            <button onclick="printElement('.content_print', 'Imprimir Reporte Becas')"
                                style="margin-bottom:5px;display: inline-block;" ng-disabled="(solicitudbecaReport | filter : selectedSolicitud : true).length == 0" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button">
                                <span class="ui-button-icon-primary ui-icon ui-icon-print"></span>
                                <span class="ui-button-text">Imprimir</span>
                            </button>
                            <button ng-click="exportExcelReport()" style="margin-bottom:5px;" class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-icon-primary" role="button" title="Exportar reporte a Excel">
                                <span class="ui-button-icon-primary ui-icon ui-icon-print"></span>
                                <span class="ui-button-text">Excel</span>
                            </button></div>
                        </div>
                    <table cellspacing=0 style="display: inline-block;">
                        <thead><tr>
                                <th>C&Eacute;DULA</th>
                                <th>NOMBRE</th>
                                <th>CARRERA</th>
                                <th>NIVEL</th>
                                <th>TIPO BECA</th>
                                <th>PORCENTAJE</th>
                                <th>ESTADO</th>
                                <th>RUBRO</th>
                                <th>PERIODO</th>
                        </tr></thead>
                        <tbody>
                            <tr ng-if="solicitudbecaReport == null || solicitudbecaReport == undefined">
                                <td colspan="9">No existen solicitudes de becas</td>
                            </tr>
                            <tr ng-if="(solicitudbecaReport | filter : selectedSolicitud : true).length == 0">
                                <td colspan="9">No existen solicitudes de becas con el filtro utilizado</td>
                            </tr>
                            <tr ng-repeat="solicitud in solicitudbecaReport | filter : selectedSolicitud : true" style="color:#508ECC;font-size:11px;">
                                <td>{{solicitud.CEDULA}}</td>
                                <td>{{solicitud.NOMBRE}}</td>
                                <td>{{solicitud.NIVELCARRERA.CARRERA}}</td>
                                <td>{{solicitud.NIVELCARRERA.NIVEL}}</td>
                                <td>{{solicitud.BECA}}</td>
                                <td style="text-align:center;">{{solicitud.OTORGADO}}%</td>
                                <td>{{solicitud.ESTADO}}</td>
                                <td>{{RUBROS[solicitud.RUBRO].n}}</td>
                                <td style="text-align:center;">{{solicitud.PERIODO.ID}}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </fieldset>
            </div>
        </script>
    </div>

</asp:Content>